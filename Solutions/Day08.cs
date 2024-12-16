using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day08: IDay
{
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        GetAntennas(sample, out var antennasDictionary, out var mapSize);
        HashSet<Vector2> antiNodes = [];
        foreach (var frequency in antennasDictionary.Values)
        {
            for (var i = 0; i < frequency.Count-1; i++)
            for (var n = i + 1; n < frequency.Count; n++)
            {
                var diff = frequency[i] - frequency[n];
                var node1 = frequency[i] + diff;
                var node2 = frequency[n] - diff;
                
                if (IsInMap(node1, mapSize)) antiNodes.Add(node1);
                if (IsInMap(node2, mapSize)) antiNodes.Add(node2);
            }
        }
        
        return antiNodes.Count;
    }
    
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        GetAntennas(sample, out var antennasDictionary, out var mapSize);
        HashSet<Vector2> antiNodes = [];
        foreach (var frequency in antennasDictionary.Values)
        {
            for (var i = 0; i < frequency.Count-1; i++)
            for (var n = i + 1; n < frequency.Count; n++)
            {
                var diff = frequency[i] - frequency[n];
                var node1 = frequency[i] + diff;
                var node2 = frequency[n] - diff;
                
                while (IsInMap(node1, mapSize))
                {
                    antiNodes.Add(node1);
                    node1 += diff;
                }

                while (IsInMap(node2, mapSize))
                {
                    antiNodes.Add(node2);
                    node2 -= diff;
                }
                
                // Add antennas positions when at least two
                antiNodes.Add(frequency[i]);
                antiNodes.Add(frequency[n]);
                
            }
        }

        // DisplayNodes(antiNodes, mapSize);
        
        return antiNodes.Count;
    }

    private static bool IsInMap(in Vector2 vector, in Vector2 mapSize) => vector.X >=0 &&
                                                             vector.X <= mapSize.X &&
                                                             vector.Y >= 0 &&
                                                             vector.Y <= mapSize.Y;

    private static void GetAntennas(bool sample, out Dictionary<char, List<Vector2>> antennas, out Vector2 mapSize)
    {
        antennas = new Dictionary<char, List<Vector2>>();
        var current = new Vector2(-1, 0);
        foreach (var c in Util.InputChars<Day08>(sample))
        {
            if (c is Util.Newline)
            {
                current.X = -1;
                current.Y++;
                continue;
            }
            
            current.X++;
            if (c is '.') continue;
            
            if (!antennas.ContainsKey(c))
                antennas.Add(c, []);
            
            antennas[c].Add(current);
        }
        mapSize = current;
    }
    
    // private static void DisplayNodes(in HashSet<Vector2> antiNodes, in Vector2 mapSize)
    // {
    //     var map = new char[(int)mapSize.Y+1, (int)mapSize.X+1];
    //     
    //     for (var y = 0; y <= mapSize.Y; y++)
    //     for (var x = 0; x <= mapSize.X; x++)
    //         map[y, x] = '.';
    //     
    //     foreach (var node in antiNodes)
    //         map[(int)node.Y, (int)node.X] = '#';
    //
    //     for (var y = 0; y <= mapSize.Y; y++)
    //     {
    //         for (var x = 0; x <= mapSize.X; x++)
    //             Console.Write(map[y, x]);
    //         
    //         Console.WriteLine();    
    //     }
    // }
}