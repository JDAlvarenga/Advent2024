using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day03: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        
        using var file = Util.GetInputStream<Day03>(sample);
        string input = file.ReadToEnd();
        
        Regex regex = new(@"mul\((\d{1,3}),(\d{1,3})\)", RegexOptions.Compiled|RegexOptions.Multiline);

        var matches = regex.Matches(input);

        return matches.Aggregate(0, (current , m) => current + int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
    }
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day03>(sample);
        string input = file.ReadToEnd();
        
        Regex regex = new(@"(?>don't\(\))|(?>do\(\))|(?>mul\((\d{1,3}),(\d{1,3})\))", RegexOptions.Compiled|RegexOptions.Multiline);

        var matches = regex.Matches(input);
        
        bool ignore = false;
        return matches.Aggregate(0, (current, match) =>
        {
            switch (match.Groups[0].Value)
            {
                case "do()": ignore = false; break;
                case "don't()": ignore = true; break;
                default: return current + (ignore ? 0 : int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value));
            }

            return current;
        });
    }

}