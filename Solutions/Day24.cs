using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

public class Day24: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        Dictionary<string, bool> wires = [];

        Dictionary<string, HashSet<string>> connections = [];
        Dictionary<string, IGate> pendingWires = [];
        
        
        using var file = Util.GetInputStream<Day24>(sample);
        while (file.ReadLine() is { } line&& !string.IsNullOrEmpty(line))
        {
            wires.Add(line[..3], line[^1] == '1');
        }

        while (!file.EndOfStream)
        {
            var gateStr = file.ReadLine()!.Split();
            var wire1 = gateStr[0];
            var gateType = gateStr[1];
            var wire2 = gateStr[2];
            var outWire = gateStr[4];
            
            var unknownWires = 2;
            if (wires.TryGetValue(wire1, out var wire1Value)) unknownWires--;
            if (wires.TryGetValue(wire2, out var wire2Value)) unknownWires--;
            if (unknownWires == 0)
            {
                var val = gateType switch
                {
                    "AND" => wire1Value && wire2Value,
                    "OR" => wire1Value || wire2Value,
                    "XOR" => wire1Value ^ wire2Value,
                    _ => throw new Exception($"Invalid gate type: {gateType}")
                };
            
                // Resolve pending connections
                SolveWire(outWire, val, ref wires, ref connections, ref pendingWires);
            
            }
            else
            {
                IGate gate = gateType switch
                {
                    "AND" => new AndGate(wire1, wire2),
                    "OR" => new OrGate(wire1, wire2),
                    "XOR" => new XorGate(wire1, wire2),
                    _ => throw new Exception($"Invalid gate type: {gateType}")
                };

                connections.TryAdd(gate.Wire1, []);
                connections[gate.Wire1].Add(outWire);
            
                connections.TryAdd(gate.Wire2, []);
                connections[gate.Wire2].Add(outWire);
            
                pendingWires.Add(outWire, gate);
            }
            
        }

        return wires.Where(kvp => kvp.Key.StartsWith('z'))
            .OrderByDescending(kvp => kvp.Key)
            .Aggregate((long)0, (agg, kvp) => (agg << 1 ) | (uint)(kvp.Value ? 1 : 0));
    }
    
    [Benchmark, Arguments(false)]
    public string Part2(bool sample = false)
    {
        var inputSize = 0;
        using var file = Util.GetInputStream<Day24>(sample);
        while (file.ReadLine() is { } line&& !string.IsNullOrEmpty(line))
        {
            if (line.StartsWith('x')) inputSize++;
        }

        
        
        Dictionary<string, IGate> gates = [];
        Dictionary<string, HashSet<string>> connections = [];
        
        
        while (!file.EndOfStream)
        {
            var gateStr = file.ReadLine()!.Split();
            var wire1 = gateStr[0];
            var gateType = gateStr[1];
            var wire2 = gateStr[2];
            var outWire = gateStr[4];
            
            
            gates.Add(outWire, gateType switch
            {
                "AND" => new AndGate(wire1, wire2),
                "OR" => new OrGate(wire1, wire2),
                "XOR" => new XorGate(wire1, wire2),
                _ => throw new Exception($"Invalid gate type: {gateType}")
            });
            
            connections.TryAdd(wire1, []);
            connections[wire1].Add(outWire);
            
            connections.TryAdd(wire2, []);
            connections[wire2].Add(outWire);

        }
        
        HashSet<string> badOuts = [];


        var lastOutWire = $"z{inputSize}";
        foreach (var (oWire, gate) in gates)
        {
            if (gate is OrGate)
            {
                // unless outs to the last bit
                if (oWire == lastOutWire) continue;
                
                // must be followed by one 'And'  and one 'Xor' gate
                if (!connections.TryGetValue(oWire, out var cons))
                {
                    badOuts.Add(oWire);
                    continue;
                }
                bool cAnd = false, cXor = false;
                foreach (var con in cons)
                    if (gates[con] is AndGate) cAnd = true;
                    else if (gates[con] is XorGate) cXor = true;

                if (!cAnd || !cXor) badOuts.Add(oWire);
                

                if (gates[gate.Wire1] is not AndGate) badOuts.Add(gate.Wire1);
                if (gates[gate.Wire2] is not AndGate) badOuts.Add(gate.Wire2);
            }
            else if (gate is AndGate)
            {
                // is first input
                if (gate.Wire1 is "x00" or "y00")
                {
                    // must be followed by one 'And'  and one 'Xor' gate
                    if (!connections.TryGetValue(oWire, out var cons))
                    {
                        badOuts.Add(oWire);
                        continue;
                    }
                    bool cAnd = false, cXor = false;
                    foreach (var con in cons)
                        if (gates[con] is AndGate) cAnd = true;
                        else if (gates[con] is XorGate) cXor = true;

                    if (!cAnd || !cXor) badOuts.Add(oWire);
                }
                else
                {
                    // must always go to a single 'Or' gate
                    if (!connections.TryGetValue(oWire, out var cons))
                    {
                        badOuts.Add(oWire);
                        continue;
                    }

                    if (cons.Count != 1 || gates[cons.First()] is not OrGate)
                    {
                        badOuts.Add(oWire);
                    }
                }
            }
            
            
            else if (gate is XorGate)
            {
                if (oWire.StartsWith('z'))
                {
                    if (oWire == "z00")
                    {
                        if (gate.Wire1 != "x00" && gate.Wire1 != "y00")
                            badOuts.Add(oWire);
                    }
                    else
                    {
                        if (gate.Wire1.StartsWith('x') || gate.Wire1.StartsWith('y'))
                            badOuts.Add(oWire);
                    }
            
                    continue;
                }
                
                if (!gate.Wire1.StartsWith('x') && !gate.Wire1.StartsWith('y'))
                    badOuts.Add(oWire);
                
                // must be followed by one 'And'  and one 'Xor' gate
                var cons = connections[oWire];
                bool cAnd = false, cXor = false;
                foreach (var con in cons)
                    if (gates[con] is AndGate) cAnd = true;
                    else if (gates[con] is XorGate) cXor = true;
            
                if (!cAnd || !cXor) badOuts.Add(oWire);
            }
        }

        return string.Join(',', badOuts.Order());
    }

    private static void SolveWire(string wire, bool wireValue, 
        ref Dictionary<string, bool> wires,
        ref Dictionary<string, HashSet<string>> connections,
        ref Dictionary<string, IGate> pendingWires)
    {
        wires.Add(wire, wireValue);
        if (!connections.Remove(wire, out var downConnections)) return;
        
        foreach (var pendingWire in downConnections)
        {
            if (!pendingWires.TryGetValue(pendingWire, out var gate)) continue;
            if (!wires.ContainsKey(gate.Wire1) || !wires.ContainsKey(gate.Wire2)) continue;
            
            pendingWires.Remove(pendingWire);
            var nextWireValue = gate.GetValue(wires[gate.Wire1], wires[gate.Wire2]);
            SolveWire(pendingWire, nextWireValue, ref wires, ref connections, ref pendingWires);
        }
    }
    
    private record AndGate(string Wire1, string Wire2):IGate
    {
        public bool GetValue(bool wire1, bool wire2) => wire1 && wire2;
    }
    private record OrGate(string Wire1, string Wire2):IGate
    {
        public bool GetValue(bool wire1, bool wire2) => wire1 || wire2;
    }
    private record XorGate(string Wire1, string Wire2):IGate
    {
        public bool GetValue(bool wire1, bool wire2) => wire1 ^ wire2;
    }

    private interface IGate
    {
        string Wire1 { get; }
        string Wire2 { get; }
        bool GetValue(bool wire1, bool wire2);
    }
    
    
}