using System.Text;

namespace Advent2024.Solutions;
using Point = Util.Point;
using Direction = Util.Direction;

public class Day15
{
    private const char Empty = '.';  
    private const char Wall = '#';  
    private const char Box = 'O';
    private const char Bot = '@';
    
    public static int Part1(bool sample = false)
    {
        // get dimensions if printing the warehouse
        // var (width, height) = GetWarehouse(sample, out var bot, out var walls, out var boxes, out var moves);
        GetWarehouse(sample, out var bot, out var walls, out var boxes, out var moves);
        foreach (var move in moves)
        {
            
            var boxCount = 0;
            var next = bot.MoveCopy(move);
            while (boxes.Contains(next))
            {
                boxCount++;
                next.Move(move);
            }
            if (walls.Contains(next)) continue;

            bot.Move(move);
            
            if (boxCount <= 0) continue;
            boxes.Remove(bot);
            boxes.Add(next);
        }
        // ShowWarehouse(bot, walls, boxes, width, height);
        
        return boxes.Sum(box => box.Y * 100 + box.X);
    }
    
    public static int Part2(bool sample = false)
    {
        // get dimensions if printing the warehouse
        // var (width, height) = GetWarehouse2(sample, out var bot, out var walls, out var boxes, out var moves);
        GetWarehouse2(sample, out var bot, out var walls, out var boxes, out var moves);
        
        // var step = 0;
        foreach (var move in moves)
        {
            // show step by step
            // ShowWarehouse2(bot, walls, boxes, width, height);
            // step++;
            // Console.WriteLine($"Step {step} {move}");
            var next = bot.MoveCopy(move);
            if (walls.Contains(next)) continue;
            
            Stack<Point> toMove = [];
            
            

            if (move is Direction.North or Direction.South)
            {
                Stack<Point> pushing = [];
                
                if (boxes.Contains(next)) pushing.Push(next);
                else
                {
                    var northWest = next.MoveCopy(Direction.West);
                    if (boxes.Contains(northWest)) pushing.Push(northWest);
                }

                while (pushing.TryPop(out Point b))
                {
                    var centered = b.MoveCopy(move);
                    var left = centered.MoveCopy(Direction.West);
                    var right = centered.MoveCopy(Direction.East);
                    if (walls.Contains(centered))
                    {
                        next = centered;
                        break;
                    }
                    if (walls.Contains(right))
                    {
                        next = right;
                        break;
                    }
                    
                    toMove.Push(b);
                    
                    if (boxes.Contains(centered)) pushing.Push(centered);
                    else
                    {
                        if (boxes.Contains(left)) pushing.Push(left);
                        if (boxes.Contains(right)) pushing.Push(right);
                    }
                    
                }
            }
            else
            {
                // position of possible boxes
                if (move == Direction.West) next.Move(move);
                while (boxes.Contains(next))
                {
                    toMove.Push(next);
                    next.Move(move, 2);
                }
                if (move == Direction.West) next.Move(Direction.East);
            }


            if (walls.Contains(next)) continue;
            
            while (toMove.TryPop(out var b))
            {
                boxes.Remove(b);
                boxes.Add(b.MoveCopy(move));
            }
            
            bot.Move(move);
        }
        // show final state
        // ShowWarehouse2(bot, walls, boxes, width,height);
        
        return boxes.Sum(box => box.Y * 100 + box.X);
    }
    
    
    // ReSharper disable once UnusedMember.Local
    private static void ShowWarehouse(in Point bot, in HashSet<Point> walls, in HashSet<Point> boxes, in int width, in int height)
    {
        StringBuilder builder = new();
        var current = new Point(-1, 0);
        
        
        while (current.Y < height)
        {
            current.X++;
            if (current.X == width)
            {
                current.Y++;
                current.X = -1;
                builder.AppendLine();
                continue;
            }
            
            if (walls.Contains(current))
                builder.Append('#');
            else if (boxes.Contains(current))
                builder.Append('O');
            else if (current == bot)
                builder.Append('@');
            else
                builder.Append('.');

        }
        Console.WriteLine(builder);
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static (int width, int height) GetWarehouse(bool sample, out Point bot, out HashSet<Point> walls, out HashSet<Point> boxes, out List<Direction> moves)
    {
        walls = [];
        boxes = [];
        bot = Point.Empty;

        moves = [];
        
        var current = new Point(-1, 0);
        var readingMap = true;

        char lastCh = default;

        var width = 0;
        var height = 0;
        foreach (var ch in Util.InputChars<Day15>(sample))
        {
            if (readingMap)
            {
                if (ch == Util.Newline)
                {
                    if (lastCh == Util.Newline)
                    {
                        readingMap = false;
                        height = current.Y;
                        continue;
                    }
                    width = current.X + 1;
                    current.Y++;
                    current.X = -1;
                    lastCh = ch;
                    continue;
                }
                
                lastCh = ch;
                current.X++;
                switch (ch)
                {
                    case Wall: walls.Add(current); break;
                    case Box: boxes.Add(current); break;
                    case Empty: break;
                    case Bot: bot = current; break;
                    default: throw new ArgumentException($"Invalid map element: {ch}");
                }
                
            }
            else
            {
                if (ch == Util.Newline) continue;

                var dir = ch switch
                {
                    '^' => Direction.North,
                    'v' => Direction.South,
                    '>' => Direction.East,
                    '<' => Direction.West,
                    _ => throw new ArgumentException($"Invalid move: {ch}"),
                };
                moves.Add(dir);
            }
        }

        return (width, height);

    }
    
    
    
    // ReSharper disable once UnusedMember.Local
    private static void ShowWarehouse2(in Point bot, in HashSet<Point> walls, in HashSet<Point> boxes, in int width, in int height)
    {
        StringBuilder builder = new();
        var current = new Point(-1, 0);
        
        
        while (current.Y < height)
        {
            current.X++;
            if (current.X == width)
            {
                current.Y++;
                current.X = -1;
                builder.AppendLine();
                continue;
            }
            
            if (walls.Contains(current))
                builder.Append('#');
            else if (boxes.Contains(current))
            {
                builder.Append("[]");
                current.X++;
            }
            else if (current == bot)
                builder.Append('@');
            else
                builder.Append('.');

        }
        Console.WriteLine(builder);
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static (int width, int height) GetWarehouse2(bool sample, out Point bot, out HashSet<Point> walls, out HashSet<Point> boxes, out List<Direction> moves)
    {
        walls = [];
        boxes = [];
        bot = Point.Empty;

        moves = [];
        
        var current = new Point(-2, 0);
        var readingMap = true;

        char lastCh = default;

        var width = 0;
        var height = 0;
        foreach (var ch in Util.InputChars<Day15>(sample))
        {
            if (readingMap)
            {
                if (ch == Util.Newline)
                {
                    if (lastCh == Util.Newline)
                    {
                        readingMap = false;
                        height = current.Y;
                        continue;
                    }
                    width = current.X + 2;
                    current.Y++;
                    current.X = -2;
                    lastCh = ch;
                    continue;
                }
                
                lastCh = ch;
                current.X += 2;
                switch (ch)
                {
                    case Wall: 
                        walls.Add(current);
                        walls.Add(current.MoveCopy(Direction.East)); 
                        break;
                    case Box: 
                        boxes.Add(current);
                        break;
                    case Empty: break;
                    case Bot: bot = current; break;
                    default: throw new ArgumentException($"Invalid map element: {ch}");
                }
                
            }
            else
            {
                if (ch == Util.Newline) continue;

                var dir = ch switch
                {
                    '^' => Direction.North,
                    'v' => Direction.South,
                    '>' => Direction.East,
                    '<' => Direction.West,
                    _ => throw new ArgumentException($"Invalid move: {ch}"),
                };
                moves.Add(dir);
            }
        }

        return (width, height);

    }
}