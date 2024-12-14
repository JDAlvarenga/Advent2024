using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Advent2024.Solutions;

public partial class Day14
{
    private const int Time = 100;
    private const int Width = 101;
    private const int Height = 103;
    
    private const int WidthSample = 11;
    private const int HeightSample = 7;
    public static int Part1(bool sample = false)
    {
        var width = sample ? WidthSample : Width;
        var height = sample ? HeightSample : Height;
        var halfWidth = width / 2;
        var halfHeight = height / 2;
        
        var quadrantCount = new int[4];
        foreach (var robot in GetRobots(sample))
        {
            var x = (int)(((robot.Position.X + robot.Velocity.X * Time) % width) + width) % width;
            var y = (int)(((robot.Position.Y + robot.Velocity.Y * Time) % height) + height) % height;

            if (x < halfWidth)
            {
                if (y < halfHeight) quadrantCount[0]++;
                else if (y > halfHeight) quadrantCount[2]++;
            }
            else if (x > halfWidth)
            {
                if (y < halfHeight) quadrantCount[1]++;
                else if (y > halfHeight) quadrantCount[3]++;
            }

        }
        return quadrantCount[0] * quadrantCount[1] * quadrantCount[2] * quadrantCount[3];
    }

    public static int Part2()
    {
        var robots = GetRobots(false).ToList();
        var time = 0;
        var min = float.MaxValue;
        var minTime = 0;
        var map = new bool[Height, Width];
        do
        {
            Array.Clear(map);
            foreach (var robot in robots)
            {
                robot.Position.X = (((robot.Position.X + robot.Velocity.X) % Width) + Width) % Width;
                robot.Position.Y = (((robot.Position.Y + robot.Velocity.Y) % Height) + Height) % Height;

                map[(int)robot.Position.Y, (int)robot.Position.X] = true;
            }


            var medianDistance = 0f;
            for (var i = 0; i < robots.Count-1; i++)
            for (var j = i+1; j < robots.Count; j++)
            {
                medianDistance += Vector2.DistanceSquared(robots[i].Position, robots[j].Position);
            }
            
            time++;
            if (min <= medianDistance) continue;
            
            min = medianDistance;
            minTime = time;
            
        } while (time < 10_000);
        // } while (time - minTime < 10_000);

        return minTime;
    }
    
    public static void ShowAtTime(int seconds)
    {
        var robots = GetRobots(false).ToList();
        var time = 0;
        bool[,] map = new bool[Height, Width];
        do
        {
            Array.Clear(map);
            foreach (var robot in robots)
            {
                robot.Position.X = (((robot.Position.X + robot.Velocity.X) % Width) + Width) % Width;
                robot.Position.Y = (((robot.Position.Y + robot.Velocity.Y) % Height) + Height) % Height;

                map[(int)robot.Position.Y, (int)robot.Position.X] = true;
            }
           
            time++;
            
        } while (time<seconds);

        StringBuilder builder = new();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                builder.Append(map[y, x] ? '#' : ' ');
            }
            builder.AppendLine();
        }
        Console.WriteLine(builder);
        
    }

    private class Robot(Vector2 position, Vector2 velocity)
    {
        public Vector2 Position = position;
        public readonly Vector2 Velocity = velocity;
    };

    private static IEnumerable<Robot> GetRobots(bool sample)
    {
        using var file = Util.GetInputStream<Day14>(sample);

        while (!file.EndOfStream)
        {
            var match = RobotRegex().Match(file.ReadLine()!);
            yield return new Robot(
                position: new Vector2(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
                velocity: new Vector2(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value)));
        }
    }
    
    [GeneratedRegex(@"^p=(\d+),(\d+) v=(-?\d+),(-?\d+)$")]
    private static partial Regex RobotRegex();
}