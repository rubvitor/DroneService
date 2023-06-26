using DroneService.Domain.Core;
using DroneService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DroneService.Test;

public class DomainFixture
{
    public DomainFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IDroneCombination, DroneCombination>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; private set; }
}
