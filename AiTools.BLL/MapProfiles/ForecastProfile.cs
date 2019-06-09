using AiTools.DAL.Entities;
using AiTools.Models.ForecastModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.BLL.MapProfiles
{
    public class ForecastProfile : Profile
    {
        public ForecastProfile()
        {
            CreateMap<ForecastResult, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            CreateMap<ForecastResult, ForecastEditModel>()
                .ForMember(dest => dest.Delimiter, opt => opt.Ignore())
                .ForMember(dest => dest.File, opt => opt.Ignore());
        }
    }
}
