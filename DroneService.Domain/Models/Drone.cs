namespace DroneService.Domain.Models;

public class Drone
{
    public Drone(string name, double maxWeight)
    {
        Name = name;
        MaxWeight = maxWeight;
    }
    public string Name { get; set; }
    public double MaxWeight { get; set; }
}

public class Location
{
    public Location(string name, double weight)
    {
        Name = name;
        Weight = weight;
    }
    public string Name;
    public double Weight { get; set; }
}
