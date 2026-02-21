using MapForge.Maps.Core;

namespace MapForge.Maps.Parameters
{
    public abstract class ParameterDefinitionBase<T> : IParameterDefinition<T>
    {
        public string Name { get; }
        public T DefaultValue { get; }
        object IParameterDefinition.DefaultValue => DefaultValue!;

        protected ParameterDefinitionBase(string name, T defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public abstract bool IsValid(T value);

        public bool IsValidObject(object value)
        {
            return value is T t && IsValid(t);
        }
    }
}
