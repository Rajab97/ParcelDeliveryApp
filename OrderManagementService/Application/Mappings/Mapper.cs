using AutoMapper;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.DTOs.Response;
using OrderManagementService.Domain.Entities;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace UserManagementService.Application.Mappings
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CreateOrderRequestDTO, Order>();
            CreateMap<ChangeOrderAddressRequestDTO, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateOrderItemRequestDTO, OrderItem>();
            CreateMap<Order, CreateOrderResponseDTO>();
            CreateMap<OrderItem, CreateOrderItemResponseDTO>();
            CreateMap<OrderStatusChangedMessage, ChangeOrderStatusRequestDTO>();
            CreateMap<Order, GetOrdersResponseDTO>();
            CreateMap<OrderItem, GetOrdersItemResponseDTO>();
        }
    }
}
