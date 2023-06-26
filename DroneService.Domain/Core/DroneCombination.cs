using DroneService.Domain.Interfaces;
using DroneService.Domain.Models;

namespace DroneService.Domain.Core;

public class DroneCombination : IDroneCombination
{
    public void InitializeReading(string filePath)
    {
        var inputFileLocations = ReadInputFile(Path.Combine(filePath, "Input.txt"));

        List<string> result = new List<string>();

        List<Location> locationUnloaded = new List<Location>();
        locationUnloaded.AddRange(inputFileLocations.Value.Locations);

        var dronesLocationsFinal = new List<(Drone Drone, List<Location> Locations)>();

        while (locationUnloaded.Count > 0)
        {
            var maxCombinations = new List<(Drone Drone, List<Location> Locations)>();

            foreach (var locationDrone in inputFileLocations.Value.Drones)
            {
                var combinations = AllCombinations(locationUnloaded, locationDrone.MaxWeight);

                if (combinations.Count() == 0)
                    break;

                var maxCombination = combinations.OrderByDescending(x => x.Sum(s => s.Weight)).ThenByDescending(x => x.Count()).FirstOrDefault();

                maxCombinations.Add((locationDrone, maxCombination.ToList()));
            }

            var bestCombination = maxCombinations.OrderByDescending(x => x.Locations.Sum(l => l.Weight)).ThenByDescending(x => x.Locations.Count).FirstOrDefault();

            locationUnloaded = locationUnloaded.Except(bestCombination.Locations).ToList();

            dronesLocationsFinal.Add((bestCombination.Drone, bestCombination.Locations));
        }

        //Adding the Drones that not exist locations to fullfil the file pattern.
        foreach (var dronesExcept in inputFileLocations.Value.Drones.Except(dronesLocationsFinal.Select(x => x.Drone)))
            dronesLocationsFinal.Add((dronesExcept, null));

        foreach (var droneGroup in dronesLocationsFinal.GroupBy(x => x.Drone.Name).OrderBy(x => x.Key))
        {
            result.Add(droneGroup.Key);

            int pos = 1;

            foreach (var locations in droneGroup.Where(x => x.Locations is not null).Select(x => x.Locations))
            {
                result.Add($"Trip #{pos}");
                result.Add(string.Join(", ", locations.OrderBy(x => x.Name).Select(x => x.Name)));

                pos++;
            }
        }

        File.WriteAllLines(Path.Combine(filePath, "Output.txt"), result);
    }

    private IEnumerable<IEnumerable<Location>> Combinations(IEnumerable<Location> locations, double k, double max)
    {
        return k == 0 ? new[] { new Location[0] } :
          locations.SelectMany((e, i) =>
            Combinations(locations.Skip(i + 1), k - 1, max).Where(x => x.Sum(s => s.Weight) <= max).Select(c => (new[] { e }).Concat(c)));
    }

    private IEnumerable<IEnumerable<Location>> AllCombinations(IEnumerable<Location> locations, double max)
    {
        double length = locations.Count();
        for (double k = 1; k <= length; k++)
        {
            var comb = Combinations(locations, k, max);
            foreach (IEnumerable<Location> c in comb.Where(x => x.Sum(x => x.Weight) <= max))
                yield return c;
        }
    }

    private (List<Drone> Drones, List<Location> Locations)? ReadInputFile(string path)
    {
        var fileLines = File.ReadLines(path);
        if (fileLines.Count() <= 1)
            return null;
        List<Drone> drones = new List<Drone>();

        string dronesLine = fileLines.FirstOrDefault();
        var dronesLineSplitted = dronesLine.Split(',');
        for (int i = 0; i < dronesLineSplitted.Length; i += 2)
        {
            var drone = new Drone(dronesLineSplitted[i].Trim(), double.Parse(dronesLineSplitted[i + 1].Replace("[", "").Replace("]", "")));
            drones.Add(drone);
        }

        List<Location> locations = new List<Location>();

        foreach (var locationValue in fileLines.Skip(1))
        {
            var locationSplitted = locationValue.Split(',');

            var location = new Location(locationSplitted.First(), double.Parse(locationSplitted.LastOrDefault().Replace("[", "").Replace("]", "")));
            locations.Add(location);
        }

        return (drones, locations);
    }
}
