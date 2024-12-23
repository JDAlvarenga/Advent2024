using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;
using Point = Util.Point;
using StepsDictionary = Dictionary<Day21.Transition, List<string>>;

public class Day21: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        GetDirPaths(out var dirSteps);
        GetNumPaths(out var numSteps);
        
        long total = 0;
        using var file = Util.GetInputStream<Day21>(sample);
        while (!file.EndOfStream)
        {
            string code = file.ReadLine()!;
            var codeNum = int.Parse(code[..^1]);
            var moves = ShortestNumpad(3, code, numSteps, dirSteps);
            total += moves * codeNum;
            
        }
        
        return total;
    }
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        GetDirPaths(out var dirSteps);
        GetNumPaths(out var numSteps);
        
        long total = 0;
        using var file = Util.GetInputStream<Day21>(sample);
        while (!file.EndOfStream)
        {
            string code = file.ReadLine()!;
            var codeNum = int.Parse(code[..^1]);
            var moves = ShortestNumpad(26, code, numSteps, dirSteps);
            // Console.WriteLine($"{code}: {moves}");
            total += moves * codeNum;
            
        }
        
        return total;
    }
    // return the shortest way to introduce a 'code' in a numerical keypad when there are 'state.Depth' robots in between
    private static long ShortestNumpad(in int robots, in string code, in StepsDictionary numSteps, in StepsDictionary dirSteps)
    {
        long codeLenght = 0;
        
        Dictionary<State, long> dirCache = [];
        // start aiming at the 'A' key in the numerical keypad
        var lastCode = A;
        foreach (var c in code)
        {
            var minStepsLenght = long.MaxValue;
            var transition = new Transition(lastCode, c);
            // get the shortest way to press 'c' in the numerical keypad
            foreach (var steps in numSteps[transition])
            {
                long stepsLenght = 0;
                var lastStep = A;
                // get the shortest way to press each step in a directional keypad
                foreach (var step in steps)
                {
                    stepsLenght += ShortestDir(
                        state: new State(
                            Transition: new Transition(lastStep, step),
                            Depth: robots-1),
                        dirSteps,
                        ref dirCache
                    );
                    lastStep = step;
                }
                
                minStepsLenght = long.Min(minStepsLenght, stepsLenght);
                
            }

            codeLenght += minStepsLenght;
            lastCode = c;
        }

        return codeLenght;
    }
    
    // return the shortest way to move from/to a key in a directional keypad when there are 'state.Depth' robots in between 
    private static long ShortestDir(in State state, in StepsDictionary dirSteps, ref Dictionary<State, long> cache)
    {
        if (state.Depth == 1) return dirSteps[state.Transition][0].Length;
        if (cache.TryGetValue(state, out var cached)) return cached;

        var minStepsLenght = long.MaxValue;
        
        foreach (var steps in dirSteps[state.Transition])
        {
            long stepsLenght = 0;
            var lastStep = A;
            foreach (var step in steps)
            {
                var nextState = new State(new Transition(lastStep, step), state.Depth - 1 );
                stepsLenght += ShortestDir(nextState, dirSteps, ref cache);
                lastStep = step;
            }
            minStepsLenght = Math.Min(minStepsLenght, stepsLenght);
        }
        
        cache.Add(state, minStepsLenght);
        return minStepsLenght;
    }
    private record State(Transition Transition, int Depth);
    // ReSharper disable NotAccessedPositionalProperty.Global
    public record Transition(char From, char To);
    // ReSharper restore NotAccessedPositionalProperty.Global
    
    // Generate all possible ways to move from/to each key in a directional keypad
    private static void GetDirPaths(out StepsDictionary dirPaths)
    {
        dirPaths = [];
        foreach (var (fromKey, fromPos) in DirPad)
        foreach (var (toKey, toPos) in DirPad)
        {
            var transition = new Transition(fromKey, toKey);
            var moveX = toPos.X - fromPos.X;
            var moveY = toPos.Y - fromPos.Y;
            
            if (fromKey == toKey)
            {
                dirPaths.Add(transition, ["A"]);
            }
            else if (fromKey == Left && toKey is Up or A)
            {
                dirPaths.Add(transition, [$"{new string(Right, moveX)}{new string(Up, -moveY)}A"]);
            }
            else if (toKey == Left && fromKey is Up or A)
            {
                dirPaths.Add(transition, [$"{new string(Down, moveY)}{new string(Left, -moveX)}A"]);
            }
            else
            {
                if (moveX == 0)
                {
                    dirPaths.Add(transition, [$"{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}A"]);
                }
                else if (moveY == 0)
                {
                    dirPaths.Add(transition, [$"{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}A"]);
                }
                else
                {
                    dirPaths.Add(transition, [
                        $"{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}A",
                        $"{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}A"
                    ]);
                }
                
            }
        }
    }
    
    // Generate all possible ways to move from/to each key in a numerical keypad
    private static void GetNumPaths(out StepsDictionary numpadPaths)
    {
        numpadPaths = [];
        foreach (var (fromKey, fromPos) in Numpad)
        foreach (var (toKey, toPos) in Numpad)
        {
            var moveX = toPos.X - fromPos.X;
            var moveY = toPos.Y - fromPos.Y;
            var transition = new Transition(fromKey, toKey);
            if (fromKey == toKey)
            {
                numpadPaths.Add(transition, ["A"]);
            }
            else if (fromPos.X == 0 && toPos.Y == 3)
            {
                numpadPaths.Add(transition, [$"{new string(Right, moveX)}{new string(Down, moveY)}A"]);
            }
            else if (fromPos.Y == 3 && toPos.X == 0)
            {
                numpadPaths.Add(transition, [$"{new string(Up, -moveY)}{new string(Left, -moveX)}A"]);
            }
            else
            {
                if (moveX == 0)
                {
                    numpadPaths.Add(transition, [$"{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}A"]);
                }
                else if (moveY == 0)
                {
                    numpadPaths.Add(transition, [$"{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}A"]);
                }
                else
                {
                    numpadPaths.Add(transition, [
                        $"{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}A",
                        $"{new string(moveX > 0 ? Right: Left, int.Abs(moveX))}{new string(moveY > 0 ? Down: Up, int.Abs(moveY))}A"
                    ]);
                }
                
            }
        }
    }
    
    private const char Zero = '0';
    private const char One = '1';
    private const char Two = '2';
    private const char Three = '3';
    private const char Four = '4';
    private const char Five = '5';
    private const char Six = '6';
    private const char Seven = '7';
    private const char Eight = '8';
    private const char Nine = '9';
    private const char A = 'A';
    private const char Up = '^';
    private const char Down = 'v';
    private const char Left = '<';
    private const char Right = '>';
    
    // Numerical keypad positions
    private static readonly Dictionary<char, Point> Numpad = new()
    {
        { A, new Point(2, 3) },
        { Zero, new Point(1, 3) },
        { One, new Point(0, 2) },
        { Two, new Point(1, 2) },
        { Three, new Point(2, 2) },
        { Four, new Point(0, 1) },
        { Five, new Point(1, 1) },
        { Six, new Point(2, 1) },
        { Seven, new Point(0, 0) },
        { Eight, new Point(1, 0) },
        { Nine, new Point(2, 0) }

    };
    
    // Directional keypad positions
    private static readonly Dictionary<char, Point> DirPad = new()
    {
        {Up, new Point(1,0)},
        {A, new Point(2,0)},
        {Left, new Point(0,1)},
        {Down, new Point(1,1)},
        {Right, new Point(2,1)},
    };

}