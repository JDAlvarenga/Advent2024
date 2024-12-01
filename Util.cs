namespace Advent2024;

public static class Util
{
    public static StreamReader GetInputStream<T>(bool sample = false)
    {
        var path = Path.Combine(
            "input",
            sample ? "samples" : string.Empty,
            $"{typeof(T).Name.ToLower()}.txt"
        );
        return new StreamReader(path);
    }
    
     
     // public static int template()
     // {
     //     string? line = null;
     //     int total = 0;
     //     using var file = Util.GetInputStream<Day01>();
     //     while ((line = file.ReadLine()) is not null)
     //     {
     //
     //     }
     //     return total;
     // }
     
}