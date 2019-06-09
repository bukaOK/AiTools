using AiTools.DAL.Entities;
using AiTools.Models.EmployeeModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.BLL.MapProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, EmployeeModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.SirName, opt => opt.MapFrom(src => src.SirName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName));
        }
    }
}
