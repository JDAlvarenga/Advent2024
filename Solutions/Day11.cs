using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day11: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        var file = Util.GetInputStream<Day11>(sample);
        var stones = file.ReadLine()!.Split().Select(long.Parse).ToList();

        Dictionary<Key, long> cache = [];
        return stones.Sum(stone => StoneCount(new Key(stone, 25), cache));
    }
    
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        var file = Util.GetInputStream<Day11>(sample);
        var stones = file.ReadLine()!.Split().Select(long.Parse).ToList();

        Dictionary<Key, long> cache = [];
        return stones.Sum(stone => StoneCount(new Key(stone, 75), cache));
    }

    private static long StoneCount(Key key, in Dictionary<Key, long> cache)
    {
        if (key.Blinks == 0) return 1;
        if (cache.TryGetValue(key, out var cached)) return cached;

        long totalCount = 0;

        if (key.Stone == 0)
        {
            totalCount = StoneCount(new Key(1, key.Blinks - 1), cache);
        }
        
        else if ((int)Math.Log10(key.Stone) + 1 is var digits && digits % 2 == 0)
        {
            var half = digits / 2;
            ReadOnlySpan<char> numStr = key.Stone.ToString();
            var left = long.Parse(numStr[..half]);
            var right = long.Parse(numStr[half..]);
            totalCount += StoneCount(new Key(left, key.Blinks - 1), cache);
            totalCount += StoneCount(new Key(right, key.Blinks - 1), cache);
        }
        else totalCount = StoneCount(new Key(key.Stone * 2024, key.Blinks - 1), cache);
            
        cache.Add(key, totalCount);
        return totalCount;
    }
    private record struct Key(long Stone, int Blinks);
}