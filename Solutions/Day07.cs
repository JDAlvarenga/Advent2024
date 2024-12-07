namespace Advent2024.Solutions;

public class Day07
{
    
    public static long Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day07>(sample);
        long totalSum = 0;
        var validKey = new Key(0, 0);
        var cache = new Dictionary<Key, int>();
        
        while (file.ReadLine() is { } line)
        {
            cache.Clear();
            cache.Add(validKey, 1);
            
            var equation = line.Split().Select(s => long.Parse(s.Trim([' ', ':']))).ToArray();
            if (Valid(equation, new Key(equation[0], equation.Length-1), cache) > 0)
                totalSum += equation[0];
        }

        return totalSum;
    }
    public static long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day07>(sample);
        long totalSum = 0;
        var validKey = new Key(0, 0);
        var cache = new Dictionary<Key, int>();
        
        while (file.ReadLine() is { } line)
        {
            cache.Clear();
            cache.Add(validKey, 1);
            
            var equation = line.Split().Select(s => long.Parse(s.Trim([' ', ':']))).ToArray();
            if (Valid2(equation, new Key(equation[0], equation.Length - 1), cache) > 0)
                totalSum += equation[0];
        }

        return totalSum;
    }
    private static int Valid(in long[] equation, Key key, in Dictionary<Key, int> cache)
    {
        var (result, idx) = key;
        
        if (idx == 0)
        {
            return result == 0 ? 1 : 0;
        }
        var total = 0;
        
        // mult operator
        if (result % equation[idx] == 0)
        {
            var multResult = result/equation[idx];
            var multKey = new Key(multResult, idx-1);
            if (!cache.ContainsKey(multKey))
                cache.Add(multKey, Valid(equation, multKey, cache));
            
            total += cache[multKey];
        }
        
        
        // add operator
        var addResult = result - equation[idx];
        if (addResult >= 0)
        {
            var addKey = new Key(addResult, idx-1);
            if (!cache.ContainsKey(addKey))
                cache.Add(addKey, Valid2(equation, addKey, cache));
            
            total += cache[addKey];
        }

        return total;
    }
    
    private static int Valid2(in long[] equation, Key key, in Dictionary<Key, int> cache)
    {
        var (result, idx) = key;
        
        if (idx == 0)
        {
            return result == 0 ? 1 : 0;
        }
        var total = 0;
        
        // mult operator
        if (result % equation[idx] == 0)
        {
            var multResult = result/equation[idx];
            var multKey = new Key(multResult, idx-1);
            if (!cache.ContainsKey(multKey))
                cache.Add(multKey, Valid2(equation, multKey, cache));
            
            total += cache[multKey];
        }
        
        
        // add operator
        var addResult = result - equation[idx];
        if (addResult >= 0)
        {
            var addKey = new Key(addResult, idx-1);
            if (!cache.ContainsKey(addKey))
                cache.Add(addKey, Valid2(equation, addKey, cache));
            
            total += cache[addKey];
        }
        
        
        // concat operator
        var idxStr = equation[idx].ToString();
        var resultStr = result.ToString();
        if (resultStr.Length - idxStr.Length > 0 // produces a valid number
            && resultStr[^idxStr.Length..] == idxStr) // numbers match
        {
            var concatResult = long.Parse(resultStr[..^idxStr.Length]);
            var concatKey = new Key(concatResult, idx-1);
            if (!cache.ContainsKey(concatKey))
                cache.Add(concatKey, Valid2(equation, concatKey, cache));
            
            total += cache[concatKey];
        }

        return total;
    }

    private record Key(long Result, int Idx);
}