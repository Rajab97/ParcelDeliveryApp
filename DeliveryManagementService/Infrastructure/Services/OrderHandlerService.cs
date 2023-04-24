using AutoMapper;
using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.DTOs.Response;
using DeliveryManagementService.Application.Services;
using DeliveryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Enums;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Commons;
using SharedLibrary.Models.Constants;

namespace DeliveryManagementService.Infrastructure.Services
{
    public class OrderHandlerService : IOrderHandlerService
    {
        private readonly ILogger<OrderHandlerService> _logger;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<DeliveryHostory> _deliveryHistoryRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderHandlerService(ILogger<OrderHandlerService> logger,
                                    IRepository<Order> orderRepo,
                                    IRepository<DeliveryHostory> deliveryHistoryRepo,
                                     IMapper mapper,
                                        IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _orderRepo = orderRepo;
            _deliveryHistoryRepo = deliveryHistoryRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<IsCancelationAllowedResponseDTO>> IsCancelationAllowedAsync(IsCancelationAllowedRequestDTO requestModel)
        {
            _logger.LogInformation($"{nameof(IsCancelationAllowedAsync)} method called");
            var order = await _orderRepo.GetIQueryable().FirstOrDefaultAsync(m => m.OrderNumber == requestModel.OrderNumber);
            if (order == null)
                throw new NotFoundException();

            if (OrderStatusFlow.IsTransitionAllowed(order.OrderStatus, OrderStatusTypes.Canceled.ToString()))
                return ApiResponse.Success(new IsCancelationAllowedResponseDTO() { IsAllowed = true,CurrentDeliveryStatus = order.OrderStatus });

            return ApiResponse.Success(new IsCancelationAllowedResponseDTO() { IsAllowed = false, CurrentDeliveryStatus = order.OrderStatus });
        }
        public async Task<ApiResponse> CancelOrderAsync(CancelOrderRequestDTO requestModel)
        {
            try
            {
                _logger.LogInformation($"{nameof(CancelOrderAsync)} method called");
                var order = await _orderRepo.GetIQueryable().FirstOrDefaultAsync(m => m.OrderNumber == requestModel.OrderNumber);
                if (order == null)
                    throw new NotFoundException();

                if (!OrderStatusFlow.IsTransitionAllowed(order.OrderStatus, OrderStatusTypes.Canceled.ToString()))
                    throw new ApplicationException($"Cancellation is impossible because the order is in {order.OrderStatus} status");

                order.OrderStatus = OrderStatusTypes.Canceled.ToString();
                var deliveryHistory = new DeliveryHostory()
                {
                    OrderId = order.Id,
                    EventTime = DateTime.Now,
                    Status = order.OrderStatus,
                };
                await _orderRepo.UpdateAsync(order);
                await _deliveryHistoryRepo.AddAsync(deliveryHistory);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Order cancelled succesfully");
                return ApiResponse.Success();
            }
            catch (ApplicationException ae)
            {
                _logger.LogError(ae, "Error occured while register order delivery");
                return ApiResponse.Error(ae.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, ExceptionMessages.ExceptionOccured);
                return ApiResponse.Error(ExceptionMessages.ExceptionOccured);
            }
        }
        public async Task<ApiResponse> RegisterOrderForDeliveryAsync(RegisterOrderForDeliveryRequestDTO requestModel)
        {
            try
            {
                _logger.LogInformation($"{nameof(RegisterOrderForDeliveryAsync)} method called");
                //This check applied to make service idempotent. If order number will be duplicated it will be added only once
                var orderWithSameOrderNumber = await _orderRepo.GetIQueryable().FirstOrDefaultAsync(m => m.OrderNumber == requestModel.OrderNumber);
                if (orderWithSameOrderNumber != null)
                    return ApiResponse.Error($"Order with same order number already exist in database.OrderNumber:{requestModel.OrderNumber},CreatedDate:{orderWithSameOrderNumber.CreatedAt}");

                var model = _mapper.Map<Order>(requestModel);
                model.OrderStatus = OrderStatusTypes.InProgress.ToString();
                var deliveryHistory = new DeliveryHostory()
                {
                    OrderId = requestModel.OrderId,
                    EventTime = DateTime.Now,
                    Status = OrderStatusTypes.InProgress.ToString()
                };
                model.DeliveryHostories = new List<DeliveryHostory>() { deliveryHistory };
                await _orderRepo.AddAsync(model);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Order registered for delivery successfuully");
                return ApiResponse.Success();
            }
            catch (ApplicationException ae)
            {
                _logger.LogError(ae,"Error occured while register order delivery");
                return ApiResponse.Error(ae.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e,ExceptionMessages.ExceptionOccured);
                return ApiResponse.Error(ExceptionMessages.ExceptionOccured);
            }
        }

       
        
    }
}
