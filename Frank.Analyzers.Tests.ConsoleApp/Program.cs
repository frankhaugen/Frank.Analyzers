using Frank.Analyzers.Tests.ConsoleApp;

Console.WriteLine("Hello World!");

var countries = Countries.Instance;

foreach (var country in countries)
{
    Console.WriteLine(country.Name);
}