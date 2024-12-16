using BenchmarkDotNet.Attributes;
using Point = Advent2024.Util.Point;
using Direction = Advent2024.Util.Direction;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day06: IDay
{
    private const char Empty = '.';
    private const char Obstacle = '#';
    private const char GuardN = '^';
    [Benchmark, Arguments(false)]
    public static int Part1(bool sample = false)
    {
        GetInput(out var map, out var guard, sample);
        var visited = new HashSet<Point>();
        var maxY = map.GetLength(0) - 1;
        var maxX = map.GetLength(1) - 1;

        guard.Map = pos => Util.IsValidPoint(pos, maxX, maxY) ? map[pos.Y, pos.X] : null;
        
        do
        {
            visited.Add(guard.Position);
            guard.Move();
            
        } while (guard.IsPatrolling);
        
        return visited.Count;
    }
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        GetInput(out var map, out var guard, sample);
        var visited = new Dictionary<Point, Direction>();
        var maxY = map.GetLength(0) - 1;
        var maxX = map.GetLength(1) - 1;
        var initial = guard with { };
        
        // function that masks
        guard.Map = char? (pos) => pos switch
        {
            _ when pos == initial.Position => Empty,
            _ when !Util.IsValidPoint(pos, maxX, maxY) => null,
            _ => map[pos.Y, pos.X]
        };
        
        var possibleObstacles = new Dictionary<Point, bool>();
        do
        {
            // Save visited tiles with direction
            if (visited.ContainsKey(guard.Position))
            {
                visited[guard.Position] |= guard.Direction;
            }
            else
            {
                visited.Add(guard.Position, guard.Direction);
            }


            var front = guard.FrontPosition;
            if (Util.IsValidPoint(front, maxX, maxY) && map[front.Y, front.X] == Empty)
            {
                var possibleGuard = guard;
                map[front.Y, front.X] = Obstacle;
                if (!possibleObstacles.ContainsKey(front))
                {
                    possibleGuard.TurnRight();
                    bool makesLoop = IsLoop(
                        visited.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                        possibleGuard);
                    possibleObstacles.Add(front, makesLoop);
                }

                map[front.Y, front.X] = Empty;
            }

            guard.Move();
        } while (guard.IsPatrolling);
        
        return possibleObstacles.Values.Count(loop => loop);
    }

    
    
    private static bool IsLoop(Dictionary<Point, Direction> visited, Guard guard)
    {
        
        while (guard.IsPatrolling)
        {
            if (visited.ContainsKey(guard.Position))
            {
                if (visited[guard.Position].HasFlag(guard.Direction)) return true;
                
                visited[guard.Position] |= guard.Direction;
            }
            else
            {
                visited.Add(guard.Position, guard.Direction);
            }
            
            guard.Move();
        }
        return false;
    }
    
    private record struct Guard(Point Position, Direction Direction, Func<Point, char?> Map)
    {
        public required Point Position = Position;
        public required Direction Direction = Direction;
        public required Func<Point, char?> Map = Map;
        
        public void Move()
        {
            Point front = FrontPosition;
            if (Map(front) is Obstacle) TurnRight();
            else Position.Move(Direction);
        }

        public Point FrontPosition => Position.MoveCopy(Direction);
        public void TurnRight() => Util.RotateDirection(ref Direction, Direction.East);
        public bool IsPatrolling => Map(Position) is not null;
    }

    private static void GetInput(out char[,] map, out Guard guard, bool sample = false)
    {
        using var file = Util.GetInputStream<Day06>(sample);
        
        // Read first line to get map size
        var line = file.ReadLine();
        if (line == null) throw new Exception("No input");
        
        // create map and load first row
        map = new char[line.Length, line.Length];
        for (var i = 0; i < line.Length; ++i)
        {
            map[0, i] = line[i];
        }
        
        // read file
        int row = 1, col = 0;
        guard = default;
        while (!file.EndOfStream)
        {
            var ch = (char)file.Read();

            if (ch is '\r' or '\n')
            {
                while (file.Peek() == '\r' || file.Peek() == '\n') file.Read();
                
                row++;
                col = 0;
                continue;
            }
            
            if (ch == GuardN)
            {
                guard = new Guard
                {
                    Position = new Point(col, row),
                    Direction = Direction.North,
                    Map = _ => null
                };
                ch = Empty;
            }
            
            map[row, col] = ch;
            col++;
        }
    }
}