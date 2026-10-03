using System.Text.Json;

namespace Minesweeper.Data.Models;

public static class MineMapCodec
{
    public static string Encode(bool[,] mineMap)
    {
        var columns = new List<List<bool>>(mineMap.GetLength(0));
        for (int x = 0; x < mineMap.GetLength(0); x++)
        {
            var column = new List<bool>(mineMap.GetLength(1));
            for (int y = 0; y < mineMap.GetLength(1); y++)
                column.Add(mineMap[x, y]);
            columns.Add(column);
        }

        return JsonSerializer.Serialize(columns);
    }

    public static bool[,]? Decode(string json)
    {
        try
        {
            var columns = JsonSerializer.Deserialize<List<List<bool>>>(json);
            if (columns == null || columns.Count == 0 || columns.Any(column => column == null || column.Count == 0))
                return null;

            int height = columns[0].Count;
            if (columns.Any(column => column.Count != height))
                return null;

            var map = new bool[columns.Count, height];
            for (int x = 0; x < columns.Count; x++)
                for (int y = 0; y < height; y++)
                    map[x, y] = columns[x][y];

            return map;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
