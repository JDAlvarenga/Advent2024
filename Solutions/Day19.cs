using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day19: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day19>(sample);
        
        var towels = file.ReadLine()!.Split(',', StringSplitOptions.TrimEntries).ToHashSet();
        
        // cache of attempted patterns
        Dictionary<string, bool> cacheDictionary = [];
        cacheDictionary.Add(string.Empty, true);
        var cache = cacheDictionary.GetAlternateLookup<ReadOnlySpan<char>>();

        file.ReadLine();

        var totalPossible = 0;
        while (!file.EndOfStream)
        {
            ReadOnlySpan<char> fullPattern = file.ReadLine()!;
            if (IsPossible(fullPattern, towels, ref cache))
                totalPossible++;
        }
        return totalPossible;
    }

    private static bool IsPossible(
        in ReadOnlySpan<char> pattern,
        in HashSet<string> towels,
        ref Dictionary<string, bool>.AlternateLookup<ReadOnlySpan<char>> cache)
    {
        if (cache.TryGetValue(pattern, out var cached)) return cached;
        
        foreach (ReadOnlySpan<char> towel in towels)
        {
            if (pattern.StartsWith(towel) && IsPossible(pattern[towel.Length..], towels, ref cache))
            {
                cache.TryAdd(pattern, true);
                return true;
            }
        } 
        
        cache.TryAdd(pattern, false);
        return false;
    }
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day19>(sample);
        
        var towels = file.ReadLine()!.Split(',', StringSplitOptions.TrimEntries).ToHashSet();
        
        // cache of attempted patterns
        Dictionary<string, long> cacheDictionary = [];
        cacheDictionary.Add(string.Empty, 1);
        var cache = cacheDictionary.GetAlternateLookup<ReadOnlySpan<char>>();

        file.ReadLine();

        long totalPossible = 0;
        while (!file.EndOfStream)
        {
            ReadOnlySpan<char> fullPattern = file.ReadLine()!;
            totalPossible += CountPossible(fullPattern, towels, ref cache);
        }
        return totalPossible;
    }
    
    private static long CountPossible(
        in ReadOnlySpan<char> pattern,
        in HashSet<string> towels,
        ref Dictionary<string, long>.AlternateLookup<ReadOnlySpan<char>> cache)
    {
        if (cache.TryGetValue(pattern, out var cached)) return cached;
        long totalPossible = 0;
        foreach (ReadOnlySpan<char> towel in towels)
        {
            if (pattern.StartsWith(towel))
            {
                totalPossible += CountPossible(pattern[towel.Length..], towels, ref cache);
            }
        }
        cache.TryAdd(pattern, totalPossible);
        return totalPossible;
    }
    
}