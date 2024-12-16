using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day05: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day05>(sample);
        
        CreateRules(file, out var rules);

        var sumMiddle = 0;
        string? line;
        while ((line = file.ReadLine()) is not null)
        {
             ReadOnlySpan<int> update = line.Split(',').Select(int.Parse).ToArray();
            
             if (IsValidUpdate(update, rules))
                 sumMiddle += update[update.Length/2];
            
        }
        return sumMiddle;
    }
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day05>(sample);
        CreateRules(file, out var rules);
        
        var sumMiddle = 0;
        string? line;
        while ((line = file.ReadLine()) is not null)
        {
            Span<int> update = line.Split(',').Select(int.Parse).ToArray();

            if (IsValidUpdate(update, rules)) continue;
            
            update.Sort((a, b) =>
                {
                    if (rules.ContainsKey(a) && rules[a].Contains(b)) return 1;
                    if (rules.ContainsKey(b) && rules[b].Contains(a)) return -1;
                    return 0;
                }
            );
            
            sumMiddle += update[update.Length/2];
            
        }
        return sumMiddle;
    }
    
    // Gets a dictionary containing the pages (value) that must go before another (key)
    private static void CreateRules(StreamReader input, out Dictionary<int, HashSet<int>> rules)
    {
        string? line;
        rules = new Dictionary<int, HashSet<int>>();
        while ((line = input.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            
            var pair = line.Split('|').Select(int.Parse).ToArray();
            if (!rules.ContainsKey(pair[1]))
            {
                rules.Add(pair[1], [pair[0]]);
            }
            else
            {
                rules[pair[1]].Add(pair[0]);
            }
        }
    }
    
    private static bool IsValidUpdate(in ReadOnlySpan<int> update, in Dictionary<int, HashSet<int>> rules)
    {
        bool isValidUpdate = true;
        for (var i = 0; isValidUpdate && i < update.Length; i++)
        {
            // Any of the pages after the current one must actually go before?
            for (var j = i + 1; j < update.Length; j++)
                if (rules.ContainsKey(update[i]) && rules[update[i]].Contains(update[j]))
                {
                    isValidUpdate = false;
                    break;
                } 
        }

        return isValidUpdate;
    }
}