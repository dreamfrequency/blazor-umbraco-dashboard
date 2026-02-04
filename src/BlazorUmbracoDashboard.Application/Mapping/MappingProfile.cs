using AutoMapper;
using BlazorUmbracoDashboard.Application.DTOs;
using BlazorUmbracoDashboard.Domain.Entities;

namespace BlazorUmbracoDashboard.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ContentNode, ContentNodeDto>().ReverseMap();
        CreateMap<AuditLog, AuditLogDto>().ReverseMap();
    }
}
