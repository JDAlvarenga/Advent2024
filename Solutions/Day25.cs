using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

public class Day25: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        GetInput(sample, out var locks, out var keys);
        int fits = 0;
        foreach (var l in locks)
        foreach (var k in keys)
        {
            if (l.H1 + k.H1 <= 5 &&
                l.H2 + k.H2 <= 5 &&
                l.H3 + k.H3 <= 5 &&
                l.H4 + k.H4 <= 5 &&
                l.H5 + k.H5 <= 5)
                fits++;
        }

        return fits;
    }

    private static void GetInput(bool sample, out HashSet<Lock> locks, out HashSet<Key> keys)
    {
        locks = [];
        keys = [];
        using var file = Util.GetInputStream<Day25>(sample);
        
        while (!file.EndOfStream)
        {
            
            var typeLine = file.ReadLine();
            if (typeLine.StartsWith('#')) // lock
            {
                int h1 = 0, h2 = 0, h3 = 0, h4 = 0, h5 = 0;
                while (file.ReadLine() is { } heights && !string.IsNullOrEmpty(heights))
                {
                    h1 += heights[0] == '#' ? 1 : 0;
                    h2 += heights[1] == '#' ? 1 : 0;
                    h3 += heights[2] == '#' ? 1 : 0;
                    h4 += heights[3] == '#' ? 1 : 0;
                    h5 += heights[4] == '#' ? 1 : 0;
                }
                locks.Add(new Lock(h1, h2, h3, h4, h5));
            }
            else
            {
                int h1 = -1, h2 = -1, h3 = -1, h4 = -1, h5 = -1;
                while (file.ReadLine() is { } heights && !string.IsNullOrEmpty(heights))
                {
                    h1 += heights[0] == '#' ? 1 : 0;
                    h2 += heights[1] == '#' ? 1 : 0;
                    h3 += heights[2] == '#' ? 1 : 0;
                    h4 += heights[3] == '#' ? 1 : 0;
                    h5 += heights[4] == '#' ? 1 : 0;
                }
                keys.Add(new Key(h1, h2, h3, h4, h5));
            }
        }
    }
    private record struct Lock(int H1, int H2, int H3, int H4, int H5);
    private record struct Key(int H1, int H2, int H3, int H4, int H5);
}