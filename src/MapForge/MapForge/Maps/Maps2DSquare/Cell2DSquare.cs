using MapForge.Maps.Core;

namespace MapForge.Maps.Maps2DSquare
{
    public class Cell2DSquare : ICell
    {
        public int X { get; }
        public int Y { get; }
        
        public IReadOnlyList<IParameterDefinition> ParameterDefinitions { get; }

        public CellValue Value { get; }
        public Cell2DSquare(int x, int y, CellValue value, IReadOnlyList<IParameterDefinition> parameterDefinitions)
        {
            X = x;
            Y = y;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ParameterDefinitions = parameterDefinitions ?? throw new ArgumentNullException(nameof(parameterDefinitions));
        }


        public int[] GetCoordinates() => [X, Y];

        public object? GetValue(string parameterName) => Value.Get(parameterName);

        public void SetValue(string parameterName, object? value)
        {
            var parameter = GetParameter(parameterName);
            if (parameter == null)
            {
                throw new ArgumentException($"Unknown parameter: '{parameterName}'.", nameof(parameterName));
            }
            if (!parameter.IsValidObject(value))
            {
                throw new ArgumentException($"Value is not valid for parameter '{parameterName}'.", nameof(value));
            }
            Value.Set(parameterName, value);
        }

        private IParameterDefinition? GetParameter(string name)
        {
            foreach (var p in ParameterDefinitions)
                if (p.Name == name) return p;
            return null;
        }
    }
}
