using Frank.Analyzers.Tests.ConsoleApp;
using Frank.Analyzers.Tests.ConsoleApp.Services;

Console.WriteLine("Hello World!");

var countries = Countries.Instance;

foreach (var country in countries)
{
    Console.WriteLine(country.Name);
}

var myTestingService = new MyTestingService();

