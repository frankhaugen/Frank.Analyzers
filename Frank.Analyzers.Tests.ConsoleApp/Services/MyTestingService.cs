using AutoMapper;
using Frank.Analyzers.Tests.ConsoleApp.Models;

namespace Frank.Analyzers.Tests.ConsoleApp.Services;

public class MyTestingService
{
    public void DoSomething()
    {
        var source = new Source
        {
            Name = "Frank",
            Description = "Description",
            Age = 30,
            Address = "Address"
        };
        
        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Profiles.MappingProfile>();
        }).CreateMapper();
        
        var destination = mapper.Map<Destination>(source);
    }
}