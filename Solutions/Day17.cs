using System.Numerics;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

[ShortRunJob]
public class Day17: IDay
{
    [Benchmark, Arguments(false)]
    public string Part1(bool sample = false)
    {
        GetInput(sample, out var a, out var b, out var c, out var program);
        return string.Join(',', Run(a, b, c, program));
    }
    
    
    // Only on actual input
    // public string Part1V2(bool sample = false)
    // {
    //     GetInput(sample, out var a, out var b, out var c, out var program);
    //
    //     var idx = program.Count;
    //     StringBuilder builder = new();
    //     // List<int> output = [];
    //     do
    //     {
    //         b = a & 0b111;
    //         b ^= 0b101;
    //         c = a >> b;
    //         b ^= 0b110;
    //         a >>= 3;
    //         b ^= c;
    //         builder.Append(b & 0b111);
    //         builder.Append(',');
    //     } while (a != 0);
    //     builder.Remove(builder.Length - 1, 1);
    //     return builder.ToString();
    // }
    
    
    // Equivalent of input program (real input only)
    // while (a != 0)
    // {
    //     t = ((a & 0b111) ^ 0b101)
    //         out == ((t ^ 0b110) ^ (a >> t)) & 0b111
    //     a >>= 3
    // }
    [Benchmark, Arguments(false)]
    public BigInteger Part2(bool sample = false)
    {
        GetInput(sample, out var a, out var b, out var c, out var program);

        return GetRegisters(program);
    }
    
    // build the solution from the end 3 bits at a time
    private BigInteger GetRegisters(in List<byte> program)
    {
        HashSet<BigInteger> validRegisters = [];
        Queue<(int idx, BigInteger register)> queue = [];
    
        queue.Enqueue((program.Count - 1, 0));
    
            
        while (queue.TryDequeue(out var result))
        {
            var (idx, register) = result;
            if (idx == -1)
            {
                validRegisters.Add(register);
                continue;
            }
            // value to check from the end of output/program
            var fromEnd = program.Count - idx;
            
            for (int offset = 0; offset < 8; offset++)
            {
                var a = (register << 3) | offset;
                var output = Run(a, program);

                if (output.Count < fromEnd) continue;
                if (program[^fromEnd] != output[^fromEnd]) continue;
                
                queue.Enqueue((idx-1, a));
                    
            }
        }
    
        return validRegisters.Min();
    }
    
    private List<byte> Run(int a, int b, int c, in List<byte> program)
    {
        var idx = -2;
        List<byte> output = [];
        while ((idx+=2) < program.Count)
        {
            switch (program[idx])
            {
                case 0: a >>= ComboOperand(program[idx+1], a, b, c); break;
                case 1: b ^= program[idx+1]; break;
                case 2: b = ComboOperand(program[idx+1], a, b, c) % 8; break;
                case 3:
                    if (a == 0) break;
                    idx = program[idx+1] - 2;
                    break;
                case 4: b ^= c; break;
                case 5: 
                    output.Add((byte)(ComboOperand(program[idx+1], a, b, c) % 8));
                    break;
                
                case 6: b = a >> ComboOperand(program[idx+1], a, b, c); break;
                case 7: c = a >> ComboOperand(program[idx+1], a, b, c); break;
            }
        }

        return output;
    }
    // Copy using BigInteger for part 2 
    private List<byte> Run(BigInteger a, in List<byte> program)
    {
        var idx = -2;
        BigInteger b = BigInteger.Zero;
        BigInteger c = BigInteger.Zero;
        List<byte> output = [];
        while ((idx+=2) < program.Count)
        {
            switch (program[idx])
            {
                case 0: a >>= (int)ComboOperand(program[idx+1], a, b, c); break;
                case 1: b ^= program[idx+1]; break;
                case 2: b = ComboOperand(program[idx+1], a, b, c) % 8; break;
                case 3:
                    if (a == 0) break;
                    idx = program[idx+1] - 2;
                    break;
                case 4: b ^= c; break;
                case 5: 
                    output.Add((byte)(ComboOperand(program[idx+1], a, b, c) % 8));
                    break;
                
                case 6: b = a >> (int)ComboOperand(program[idx+1], a, b, c); break;
                case 7: c = a >> (int)ComboOperand(program[idx+1], a, b, c); break;
            }
        }
        return output;
    }


    private static int ComboOperand(in byte op, in int a, in int b, in int c) => op switch
    {
        <= 3 => op,
        4 => a,
        5 => b,
        6 => c,
        _ => throw new ArgumentException($"Unknown combo operand: {op}")

    }
    
    // Copy using BigInteger for part 2
    ;private static BigInteger ComboOperand(in byte op, in BigInteger a, in BigInteger b, in BigInteger c) => op switch
    {
        <= 3 => op,
        4 => a,
        5 => b,
        6 => c,
        _ => throw new ArgumentException($"Unknown combo operand: {op}")

    };

    private static void GetInput(bool sample, out int a, out int b, out int c,
        out List<byte> program)
    {
        using var file = Util.GetInputStream<Day17>(sample);
        a = int.Parse(file.ReadLine()!.Split(':', StringSplitOptions.TrimEntries)[1]);
        b = int.Parse(file.ReadLine()!.Split(':', StringSplitOptions.TrimEntries)[1]);
        c = int.Parse(file.ReadLine()!.Split(':', StringSplitOptions.TrimEntries)[1]);
        file.ReadLine();
        program = file.ReadLine()!.Split([' ',',']).Skip(1).Select(byte.Parse).ToList();
    }
}