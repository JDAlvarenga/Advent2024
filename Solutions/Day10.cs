using System.Collections;
using Point = Advent2024.Util.Point;
namespace Advent2024.Solutions;

public class Day10
{
    public static int Part1(bool sample = false)
    {
        Dictionary<Point, byte> map = new();
        List<Point> peaks = [];
        List<Point> trailHeads = [];
        Point current = new(-1, 0);
        foreach (var ch in Util.InputChars<Day10>(sample))
        {
            if (ch == Util.Newline)
            {
                current.X = -1;
                current.Y++;
                continue;
            }

            current.X++;
            var height = (byte)(ch - '0');
            map.Add(current, height);
            
            if (height == 9) peaks.Add(current);
            else if (height == 0) trailHeads.Add(current);
        }
        // Each represents all the points in the map that can reach a certain peak
        List<BitArray> reachablePeaks = new(peaks.Count);


        var widthOfMap = current.X + 1; 
        foreach (var peak in peaks)
        {
            Descend(peak, map, widthOfMap, out var peakMap);
            reachablePeaks.Add(peakMap);
        }

        var trailScores = 0;
        foreach (var trailhead in trailHeads)
        {
            trailScores += reachablePeaks.Count(peakMap => peakMap.IsVisited(widthOfMap, trailhead));
        }
        
        return trailScores;
    }
    
    private static void Descend(Point peak, in Dictionary<Point, byte> map, in int width, out BitArray visited)
    {
        visited = new BitArray(map.Count);

        Stack<Point> alternative = [];
        alternative.Push(peak);

        while (alternative.TryPop(out var position))
        {
            var height = map[position];
            while (true)
            {
                if (visited.IsVisited(width, position))
                    break;
                
                visited.SetVisited(width, position);
                
                
                if (height == 0) break;
                
                Util.Direction? next = null;
                
                foreach (var direction in AllowedDirections)
                {
                    var nextPosition = position.MoveCopy(direction);
                    if (!map.TryGetValue(nextPosition, out var h) || h != height - 1) continue;
                    
                    if (next is null)
                        next = direction;
                    else
                        alternative.Push(nextPosition);
                }

                if (next is null) break;
                
                height--;
                position.Move(next.Value);
            }
            
        }
    }
    
    public static int Part2(bool sample = false)
    {
        Dictionary<Point, byte> map = new();
        List<Point> peaks = [];
        List<Point> trailHeads = [];
        Point current = new(-1, 0);
        foreach (var ch in Util.InputChars<Day10>(sample))
        {
            if (ch == Util.Newline)
            {
                current.X = -1;
                current.Y++;
                continue;
            }

            current.X++;
            var height = (byte)(ch - '0');
            map.Add(current, height);
            
            if (height == 9) peaks.Add(current);
            else if (height == 0) trailHeads.Add(current);
        }
        
        // Dictionary<Point, int> scores = new(map.Count);
        int[,] scores = new int[current.Y + 1, current.X + 1];

        foreach (var peak in peaks)
            Descend2(peak, map, ref scores);
        
        return trailHeads.Sum(trailHead => scores[trailHead.Y, trailHead.X]);

    }

    private static readonly Util.Direction[] AllowedDirections = [
        Util.Direction.North,
        Util.Direction.South,
        Util.Direction.East,
        Util.Direction.West
    ];
    private static void Descend2(Point peak, in Dictionary<Point, byte> map, ref int[,] scores)
    {
        Stack<Point> alternative = [];
        alternative.Push(peak);
        
        while (alternative.TryPop(out var position))
        {
            var height = map[position];
            while (true)
            {
                scores[position.Y, position.X]++;
                
                if (height == 0) break;
                
                Util.Direction? next = null;

                foreach (var direction in AllowedDirections)
                {
                    var nextPosition = position.MoveCopy(direction);
                    if (!map.TryGetValue(nextPosition, out var h) || h != height - 1) continue;
                    
                    if (next is null)
                        next = direction;
                    else
                        alternative.Push(nextPosition);
                }

                if (next is null) break;
                
                height--;
                position.Move(next.Value);
            }
            
        }
    }
    
}

public static class HikingExtensions
{
    public static void SetVisited(this BitArray visited, in int width, in Point position)
    {
        visited[position.X + position.Y * width] = true;
    }

    public static bool IsVisited(this BitArray visited, in int width, in Point position) =>
        visited[position.X + position.Y * width];
}
