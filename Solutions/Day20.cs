using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;
using Point = Util.Point;
using Direction = Util.Direction;

public class Day20: IDay
{
    private const char Wall = '#';
    private const char Track = '.';
    private const char Start = 'S';
    private const char End = 'E';

    private static readonly Direction[] AllowedDirections =
        [Direction.North, Direction.East, Direction.South, Direction.West];
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        var (width, height) = GetWalls(sample, out var walls, out var start, out var end);
        GetTimes(walls, start, end, out var times);

        var shortcuts = 0;

        var inWallX = width - 2;
        var inWallY = height - 2;
        var wallTimes = new List<int>(AllowedDirections.Length);
        foreach (var wall in walls)
        {
            // ignore if is outer wall
            if (!Util.IsValidPoint(wall, inWallX, inWallY, 1, 1)) continue;
        
            wallTimes.Clear();
            foreach (var dir in AllowedDirections)
            {
                var next = wall.MoveCopy(dir);
                if (walls.Contains(next)) continue;
                
                wallTimes.Add(times[next]);
            }
            
            for(var i = 0; i < wallTimes.Count-1; i++)
            for (var j = i + 1; j < wallTimes.Count; j++)
            {
                var save =int.Abs(wallTimes[i] - wallTimes[j]) - 2;
                if (save >= 100)
                    shortcuts++;
            }
        }
            
        return shortcuts;
    }
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        var minShortcut = sample ? 50 : 100;
        var maxDistance = 20;
        GetWalls(sample, out var walls, out var start, out var end);
        GetTimesList(walls, start, end, out var track);

        var shortcuts = 0;

        for(var i = 0; i < track.Count - 1; i++)
        for (var j = i + minShortcut + 2; j < track.Count; j++)
        {
            var save = j - i;
            var distance = track[i].ManhattanDistance(track[j]); 
            if (distance <= maxDistance && save - distance >= minShortcut)
                shortcuts++;
            
        }
        return shortcuts;
    }
    
    // Ended up being worse than original
    // [Benchmark, Arguments(false)]
    // public int Part2V2(bool sample = false)
    // {
    //     var minShortcut = sample ? 50 : 100;
    //     var maxDistance = 20;
    //     var (width, height) = GetWalls(sample, out var walls, out var start, out var end);
    //     GetTimes(walls, start, end, out var track);
    //     var totalTimes = 0;
    //     foreach (var (pos, time) in track)
    //     {
    //         // stay within the maze and skip maze border
    //         var yMaxOffset = int.Min(maxDistance, height-2 - pos.Y);
    //         var yMinOffset = -int.Min(maxDistance, int.Max(pos.Y-1, 0));
    //         for (var y = yMinOffset; y <= yMaxOffset; y++)
    //         {
    //             var yAbs = Math.Abs(y);
    //             // stay within the maze and skip maze border
    //             var xMaxOffset = int.Min(maxDistance - yAbs, width-2 - pos.X);
    //             var xMinOffset = -int.Min(maxDistance - yAbs, int.Max(pos.X-1, 0));
    //             for (var x = xMinOffset; x <= xMaxOffset ; x++)
    //             {
    //                 
    //                 var next = pos.OffsetCopy(x, y);
    //                 if (!track.TryGetValue(next, out var nextTime)) continue;
    //
    //                 var save = nextTime - time;
    //                 if (int.IsPositive(save) && save - next.ManhattanDistance(pos) >= minShortcut)
    //                     totalTimes++;
    //             }
    //         }
    //     }
    //
    //     return totalTimes;
    // }
    

    private void GetTimesList(in HashSet<Point> walls, in Point start, in Point end, out List<Point> track)
    {
        track = [start];
        
        var pos = start;
        var dir = Direction.North;
        while (pos != end)
        {
            var next = pos.MoveCopy(dir);

            if (walls.Contains(next))
            {
                dir = Util.RotateDirection(dir, Direction.East);
                next = pos.MoveCopy(dir);
                if (walls.Contains(next))
                {
                    dir = Util.RotateDirection(dir, Direction.South);
                    next = pos.MoveCopy(dir);
                }
            }
            pos = next;

            track.Add(pos);
        }
    }
    private void GetTimes(in HashSet<Point> walls, in Point start, in Point end, out Dictionary<Point, int> times)
    {
        times = new Dictionary<Point, int> {{start, 0}};
        var time = 0;
        var pos = start;
        var dir = Direction.North;
        while (pos != end)
        {
            
            var next = pos.MoveCopy(dir);

            if (walls.Contains(next))
            {
                dir = Util.RotateDirection(dir, Direction.East);
                next = pos.MoveCopy(dir);
                if (walls.Contains(next))
                {
                    dir = Util.RotateDirection(dir, Direction.South);
                    next = pos.MoveCopy(dir);
                }
            }
            pos = next;

            times.Add(pos, ++time);
            
        }
    }

    private Point GetWalls(bool sample, out HashSet<Point> walls, out Point start, out Point end)
    {
        walls = [];
        start = default;
        end = default;
        
        var current = new Point(-1, 0);
        foreach (var ch in Util.InputChars<Day20>(sample))
        {
            if (ch == Util.Newline)
            {
                current.Y++;
                current.X = -1;
                continue;
            }

            current.X++;
            
            switch (ch)
            {
                case Wall:
                    walls.Add(current);
                    break;
                case Track:
                    break;
                case Start:
                    start = current;
                    break;
                case End:
                    end = current;
                    break;
            }
        }
        // Dimensions of map
        current.Offset(1, 1);
        return current;
    }
}