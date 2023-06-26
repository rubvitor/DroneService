using DroneService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DroneService.Test;

public class DroneOutputTest : IClassFixture<DomainFixture>
{
    private ServiceProvider _serviceProvider;

    public DroneOutputTest(DomainFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    [Fact]
    public void TestIfHasValidOutput()
    {
        var droneCombination = _serviceProvider.GetService<IDroneCombination>();

        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var outputPath = Path.Combine(rootPath, "Output.txt");
        var outputExpectedPath = Path.Combine(rootPath, "OutputComparer.txt");

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        droneCombination.InitializeReading(rootPath);

        Assert.True(File.Exists(outputPath));

        var outputFile = File.ReadAllText(outputPath).Trim().ToUpper();
        var outPutExpectedFile = File.ReadAllText(outputExpectedPath).Trim().ToUpper();

        Assert.True(outputFile.Equals(outPutExpectedFile));
    }
}