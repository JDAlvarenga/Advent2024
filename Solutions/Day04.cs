using System.Drawing;

namespace Advent2024.Solutions;

public class Day04
{
    private const string Word = "XMAS";
    
    private const string XWord = "MAS";
    private const int XWordMiddleIdx = 1;
    public static int Part1(bool sample = false)
    {
       GetMap(out var map, sample);
       var totalCount = 0;
       
       for (var y = 0; y < map.Length; y++)
       for (var x = 0; x < map.Length; x++)
       {
           // Only X characters
           if (map[y][x] != Word[0]) continue;
           
           // Check in every direction for the word
           foreach (var d in Enum.GetValues<Direction>())
           {
               var pos = new Point(x, y);
               if (CheckWord(pos: pos, d, map))
               {
                   totalCount++;
               }
           }

       }
       return totalCount;
    }

    // Check if word is found in pos moving in dir within map
    private static bool CheckWord(Point pos, in Direction dir, in char[][] map, bool word2 = false)
    {
        // each character in Word
        foreach (var c in word2 ? XWord : Word)
        {
            if (!IsValidPosition(pos, map)) return false;
            if (map[pos.Y][pos.X] != c) return false;
            Move(ref pos, dir);
        }

        return true;
    }


    public static int Part2(bool sample = false)
    {
        GetMap(out var map, sample);
        var totalCount = 0;
        
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map.Length; x++)
        {
            if (map[y][x] != XWord[XWordMiddleIdx]) continue;
            
            var ne = new Point(x,y); Move(ref ne, Direction.NorthEast);
            var se = new Point(x,y); Move(ref se, Direction.SouthEast);
            var sw = new Point(x,y); Move(ref sw, Direction.SouthWest);
            var nw = new Point(x,y); Move(ref nw, Direction.NorthWest);

            if ((CheckWord(ne, Direction.SouthWest, map, true) ||
                 CheckWord(sw, Direction.NorthEast, map, true))
                &&
                (CheckWord(nw, Direction.SouthEast, map, true) ||
                 CheckWord(se, Direction.NorthWest, map, true)))
            {
                totalCount++;
            }
        }
        return totalCount;
    }

    

    private static bool IsValidPosition(in Point pos, in char[][] map)
    {
        return pos.Y >= 0 && 
               pos.Y < map.Length &&
               pos.X >= 0 &&
               pos.X < map.Length;
    }

    private enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    
    private static void Move(ref Point point, Direction dir, int spaces = 1)
    {
        switch (dir)
        {
            case Direction.North: point.Y -= spaces; break;
            case Direction.NorthEast: point.X += spaces; point.Y -= spaces; break;
            case Direction.East: point.X += spaces; break;
            case Direction.SouthEast: point.X += spaces; point.Y += spaces; break;
            case Direction.South: point.Y += spaces; break;
            case Direction.SouthWest: point.X -= spaces; point.Y += spaces; break;
            case Direction.West: point.X -= spaces; break;
            case Direction.NorthWest: point.Y -= spaces; point.X -= spaces; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
        
    }

    // Get input
    private static void GetMap( out char[][] map, bool sample)
    {
        using var file = Util.GetInputStream<Day04>(sample);

        var first = file.ReadLine()!.ToCharArray();
        map = new char[first.Length][];

        map[0] = first;
        
        for (var y = 1; y < first.Length; y++)
        {
            map[y] = file.ReadLine()!.ToCharArray();
        }
    }
}