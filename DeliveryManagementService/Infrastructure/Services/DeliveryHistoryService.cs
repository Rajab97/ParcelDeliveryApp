using AutoMapper;
using DeliveryManagementService.Application.Common.Models;
using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.DTOs.Response;
using DeliveryManagementService.Application.Services;
using DeliveryManagementService.Application.Services.Common;
using DeliveryManagementService.Domain.Entities;
using DeliveryManagementService.Helpers.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Enums;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Commons;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace DeliveryManagementService.Infrastructure.Services
{
    public class DeliveryHistoryService : IDeliveryHistoryService
    {
        private readonly ILogger<DeliveryHistoryService> _logger;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<DeliveryHostory> _deliveryHistoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaHelperService _kafkaHelperService;
        private readonly CurrentUser _currentUser;
        private readonly IMapper _mapper;

        public DeliveryHistoryService(ILogger<DeliveryHistoryService> logger,
                                    IRepository<Order> orderRepo,
                                    IRepository<DeliveryHostory> deliveryHistoryRepo,
                                    IUnitOfWork unitOfWork,
                                    IKafkaHelperService kafkaHelperService,
                                    CurrentUser currentUser,
                                    IMapper mapper)
        {
            _logger = logger;
            _orderRepo = orderRepo;
            _deliveryHistoryRepo = deliveryHistoryRepo;
            _unitOfWork = unitOfWork;
            _kafkaHelperService = kafkaHelperService;
            _currentUser = currentUser;
            _mapper = mapper;
        }
        public async Task<EFApiPaginationResponse<GetOrdersResponseModel>> GetOrdersAsync(PaginationFilterModel paginationFilterModel)
        {
            _logger.LogInformation($"{nameof(GetOrdersAsync)} method called");
            EFApiPaginationResponse<GetOrdersResponseModel> response = new EFApiPaginationResponse<GetOrdersResponseModel>(100,paginationFilterModel);
            var orders = _orderRepo.GetIQueryable().Include(m => m.DeliveryHostories.OrderBy(x=>x.EventTime)).AsQueryable();

            //This logic used for couriers can see only orders assigned to them and users view only order created by them
            if (_currentUser.IsCourier)
                orders = orders.Where(m => m.CourierId == _currentUser.UserId);
            else if(_currentUser.IsUser)
                orders = orders.Where(m => m.CustomerId == _currentUser.UserId);

            _logger.LogInformation($"{await orders.CountAsync()} orders retrived");
            await response.SuccessAsync(orders,m=>_mapper.Map<GetOrdersResponseModel>(m));
            return response;
        }

        public async Task<ApiResponse<IEnumerable<GetDeliveryHistoryOfOrderResponseDTO>>> GetDeliveryHistoryOfOrderAsync(GetDeliveryHistoryOfOrderRequestDTO requestModel)
        {
            _logger.LogInformation($"{nameof(GetDeliveryHistoryOfOrderAsync)} method called");
            var order = await _orderRepo.GetIQueryable().Include(m => m.DeliveryHostories.OrderBy(x => x.EventTime)).FirstOrDefaultAsync(m=>m.OrderNumber == requestModel.OrderNumber);
            if (order == null)
                throw new NotFoundException();

            //This logic used for couriers can see only orders assigned to them and users view only order created by them
            if (_currentUser.IsCourier && order.CourierId != _currentUser.UserId)
                throw new ApplicationException("You can view only order that assigned to you");
            else if (_currentUser.IsUser && order.CustomerId != _currentUser.UserId)
                throw new ApplicationException("You can view only order that created by you");

            _logger.LogInformation($"Order retrived succesfully");
            var result = _mapper.Map<IEnumerable<GetDeliveryHistoryOfOrderResponseDTO>>(order.DeliveryHostories);
            return ApiResponse.Success(result);
        }
        public async Task<ApiResponse> ChangeOrderStatusAsync(ChangeOrderStatusRequestDTO requestModel)
        {
            _logger.LogInformation($"{nameof(ChangeOrderStatusAsync)} method called");
            var order = await _orderRepo.GetIQueryable().FirstOrDefaultAsync(m=>m.Id == requestModel.Id);
            if (order == null)
                throw new NotFoundException();

            if (!OrderStatusFlow.IsTransitionAllowed(order.OrderStatus,requestModel.OrderStatus))
                throw new ApplicationException($"Transtion doesn't exist between {order.OrderStatus} and {requestModel.OrderStatus}");

            order.OrderStatus = requestModel.OrderStatus;

            var newDeliveryHistory = new DeliveryHostory()
            {
                OrderId = order.Id,
                Status = order.OrderStatus,
                EventTime = DateTime.Now
            };
            await _orderRepo.UpdateAsync(order);
            await _deliveryHistoryRepo.AddAsync(newDeliveryHistory);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Order status changed to {requestModel.OrderStatus} status.OrderNumber: {order.OrderNumber}");

            var kafkaMessage = new OrderStatusChangedMessage()
            {
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                StatusChangeDate = DateTime.Now
            };
            await _kafkaHelperService.ProduceMessageAsync(kafkaMessage,KafkaTopics.ORDER_STATUS_CHANGED);
            return ApiResponse.Success();
        }
    }
}
