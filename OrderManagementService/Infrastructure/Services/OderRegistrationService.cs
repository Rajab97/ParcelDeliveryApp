using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.Common.Models;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.DTOs.Response;
using OrderManagementService.Application.Services;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Helpers.Configs;
using OrderManagementService.Helpers.Models;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Enums;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Commons;
using SharedLibrary.Models.Constants;
using SharedLibrary.Models.KafkaSchemaRegistry;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagementService.Infrastructure.Services
{
    public class OderRegistrationService : IOderRegistrationService
    {
        private readonly ILogger<OderRegistrationService> _logger;
        private readonly IRepository<Order> _ordersRepo;
        private readonly IMapper _mapper;
        private readonly CurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaHelperService _kafkaHelperService;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IAccountService _accountService;
        private readonly IDeliveryService _deliveryService;
        private readonly IRepository<OutBoxMessage> _outBoxRepo;

        public OderRegistrationService(ILogger<OderRegistrationService> logger,
                                        IRepository<Order> ordersRepo,
                                            IMapper mapper,
                                                CurrentUser currentUser,
                                                    IUnitOfWork unitOfWork,
                                                        IKafkaHelperService kafkaHelperService,
                                                            IOptions<KafkaConfig> kafkaConfig,
                                                                IAccountService accountService,
                                                                    IDeliveryService deliveryService,
                                                                        IRepository<OutBoxMessage> outBoxRepo)
        {
            _logger = logger;
            _ordersRepo = ordersRepo;
            _mapper = mapper;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _kafkaHelperService = kafkaHelperService;
            _kafkaConfig = kafkaConfig.Value;
            _accountService = accountService;
            _deliveryService = deliveryService;
            _outBoxRepo = outBoxRepo;
        }

        public async Task<ApiResponse<CreateOrderResponseDTO>> CreateOrderAsync(CreateOrderRequestDTO requestDTO)
        {
            _logger.LogInformation("CreateOrderAsync method called.");
            _logger.LogInformation($"Request model: {JsonConvert.SerializeObject(requestDTO)}");
            if (requestDTO.OrderItems == null || !requestDTO.OrderItems.Any())
                throw new ApplicationException("You must add minimum one item to order");

            var orderModel = _mapper.Map<Order>(requestDTO);
            var orderITems = _mapper.Map<List<OrderItem>>(requestDTO.OrderItems);
            orderModel.OrderNumber = GenerateOrderNumber();
            orderModel.OrderItems =orderITems;
            orderModel.OrderStatus = OrderStatusTypes.Pending.ToString();
            orderModel.CustomerFirstName = _currentUser.FirstName;
            orderModel.CustomerLastName = _currentUser.LastName;
            orderModel.CustomerId = _currentUser.UserId;
            orderModel.InvoiceAmount = GetTotalInvoiceAmount(orderITems);
            await _ordersRepo.AddAsync(orderModel);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Order created successfully");
            _logger.LogInformation($"Persisted order {JsonConvert.SerializeObject(orderModel,new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            var resultModel = _mapper.Map<CreateOrderResponseDTO>(orderModel);
            return ApiResponse.Success(resultModel);
        }

        public async Task<ApiResponse> ChangeOrderAddressesAsync(ChangeOrderAddressRequestDTO requestModel)
        {
            _logger.LogInformation($"${nameof(ChangeOrderAddressesAsync)} method called");
            _logger.LogInformation($"Request model: {JsonConvert.SerializeObject(requestModel)}");
            var order = await _ordersRepo.GetIQueryable().FirstOrDefaultAsync(m=>m.Id == requestModel.OrderId);
            if (order == null)
                throw new NotFoundException();
            if (!_currentUser.IsAdmin && order.CustomerId != _currentUser.UserId)
                throw new ApplicationException("You can only update your order.");
            if (order.OrderStatus != OrderStatusTypes.Pending.ToString())
                throw new ApplicationException($"The order should be in {OrderStatusTypes.Pending} status for changing its addresses.");

            var model = _mapper.Map(requestModel,order);

            await _ordersRepo.UpdateAsync(model);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Order addresses changed successfully");
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> ChangeOrderStatusAsync(ChangeOrderStatusRequestDTO requestModel)
        {
            try
            {
                _logger.LogInformation($"${nameof(ChangeOrderStatusAsync)} method called");
                _logger.LogInformation($"Request model: {JsonConvert.SerializeObject(requestModel)}");
                var order = await _ordersRepo.GetIQueryable().FirstOrDefaultAsync(m => m.OrderNumber == requestModel.OrderNumber);
                if (order == null)
                    throw new NotFoundException();

                if (!OrderStatusFlow.IsStatusExist(requestModel.OrderStatus))
                    throw new ApplicationException($"{requestModel.OrderStatus} status not exist");

                if (!OrderStatusFlow.IsTransitionAllowed(order.OrderStatus, requestModel.OrderStatus))
                    throw new ApplicationException($"Transtion doesn't exist between {order.OrderStatus} and {requestModel.OrderStatus}");

                order.OrderStatus = requestModel.OrderStatus;
                order.UpdatedAt = requestModel.StatusChangeDate;

                await _ordersRepo.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Order status changed successfully");
                return ApiResponse.Success();
            }
            catch (ApplicationException ae)
            {
                _logger.LogError(ae, "Error occured while change order status");
                return ApiResponse.Error(ae.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, ExceptionMessages.ExceptionOccured);
                return ApiResponse.Error(ExceptionMessages.ExceptionOccured);
            }
        }

        public async Task<ApiResponse> AssignToCurierAsync(AssignToCurierRequestDTO requestModel)
        {
            _logger.LogInformation($"${nameof(AssignToCurierAsync)} method called");
            _logger.LogInformation($"Request model: {JsonConvert.SerializeObject(requestModel)}");
            var order = await _ordersRepo.GetIQueryable().Include(m=>m.OrderItems).FirstOrDefaultAsync(m => m.Id == requestModel.OrderId);
            if (order == null)
                throw new NotFoundException();
            if (order.OrderStatus != OrderStatusTypes.Pending.ToString())
                throw new ApplicationException("You can assign only orders in Pending status");

            var courier = await _accountService.GetCourierAsync(requestModel.CourierId); 
            order.AssignedBy = $"{_currentUser.FirstName} {_currentUser.LastName}";
            order.OrderStatus = OrderStatusTypes.InProgress.ToString();
            AssignToCurierMessage message = new AssignToCurierMessage()
            {
                AssignedBy = order.AssignedBy,
                BillingAddress = order.BillingAddress,
                CustomerFullName = $"{order.CustomerFirstName} {order.CustomerLastName}",
                CustomerId = order.CustomerId,
                InvoiceAmount = order.InvoiceAmount,
                NumberOfItems = order.OrderItems.Sum(m=>m.Quantity),
                OrderNumber = order.OrderNumber,
                OrderId= order.Id,
                ShippingAddress = order.ShippingAddress,
                CourierId= courier.Id,
                CourierFullName = $"{courier.FirstName} {courier.LastName}",
                CourierUserName = courier.UserName
            };
            await _outBoxRepo.AddAsync(new OutBoxMessage() { Message = JsonConvert.SerializeObject(message) , Topic = KafkaTopics.ASSIGN_TO_COURIER });
            //Use outBox pattern for producing more reliable messages
            //await _kafkaHelperService.ProduceMessageAsync(message, KafkaTopics.ASSIGN_TO_COURIER);
            await _ordersRepo.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Successfully assigned to courier");
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> CancelOrderAsync(CancelOrderRequestDTO requestModel)
        {
            _logger.LogInformation($"${nameof(CancelOrderAsync)} method called");
            _logger.LogInformation($"Request model: {JsonConvert.SerializeObject(requestModel)}");
            var order = await _ordersRepo.GetIQueryable().Include(m => m.OrderItems).FirstOrDefaultAsync(m => m.Id == requestModel.OrderId);
            if (order == null)
                throw new NotFoundException();

            var response = await _deliveryService.IsCancellationAllowedAsync(order.OrderNumber);
            _logger.LogInformation($"Delivery service response {JsonConvert.SerializeObject(response)}");
            if (!response.IsAllowed)
            {
                order.OrderStatus = response.CurrentDeliveryStatus;
                await _ordersRepo.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("ORder status synchronize with delivery service");
                throw new ApplicationException("Order is not in appropriate status for cancellation");
            }
            else
            {
                order.OrderStatus = OrderStatusTypes.Canceled.ToString();
                CancelOrderMessage message = new CancelOrderMessage()
                {
                     OrderNumber = order.OrderNumber,
                };
                await _outBoxRepo.AddAsync(new OutBoxMessage() { Message = JsonConvert.SerializeObject(message), Topic = KafkaTopics.CANCEL_ORDER });
                //Use outBox pattern for producing more reliable messages
               // await _kafkaHelperService.ProduceMessageAsync(message, KafkaTopics.CANCEL_ORDER);
                await _ordersRepo.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Successfully canceled");
            }
         
            return ApiResponse.Success();
        }
        public async Task<EFApiPaginationResponse<GetOrdersResponseDTO>> GetOrdersAsync(PaginationFilterModel paginationFilter)
        {
            _logger.LogInformation($"{nameof(GetOrdersAsync)} merhod called");
            var result = new EFApiPaginationResponse<GetOrdersResponseDTO>(100,paginationFilter);
            var orders = _ordersRepo.GetIQueryable().Include(m=>m.OrderItems).AsQueryable();
            if (!_currentUser.IsAdmin)
                orders = orders.Where(m => m.CustomerId == _currentUser.UserId);

            _logger.LogInformation($"{await orders.CountAsync()} orders exist for this filter");

            await result.SuccessAsync(orders, m => _mapper.Map<GetOrdersResponseDTO>(m));
            return result;

        }
        private decimal GetTotalInvoiceAmount(List<OrderItem> orderItems)
        {
            return orderItems.Sum(m => m.Price * m.Quantity);
        }
        private string GenerateOrderNumber()
        {
            var now = DateTime.Now;
            var ticks = now.Ticks;
            var random = new Random();
            var randomNumber = random.Next(0, 99999);

            var dataToHash = $"{ticks}-{randomNumber}";
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);

            return $"{now.ToString("yyyyMMddHHmmss")}-{hashString}";
        }
    }
}
