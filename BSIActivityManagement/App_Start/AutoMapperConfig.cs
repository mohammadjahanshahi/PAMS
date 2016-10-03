using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace BSIActivityManagement
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<DAL.AMOrganization, ViewModel.JsonOrganizationViewModel>();
                x.CreateMap<DAL.AMProcess, ViewModel.JsonProcessViewModel>().ForMember(m => m.ImageId, opt => opt.MapFrom(src => src.ProcessType.ImageId));
            });
        }
    }
}
