using AutoMapper;
using AiTools.DAL.Entities;
using AiTools.Models.AccountModels;

namespace AiTools.Infrastructure.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
