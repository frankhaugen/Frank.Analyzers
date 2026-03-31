using AutoMapper;
using Frank.Analyzers.Tests.ConsoleApp.Models;

namespace Frank.Analyzers.Tests.ConsoleApp.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Source, Destination>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description));
    }
}