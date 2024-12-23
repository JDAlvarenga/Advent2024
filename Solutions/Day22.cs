using BenchmarkDotNet.Attributes;

namespace Advent2024.Solutions;

public class Day22: IDay
{
    [Benchmark, Arguments(false)]
    public long Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day22>(sample);

        long total = 0;
        while (!file.EndOfStream)
        {
            var number = long.Parse(file.ReadLine()!);
            for (var i = 0; i < 2000; i++)
            {
                var temp = number << 6;
                number ^= temp;
                number &= 16777215;

                temp = number >> 5; // /32
                number ^= temp;
                number &= 16777215;

                temp = number << 11; // *2048
                number ^= temp;
                number &= 16777215;
                
            }
            total += number;

        }
        return total;
    }
    [Benchmark, Arguments(false)]
    public long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day22>(sample);
        
        Dictionary<PriceChanges, int> totalBananas = [];
        
        while (!file.EndOfStream)
        {
            var number = long.Parse(file.ReadLine()!);
            var lastPrice = 0;
            
            HashSet<PriceChanges> visited = [];    
            PriceChanges changes = new PriceChanges(0,0,0,0);
            
            for (var i = 0; i < 2000; i++)
            {
                var temp = number << 6; // *64
                number ^= temp;
                number &= 16777215;

                temp = number >> 5; // /32
                number ^= temp;
                number &= 16777215;

                temp = number << 11; // *2048
                number ^= temp;
                number &= 16777215;

                var price = (int) (number % 10);
                changes = new PriceChanges(
                    Change1: changes.Change2,
                    Change2: changes.Change3,
                    Change3: changes.Change4,
                    Change4: price - lastPrice);
                
                
                if (i >= 4 && visited.Add(changes))
                {
                    if (!totalBananas.TryAdd(changes, price))
                        totalBananas[changes] += price;
                }
                
                lastPrice = price;
            }

        }
        return totalBananas.Values.Max();
    }
    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record PriceChanges(int Change1, int Change2, int Change3, int Change4);
}