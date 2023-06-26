using DroneService.Domain.Core;
using DroneService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IDroneCombination, DroneCombination>();
    })
    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;
var droneCombination = provider.GetRequiredService<IDroneCombination>();

Console.WriteLine("Please enter the entire File Path of Input and Output: ");
string filePath = Console.ReadLine();

droneCombination.InitializeReading(filePath);