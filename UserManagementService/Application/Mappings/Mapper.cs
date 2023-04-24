using AutoMapper;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.DTOs.Response;
using UserManagementService.Domain.Entities;

namespace UserManagementService.Application.Mappings
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SignUpRequestDTO, User>();
            CreateMap<CreateAccountRequestDTO, User>();
            CreateMap<User, GetCouriersResponseDTO>();
        }
    }
}
