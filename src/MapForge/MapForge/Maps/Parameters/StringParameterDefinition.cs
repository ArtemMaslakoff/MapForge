namespace MapForge.Maps.Parameters
{
    public class StringParameterDefinition : ParameterDefinitionBase<string>
    {
        public IReadOnlyList<string>? AllowedValues { get; }

        public StringParameterDefinition(string name, string defaultValue, IReadOnlyList<string>? allowedValues = null) : base(name, defaultValue)
        {
            AllowedValues = allowedValues;

            if (AllowedValues != null && AllowedValues.Count > 0 && !AllowedValues.Contains(defaultValue, StringComparer.Ordinal))
            {
                throw new ArgumentException($"Default value \"{defaultValue}\" must be one of the allowed values.", nameof(defaultValue));
            }
        }
        public override bool IsValid(string value)
        {
            if (AllowedValues == null || AllowedValues.Count == 0) return true;
            return AllowedValues.Contains(value, StringComparer.Ordinal);
        }
    }
}
