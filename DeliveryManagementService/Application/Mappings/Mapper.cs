using AutoMapper;
using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.DTOs.Response;
using DeliveryManagementService.Domain.Entities;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace UserManagementService.Application.Mappings
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AssignToCurierMessage, RegisterOrderForDeliveryRequestDTO>();
            CreateMap<CancelOrderMessage, CancelOrderRequestDTO>();
            CreateMap<RegisterOrderForDeliveryRequestDTO, Order>();
            CreateMap<Order, GetOrdersResponseModel>()
                .ForMember(m=>m.CurrentStatus,m=>m.MapFrom(x=>x.OrderStatus))
                .ForMember(m=>m.OrderStatusHistories,m=>m.MapFrom(x=>x.DeliveryHostories));
            CreateMap<DeliveryHostory, GetOrdersHistoryItemResponseModel>();
            CreateMap<DeliveryHostory, GetDeliveryHistoryOfOrderResponseDTO>();
        }
    }
}
