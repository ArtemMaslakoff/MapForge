namespace MapForge.Maps.Core
{
    public interface IParameterDefinition
    {
        string Name { get; }
        object DefaultValue { get; }
        bool IsValidObject(object value);
    }
    public interface IParameterDefinition<T> : IParameterDefinition
    {
        new T DefaultValue { get; }
        bool IsValid(T value);
    }
}
