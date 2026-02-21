namespace MapForge.Maps.Parameters
{
    public class BooleanParameterDefinition : ParameterDefinitionBase<bool>
    {
        public BooleanParameterDefinition(string name, bool defaultValue = false) : base(name, defaultValue) { }

        public override bool IsValid(bool value)
        {
            return true;
        }
    }
}
