using System.Collections.ObjectModel;

namespace MapForge.Maps.Core
{
    public class CellValue
    {
        private readonly Dictionary<string, object?> _values = new();

        public object? Get(string parameterName)
        {
            _values.TryGetValue(parameterName, out var parameterValue);
            return parameterValue;
        }

        public void Set(string parameterName, object? value) => _values[parameterName] = value;

        public bool HasParameter(string parameterName) => _values.ContainsKey(parameterName);

        /// <summary>Возвращает неизменяемое представление значений (изменение через Set по-прежнему возможно только из ячейки с валидацией).</summary>
        public IReadOnlyDictionary<string, object?> AsReadOnly() => new ReadOnlyDictionary<string, object?>(_values);

        public void InitFromDefaults(IReadOnlyList<IParameterDefinition> parameterDefinitions)
        {
            foreach (var p in parameterDefinitions)
            {
                _values[p.Name] = p.DefaultValue;
            }
        }
    }
}
