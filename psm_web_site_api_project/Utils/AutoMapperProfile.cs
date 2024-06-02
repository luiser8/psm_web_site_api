using AutoMapper;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Utils.Mappers;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuarios, UsuariosResponseDto>();
            CreateMap<Usuarios, UsuariosPayloadPutDto>();
        }
    }