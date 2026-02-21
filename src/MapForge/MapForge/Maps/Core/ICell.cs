namespace MapForge.Maps.Core
{
    public interface ICell
    {
        int[] GetCoordinates();
        object? GetValue(string parameterName);
        void SetValue(string parameterName, object? value);
    }
}
