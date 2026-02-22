using MapForge.MapFabrics;
using MapForge.Maps.Core;
using MapForge.Maps.Maps2DSquare;
using MapForge.Maps.Parameters;

internal class Program
{
    private static void Main(string[] args)
    {
        var parameters = new List<IParameterDefinition>
        {
            new NumericParameterDefinition<int>("Height", 50, 0, 100),
            new BooleanParameterDefinition("IsWater", false),
        };

        var map = new Map2DSquare("Main", 13, 13, parameters);

        MapFillFabric.FillBand<bool>(map, "IsWater", true, 1, 2, 5);
        MapFillFabric.FillInterior<int>(map, "Height", 77, 2);
        // Опционально: явно заполнить все ячейки дефолтами (если добавишь MapFillFabric.FullDefaultFill)
        // MapFillFabric.FullDefaultFill(map);

        PrintMapToConsole(map);
    }

    /// <summary>Выводит 2D карту в консоль: одна ячейка = один символ или короткий блок.</summary>
    private static void PrintMapToConsole(IMap map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ICell cell = map.GetCell([x, y]);
                string representation = FormatCell(cell);
                Console.Write(representation);
            }
            Console.WriteLine();
        }
    }

    /// <summary>Форматирует одну ячейку для вывода (можно подстроить под свои параметры).</summary>
    private static string FormatCell(ICell cell)
    {
        object? height = cell.GetValue("Height");
        object? isWater = cell.GetValue("IsWater");
        bool water = isWater is true;
        int h = height is int i ? i : 0;

        // Вариант 1: один символ — вода '~' или высота цифрой
        //if (water) return "~ ";
        //return h + " ";

        // Вариант 2: блок вида [H:0 W:0]
        return $"[{h},{(water ? 1 : 0)}] ";
    }
}