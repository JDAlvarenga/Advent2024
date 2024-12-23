using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

public class Day23: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day23>(sample);
        Dictionary<string, HashSet<string>> connections = [];
        var totalTCliques = 0;
        while (!file.EndOfStream)
        {
            var line = file.ReadLine()!.Split('-');
            var commonNodes = connections.Where(node => node.Value.Contains(line[0]) && node.Value.Contains(line[1]));
            foreach (var node in commonNodes)
            {
                if (node.Key.StartsWith('t') || line[0].StartsWith('t') || line[1].StartsWith('t'))
                {
                    totalTCliques++;
                }
            }
            
            if (!connections.TryAdd(line[0], [line[1]]))
                connections[line[0]].Add(line[1]);
            
            if (!connections.TryAdd(line[1], [line[0]]))
                connections[line[1]].Add(line[0]);
        }
        return totalTCliques;
    }
    [Benchmark, Arguments(false)]
    public string Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day23>(sample);
        Dictionary<string, HashSet<string>> connections = [];
        while (!file.EndOfStream)
        {
            var line = file.ReadLine()!.Split('-');
            
            if (!connections.TryAdd(line[0], [line[1]]))
                connections[line[0]].Add(line[1]);
            
            if (!connections.TryAdd(line[1], [line[0]]))
                connections[line[1]].Add(line[0]);
        }

        var clique = MaxClique(connections, [], connections.Keys.ToHashSet(), []);

        return string.Join(',', clique.Order());
    }

    private static HashSet<string> MaxClique(in Dictionary<string, HashSet<string>> connections, HashSet<string> clique, HashSet<string> possibleNodes, HashSet<string> excludedNodes)
    {
        if (possibleNodes.Count == 0 && excludedNodes.Count == 0) return clique;
        
        Queue<string> queue = new(possibleNodes);
        HashSet<string> maxClique = [];
        while (queue.TryDequeue(out var node))
        {
            clique.Add(node);
            var r = MaxClique(connections, 
                clique, 
                possibleNodes.Intersect(connections[node]).ToHashSet(),
                excludedNodes.Intersect(connections[node]).ToHashSet());
            
            if (r.Count > maxClique.Count)
                maxClique = r.ToHashSet();
            
            clique.Remove(node);
            
            possibleNodes.Remove(node);
            excludedNodes.Add(node);
        }

        return maxClique;
    }
    
}