using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;
using Point = Util.Point;
using Direction = Util.Direction;

[ShortRunJob]
public class Day18: IDay
{
    private const int SampleSize = 7;
    private const int Size = 71;
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var size = sample ? SampleSize : Size;
        int wallOrder = 0;
        var walls = GetBytes(sample, sample ? 12 : 1024 ).ToDictionary(p => p, _ => wallOrder++);
        var end = new Point(size-1, size-1);
        Dijkstra(walls, size, Point.Empty, end, out var steps);
        
        return steps[end];
    }
    [Benchmark, Arguments(false)]
    public string Part2(bool sample = false)
    {
        var size = sample ? SampleSize : Size;
        var i = 0;
        var walls = GetBytes(sample).ToDictionary(p => p, _ => i++);
        var end = new Point(size-1, size-1);

        var possible = 0;
        var notPossible = walls.Count;

        while (possible + 1 != notPossible)
        {
            int half = possible + (notPossible - possible) / 2;
            Dijkstra(walls, size, Point.Empty, end, out var steps, half);
            if (steps.ContainsKey(end))
            {
                possible = half;
            }
            else notPossible = half;
            
        }

        var cutoff = walls.First(kvp => kvp.Value == notPossible).Key;
        return $"{cutoff.X},{cutoff.Y}";
    }

    private static readonly Direction[] AllowedDirections =
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];
    
    private static void Dijkstra(in Dictionary<Point, int> walls, in int size, in Point start, in Point end, out Dictionary<Point, int> stepsFromStart, in int wallLimit = int.MaxValue)
    {
        stepsFromStart = [];
        var queue = new PriorityQueue<Point, int>();
        queue.Enqueue(start, 0);
        stepsFromStart.Add(start, 0);

        while (queue.TryDequeue(out var pos, out var steps))
        {
            if (pos == end) continue;

            foreach (var direction in AllowedDirections)
            {
                var nextPos = pos.MoveCopy(direction);
                if (walls.TryGetValue(nextPos, out int wallOrder) && wallOrder <= wallLimit) continue;
                if (!Util.IsValidPoint(nextPos, size - 1, size - 1)) continue;
                
                if (stepsFromStart.TryAdd(nextPos, steps + 1))
                {
                    queue.Enqueue(nextPos, steps + 1);
                }
                else if (steps + 1 < stepsFromStart[nextPos])
                {
                    stepsFromStart[nextPos] = steps + 1;
                    queue.Remove(nextPos, out _, out _);
                    queue.Enqueue(nextPos, steps + 1);
                }
                
            }
        }
    }

    private IEnumerable<Point> GetBytes(bool sample, int count = int.MaxValue)
    {
        using var file = Util.GetInputStream<Day18>(sample);
        while (count-- > 0 && !file.EndOfStream)
        {
            var coors = file.ReadLine()!.Split(',').Select(int.Parse).ToArray();
            yield return new Point(coors[0], coors[1]);
        }
    }
}