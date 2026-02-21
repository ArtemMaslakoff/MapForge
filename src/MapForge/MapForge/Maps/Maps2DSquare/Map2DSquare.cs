using MapForge.Maps.Core;

namespace MapForge.Maps.Maps2DSquare
{
    public class Map2DSquare : IMap
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }

        public IReadOnlyList<IParameterDefinition> ParameterDefinitions { get; }

        private readonly Cell2DSquare[,] _cells;

        public Map2DSquare(string name, int width, int height, IReadOnlyList<IParameterDefinition> parameterDefinitions)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Width and height must be positive.");
            }
            Width = width;
            Height = height;
            ParameterDefinitions = parameterDefinitions ?? throw new ArgumentNullException(nameof(parameterDefinitions));
            _cells = new Cell2DSquare[Width, Height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cellValue = new CellValue();
                    cellValue.InitFromDefaults(ParameterDefinitions);
                    _cells[x, y] = new Cell2DSquare(x, y, cellValue, parameterDefinitions);
                }
            }
        }

        public IEnumerable<ICell> GetAllCells()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return _cells[x, y];
                }
            }
        }

        public ICell GetCell(int[] coordinates)
        {
            if (coordinates == null || coordinates.Length < 2)
            {
                throw new ArgumentException("Need at least [x, y].", nameof(coordinates));
            }
            int x = coordinates[0];
            int y = coordinates[1];
            CheckBounds(x, y);
            return _cells[x, y];
        }

        public int GetDimensionCount() => 2;
        public int GetLength(int dimension) => dimension == 0 ? Width : Height;

        private void CheckBounds(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Coordinates ({x}, {y}) are out of bounds.");
            }
        }
    }
}
