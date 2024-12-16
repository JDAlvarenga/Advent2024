using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using Point = Advent2024.Util.Point;
namespace Advent2024.Solutions;

[ShortRunJob]
public partial class Day13: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        long total = 0;
        foreach (var machine in GetClawMachines(sample))
        {
            // int x = 8400, Y = 5400, Xa=94, Ya = 34, Xb = 22, Yb = 67;
            // var B = (x*Ya - Y*Xa) / (Xb*Ya - Xa*Yb);
            // var A = (x*Yb - Y*Xb) / (Xa*Yb - Xb*Ya);

            var (prize, aButton, bButton) = machine;
            var a = (double)(prize.X * bButton.Y - prize.Y * bButton.X) / (aButton.X * bButton.Y - bButton.X * aButton.Y);
            var b = (double)(prize.X * aButton.Y - prize.Y * aButton.X) / (bButton.X * aButton.Y - aButton.X * bButton.Y);

            
            if (double.IsInteger(a) && double.IsInteger(b))
                total += (long)(a*3 + b);

        }
        return total;
    }
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        long total = 0;
        foreach (var machine in GetClawMachines(sample))
        {
            var (prize, aButton, bButton) = machine;
            prize.X += 10_000_000_000_000;
            prize.Y += 10_000_000_000_000;
            var a = (double)(prize.X * bButton.Y - prize.Y * bButton.X) / (aButton.X * bButton.Y - bButton.X * aButton.Y);
            var b = (double)(prize.X * aButton.Y - prize.Y * aButton.X) / (bButton.X * aButton.Y - aButton.X * bButton.Y);
            
            
            if (double.IsInteger(a) && double.IsInteger(b))
                total += (long)(a*3 + b);
        }
        return total;
    }
    
    private static IEnumerable<ClawMachine> GetClawMachines(bool sample)
    {
        using var file = Util.GetInputStream<Day13>(sample);
        while (!file.EndOfStream)
        {
            var aMatch = AButtonRegex().Match(file.ReadLine()!);
            var bMatch = BButtonRegex().Match(file.ReadLine()!);
            var prizeMatch = PrizeRegex().Match(file.ReadLine()!);
            file.ReadLine();

            yield return new ClawMachine(
                AButton: new Point(int.Parse(aMatch.Groups[1].Value), int.Parse(aMatch.Groups[2].Value)),
                BButton: new Point(int.Parse(bMatch.Groups[1].Value), int.Parse(bMatch.Groups[2].Value)),
                Prize: new ValueTuple<long, long>(int.Parse(prizeMatch.Groups[1].Value), int.Parse(prizeMatch.Groups[2].Value))
            );

        }
    }

    private record ClawMachine((long X, long Y) Prize, Point AButton, Point BButton);
    
    [GeneratedRegex(@"^Button A: X\+(\d+), Y\+(\d+)$")]
    private static partial Regex AButtonRegex();
    [GeneratedRegex(@"^Button B: X\+(\d+), Y\+(\d+)$")]
    private static partial Regex BButtonRegex();
    
    [GeneratedRegex(@"^Prize: X=(\d+), Y=(\d+)$")]
    private static partial Regex PrizeRegex();
    
}
