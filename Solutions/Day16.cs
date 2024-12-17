using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;
using Point = Util.Point;
using Direction = Util.Direction;

[ShortRunJob]
public class Day16: IDay
{
    private const char Wall = '#';
    private const char Empty = '.';
    private const char Start = 'S';
    private const char End = 'E';
    
    private static readonly  Direction[] AllowedDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
    
    [Benchmark, Arguments(false)]
    public int Part1(bool sample = false)
    {
        GetMap(sample, out var walls, out var start, out var end);
        return MinDijkstra(walls, start, end, out _);
    }
    
    [Benchmark, Arguments(false)]
    public int Part2(bool sample = false)
    {
        GetMap(sample, out var walls, out var start, out var end);
        var min = MinDijkstra(walls, start, end, out var scores);

        HashSet<Point> bestSeats = [];
        HashSet<State> validStates = [];
        Stack<State> stack = [];

        foreach (var sc in scores.Where(kvp => kvp.Key.Position == end && kvp.Value == min))
        {
            stack.Push(sc.Key);
            // validStates.Add(sc.Key);
        }
        // walk from end to start maintaining min cost
        while (stack.TryPop(out var state))
        {
            if (!validStates.Add(state)) continue;
            
            bestSeats.Add(state.Position);

            foreach (var direction in AllowedDirections)
            {
                if (state.Direction == direction) continue;

                int nextCost;
                Point nextPos;
                Direction oppositeDirection;
                if (direction == Util.RotateDirection(state.Direction, Direction.South))
                {
                    nextPos = state.Position.MoveCopy(direction);
                    oppositeDirection = state.Direction;
                    nextCost = scores[state] - 1;
                }
                else
                {
                    nextPos = state.Position;
                    oppositeDirection = Util.RotateDirection(direction, Direction.South);
                    nextCost = scores[state] - 1000;
                }
                
                var nextState = new State(nextPos, oppositeDirection);
                
                if (scores.TryGetValue(nextState, out var actualCost) && actualCost == nextCost)
                {
                    stack.Push(nextState);
                }

            }
        }
        return bestSeats.Count;
    }

    private static int MinDijkstra(in HashSet<Point> walls, in Point start, in Point end, out Dictionary<State, int> scores)
    {
        scores = [];
        var queue = new PriorityQueue<State, int>();

        var init = new State(start, Direction.East);
        queue.Enqueue(init, 0);
        scores[init] = 0;

        while (queue.TryDequeue(out var state, out var cost))
        {
            if (state.Position == end) continue;
            
            
            // neighbors
            foreach (var direction in AllowedDirections)
            {
                if (direction == Util.RotateDirection(state.Direction, Direction.South)) continue;
                
                
                
                int nextCost;
                Point nextPos;
                if (direction == state.Direction)
                {
                    nextPos = state.Position.MoveCopy(direction);
                    if (walls.Contains(nextPos)) continue;
                    nextCost = cost + 1;
                }
                else
                {
                    nextPos = state.Position;
                    nextCost = cost + 1000;
                }
                

                var nextState = new State(nextPos, direction);
                
                if (scores.TryAdd(nextState, nextCost))
                {
                    queue.Enqueue(nextState, nextCost);
                }
                else if (nextCost < scores[nextState])
                {
                    scores[nextState] = nextCost;
                    queue.Remove(nextState, out _, out _);
                    queue.Enqueue(nextState, nextCost);
                }
            }
        }
        
        // can only reach end while moving east or north
        var eScore = scores.GetValueOrDefault(new State(end, Direction.East), int.MaxValue);
        var nScore = scores.GetValueOrDefault(new State(end, Direction.North), int.MaxValue);

        return int.Min(eScore, nScore);
    }
    private record State (Point Position, Direction Direction);

    private static void GetMap(bool sample, out HashSet<Point> walls, out Point start, out Point end)
    {
        walls = [];
        start = Point.Empty;
        end = Point.Empty;
        

        var current = new Point(-1, 0);
        foreach (var ch in Util.InputChars<Day16>(sample))
        {
            if (ch == Util.Newline)
            {
                current.Y++;
                current.X = -1;
                continue;
            }
            current.X++;

            switch (ch)
            {
                case Empty:
                    continue;
                case Wall:
                    walls.Add(current);
                    break;
                case Start:
                    start = current;
                    break;
                case End:
                    end = current;
                    break;
            }
        }
    }
}