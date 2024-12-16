using BenchmarkDotNet.Attributes;
using Point = Advent2024.Util.Point;
using Direction = Advent2024.Util.Direction;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day12: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false) => GetCost(CostBase, sample);
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false) => GetCost(CostDiscount, sample);
    
    

    private delegate int CostDelegate(in Point start, in Dictionary<Point, char> map, ref HashSet<Point> visited);
    private static int GetCost(CostDelegate costFunction, bool sample = false)
    {
        Dictionary<Point, char> map = new();
        Point current = new(-1, 0);
        foreach (var ch in Util.InputChars<Day12>(sample))
        {
            if (ch == Util.Newline)
            {
                current.Y++;
                current.X = -1;
                continue;
            }
            
            current.X++;
            map.Add(current, ch);
        }
        
        HashSet<Point> visited = new(map.Count);
        current = Point.Empty;
        var totalCost = 0;
        do
        {
            totalCost += costFunction(current, map, ref visited);
            current = map.Keys.FirstOrDefault( point => !visited.Contains(point));
        }
        while(current != Point.Empty);
        
        return totalCost;
    }

    private static readonly Direction[] AllowedDirections =
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];
    private static int CostBase(in Point start, in Dictionary<Point, char> map, ref HashSet<Point> visited)
    {
        var area = 0;
        var perimeter = 0;
        
        Queue<Point> queue = new();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.TryDequeue(out var current))
        {
            area++;
            
            foreach (var direction in AllowedDirections)
            {
                var nextPoint = current.MoveCopy(direction);
                if (map.TryGetValue(nextPoint, out var value))
                {
                    if (value != map[current]) perimeter++;
                    else if(visited.Add(nextPoint)) queue.Enqueue(nextPoint);
                    
                }
                else perimeter++;

            }
            
        }
        return area * perimeter;
    }
    
    private static int CostDiscount(in Point start, in Dictionary<Point, char> map, ref HashSet<Point> visited)
    {
        var area = 0;
        var sides = 0;
        
        Queue<Point> queue = new();
        queue.Enqueue(start);
        var ch = map[start];
        visited.Add(start);
        while (queue.TryDequeue(out var current))
        {
            area++;
            
            var northPoint = current.MoveCopy(Direction.North);
            var eastPoint = current.MoveCopy(Direction.East);
            var southPoint = current.MoveCopy(Direction.South);
            var westPoint = current.MoveCopy(Direction.West);
            map.TryGetValue(northPoint, out var north);
            map.TryGetValue(eastPoint, out var east);
            map.TryGetValue(southPoint, out var south);
            map.TryGetValue(westPoint, out var west);
            
            
            
            // northEast
            if (ch != north && ch != east) sides++; // is outer corner
            else
            {
                map.TryGetValue(current.MoveCopy(Direction.NorthEast), out var northEast);
                if (ch == north && ch == east && ch != northEast) sides++; // is inner corner
            }
            
            // southEast
            if (ch != south && ch != east) sides++; // is outer corner
            else
            {
                map.TryGetValue(current.MoveCopy(Direction.SouthEast), out var southEast);
                if (ch == south && ch == east && ch != southEast) sides++; // is inner corner
            }
            
             // southWest
             if (ch != south && ch != west) sides++; // is outer corner
             else
             {
                 map.TryGetValue(current.MoveCopy(Direction.SouthWest), out var southWest); 
                if (ch == south && ch == west && ch != southWest) sides++; // is inner corner
             }
             
             // northWest
             if (ch != north && ch != west) sides++; // is outer corner
             else
             {
                 map.TryGetValue(current.MoveCopy(Direction.NorthWest), out var northWest); 
                 if (ch == north && ch == west && ch != northWest) sides++; // is inner corner
             }
             
             
             //move if part of garden AND not visited before or already enqueued
             if (ch == north && visited.Add(northPoint)) queue.Enqueue(northPoint);
             if (ch == east && visited.Add(eastPoint)) queue.Enqueue(eastPoint);
             if (ch == south && visited.Add(southPoint)) queue.Enqueue(southPoint);
             if (ch == west && visited.Add(westPoint)) queue.Enqueue(westPoint);
            
            
        }
        return area * sides;
    }
}