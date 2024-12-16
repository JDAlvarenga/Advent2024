using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day01: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        string? line = null;
        var list1 = new List<int>();
        var list2 = new List<int>();
        
        using var file = Util.GetInputStream<Day01>(sample);
        while ((line = file.ReadLine()) is not null)
        {
             var pairs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
             list1.Add(pairs[0]);
             list2.Add(pairs[1]);
        }
        
        list1.Sort();
        list2.Sort();
        
        return list1.Zip(list2, (id1, id2) => int.Abs(id1 - id2)).Sum();
    }
    
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        string? line = null;
        
        var frecuencies1 = new Dictionary<int, int>();
        var list2 = new List<int>();
        
        using var file = Util.GetInputStream<Day01>(sample);
        while ((line = file.ReadLine()) is not null)
        {
             var pairs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
             
             if (!frecuencies1.TryAdd(pairs[0], 1))
                 frecuencies1[pairs[0]]++;
             
             list2.Add(pairs[1]);
        }

        var frecuencies2 = frecuencies1.ToDictionary(kvp => kvp.Key, _ => 0);
        
        foreach (var id in list2)
        {
            if (frecuencies2.ContainsKey(id))
                frecuencies2[id]++;
        }

        return frecuencies1.Keys.Select(id => id * frecuencies1[id] * frecuencies2[id]).Sum();
    }
}