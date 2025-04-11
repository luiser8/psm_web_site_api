using AutoMapper;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Utils.Mappers;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuariosResponse>();
            CreateMap<Usuario, UsuariosPayloadPut>();
            CreateMap<Header, HeaderResponse>()
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo != null ? Convert.FromBase64String(src.Logo) : Array.Empty<byte>()))
                .ForMember(dest => dest.HeaderCollections, opt => opt.MapFrom(src => src.HeaderCollections))
                .ForMember(dest => dest.HeaderExtensions, opt => opt.MapFrom(src => src.HeaderExtensions));
            CreateMap<HeaderPayload, Header>();
        }
    }