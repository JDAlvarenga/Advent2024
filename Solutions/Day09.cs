namespace Advent2024.Solutions;

public class Day09
{
    public static long Part1(bool sample = false)
    {
        using var file = Util.GetInputStream<Day09>(sample);
        
        List<int> blockGroups = []; // sizes of used/empty blocks
        List<int> ids = []; // blocks with their ids
        var currentId = 0;
        
        var notEmpty = true;
        while (!file.EndOfStream)
        {
            // '0' == 48
            var n = file.Read() - 48;
            blockGroups.Add(n);

            if (notEmpty)
            {
                for (var i = 0; i < n; i++)
                {
                    ids.Add(currentId);
                }

                currentId++;

            }
            notEmpty = !notEmpty;
        }
        
        long checksum = 0;
        notEmpty = true;
        
        var pos = 0;
        // foreach block, if empty, take from the end of ids list, else from the start
        foreach (var group in blockGroups)
        {
            for (var i = 0; i < group; i++)
            {
                if (ids.Count == 0) return checksum;
                
                var idx = notEmpty ? 0 : ids.Count - 1;
                var id = ids[idx];
                ids.RemoveAt(idx);
                
                checksum += id * pos;
                pos++;
            }
            notEmpty = !notEmpty;
        }
        return checksum;
    }

    private record BlockGroup(int Start, int End, int Id)
    {
        public long Sum => Enumerable.Range(Start, Size).Aggregate<int, long>(0, (acc, pos) => pos * Id + acc);
        public int Size => End - Start + 1;
    }
    public static long Part2(bool sample = false)
    {
        using var file = Util.GetInputStream<Day09>(sample);
        
        // A copy of input. saves sizes of used/empty blocks
        List<BlockGroup> blockGroups = [];
        List<BlockGroup> emptyGroups = [];
        
        var currentId = 0;
        var currentPos = 0;
        var isEmpty = false;
        
        while (!file.EndOfStream)
        {
            // '0' == 48
            var n = file.Read() - 48;

            if (isEmpty)
            {
                if (n != 0)
                    emptyGroups.Add(new BlockGroup(Start: currentPos, End: currentPos + n - 1, Id: 0));
            }
            else
            {
                blockGroups.Add(new BlockGroup(Start: currentPos, End: currentPos + n - 1, Id: currentId));
                currentId++;
            }

            currentPos += n;
            isEmpty = !isEmpty;
        }

        // try to fit files from the end...
        for (var i = blockGroups.Count - 1; i >= 0; i--)
        {
            var gSize = blockGroups[i].Size;
            // ...into empty block from the start
            for (var j = 0; j < emptyGroups.Count; j++)
            {
                // no empty blocks on the left of this file
                if (blockGroups[i].End < emptyGroups[j].Start) break;
                var emptySize = emptyGroups[j].Size;
                // file fits perfectly
                if (gSize == emptySize)
                {
                    blockGroups[i] = emptyGroups[j] with { Id = blockGroups[i].Id };
                    emptyGroups.RemoveAt(j);
                    break;
                }
                // file fits with extra space
                if (gSize < emptySize)
                {
                    blockGroups[i] = emptyGroups[j] with { End = emptyGroups[j].Start + gSize - 1, Id = blockGroups[i].Id };
                    emptyGroups[j] = emptyGroups[j] with { Start = blockGroups[i].End + 1 };
                    break;
                }
                
            }
        }

        return blockGroups.Aggregate<BlockGroup, long>(0, (current, group) => current + group.Sum);
    }

    
}