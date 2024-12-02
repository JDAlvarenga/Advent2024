namespace Advent2024.Solutions;

public class Day02
{
    public static int Part1(bool sample = false)
    {
        string? line = null;
        var totalSafe = 0;
        
        using var file = Util.GetInputStream<Day02>(sample);
        while ((line = file.ReadLine()) != null)
        {
            var report = line.Split().Select(int.Parse).ToList();

            if (IsSafeReport(report)) totalSafe++;
        }
        
        return totalSafe;
    }
    
    
    public static int Part2(bool sample = false)
    {
        string? line = null;
        var totalSafe = 0;
        
        using var file = Util.GetInputStream<Day02>(sample);
        while ((line = file.ReadLine()) != null)
        {
            var report = line.Split().Select(int.Parse).ToList();

            if (IsSafeReport(report, 1)) totalSafe++;

        }
        
        return totalSafe;
    }

    private static bool IsSafeReport(List<int> report, int unsafeLevels = 0)
    {

        bool? lastIncreasing = null;

        

        
        for (var i = 0; i < report.Count-1; i++)
        {
            int diff = report[i + 1] - report[i];
            
            switch (diff)
            {
                case var _ when lastIncreasing is null && Math.Abs(diff) is not 0 and <= 3:
                case var _ when lastIncreasing is true && diff is >= 1 and <= 3:
                case var _ when lastIncreasing is false && diff is >= -3 and <= -1:
                    lastIncreasing = int.IsPositive(diff);
                    break;
                // case var _ when lastIncreasing is false && diff is >= 1 and <= 3:
                // case var _ when lastIncreasing is true && diff is >= -3 and <= -1:
                // case 0:
                default:
                        
                    if (unsafeLevels <= 0) return false;
                    
                    
                    // try removing the current level
                    var altReport = report.ToList();
                    altReport.RemoveAt(i);
                    if (IsSafeReport(altReport, unsafeLevels - 1)) return true;
            
                    // try removing the next level
                    altReport = report.ToList();
                    altReport.RemoveAt(i+1);
                    if (IsSafeReport(altReport, unsafeLevels - 1)) return true;
                    
                    // try removing previous level
                    if (i <= 0) return false;
                    altReport = report.ToList();
                    altReport.RemoveAt(i - 1);
                    return IsSafeReport(altReport, unsafeLevels - 1);
            }
        }
        // no unsafe levels
        return true;

    }
}