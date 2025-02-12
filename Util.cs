namespace Advent2024;

public static class Util
{
    public static StreamReader GetInputStream<T>(bool sample = false)
    {
        var path = Path.Combine(
            "input",
            sample ? "samples" : string.Empty,
            $"{typeof(T).Name.ToLower()}.txt"
        );
        return new StreamReader(path);
    }
    
    public const char Newline = '\n';
    public static IEnumerable<char> InputChars<T>(bool sample)
    {
        using var file = Util.GetInputStream<T>(sample);
        
        
        while (!file.EndOfStream)
        {
            var ch = (char)file.Read();
            if (ch is '\r' or '\n')
            {
                if (file.Peek() is '\n' or '\r') file.Read();
                
                yield return Newline;
            }
            else
                yield return ch;
        }
    }
    
    [Flags]
    public enum Direction
    {
        North = 1,
        NorthEast = 2,
        East = 4,
        SouthEast = 8,
        South = 16,
        SouthWest = 32,
        West = 64,
        NorthWest = 128
    }
    private static int DirectionIdx(Direction dir) => dir switch
    {
        Direction.North => 0,
        Direction.NorthEast => 1,
        Direction.East => 2,
        Direction.SouthEast => 3,
        Direction.South => 4,
        Direction.SouthWest => 5,
        Direction.West => 6,
        Direction.NorthWest => 7,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };

    public static Direction RotateDirection(Direction direction, in Direction turn) => RotateDirection(ref direction, turn);
    public static Direction RotateDirection(ref Direction direction, in Direction turn)
    {
        var offset = DirectionIdx(turn);
        var dirIdx = DirectionIdx(direction);
        direction = (Direction) (1 << ((dirIdx + offset) % 8));
        return direction;
    }

    public record struct Point(int X, int Y)
    {
        public static readonly Point Empty = new(0, 0);
        public void Offset(in Point delta)
        {
            X += delta.X;
            Y += delta.Y;
        }

        public void Offset(in int deltaX, in int deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }

        public Point OffsetCopy(in Point delta) => new (X + delta.X, Y + delta.Y);
        public Point OffsetCopy(in int deltaX, in int deltaY) => new (X + deltaX, Y + deltaY);

        public Point MoveCopy(in Direction dir, in int spaces = 1)
        {
            var moved = this with { };
            moved.Move(dir, spaces);
            return moved;
        }
        public void Move(in Direction dir, in int spaces = 1)
        {
            switch (dir)
            {
                case Direction.North: Offset(0,-spaces); break;
                case Direction.NorthEast: Offset(spaces, -spaces); break;
                case Direction.East: Offset(spaces, 0); break;
                case Direction.SouthEast: Offset(spaces, spaces); break;
                case Direction.South: Offset(0, spaces); break;
                case Direction.SouthWest: Offset(-spaces, spaces); break;
                case Direction.West: Offset(-spaces, 0); break;
                case Direction.NorthWest: Offset(-spaces, -spaces); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
        public int ManhattanDistance(in Point point) => Math.Abs(point.X - X) + Math.Abs(point.Y - Y);
    }
    // public static Point MovePoint(Point point, in Direction dir, in int spaces = 1)
    // {
    //     MovePoint(ref point, dir, spaces);
    //     return point;
    // }
    // public static void MovePoint(ref Point point, in Direction dir, in int spaces = 1)
    // {
    //     switch (dir)
    //     {
    //         case Direction.North: point.Offset(0,-spaces); break;
    //         case Direction.NorthEast: point.Offset(spaces, -spaces); break;
    //         case Direction.East: point.Offset(spaces, 0); break;
    //         case Direction.SouthEast: point.Offset(spaces, spaces); break;
    //         case Direction.South: point.Offset(0, spaces); break;
    //         case Direction.SouthWest: point.Offset(-spaces, spaces); break;
    //         case Direction.West: point.Offset(-spaces, 0); break;
    //         case Direction.NorthWest: point.Offset(-spaces, -spaces); break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
    //     }
    // }
    public static bool IsValidPoint(in Point pos, in int maxX, in int maxY, in int minX = 0, in int minY = 0)
    {
        return pos.Y >= minY && 
               pos.Y <= maxY &&
               pos.X >= minX &&
               pos.X <= maxX;
    }
}