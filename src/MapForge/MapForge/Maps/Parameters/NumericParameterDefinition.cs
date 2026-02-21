namespace MapForge.Maps.Parameters
{
    public class NumericParameterDefinition<T> : ParameterDefinitionBase<T> where T : struct, IComparable<T>
    {
        public T? Min { get; }
        public T? Max { get; }

        public NumericParameterDefinition(string name, T defaultValue, T? min = null, T? max = null) : base(name, defaultValue)
        {
            Min = min;
            Max = max;
        }
        public override bool IsValid(T value)
        {
            if (Min.HasValue && value.CompareTo(Min.Value) < 0) return false;
            if (Max.HasValue && value.CompareTo(Max.Value) > 0) return false;
            return true;
        }
    }
}
