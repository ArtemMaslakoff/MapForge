namespace MapForge.Maps.Core
{
    public interface IMap
    {
        string Name { get; }

        IReadOnlyList<IParameterDefinition> ParameterDefinitions { get; }
        ICell GetCell(int[] coordinates);
        IEnumerable<ICell> GetAllCells();
        int GetDimensionCount();
        int GetLength(int dimension);
    }
}
