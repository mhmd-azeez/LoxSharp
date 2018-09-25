using System.Collections.Generic;

namespace LoxSharp
{
    public class Environment
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();

        public void Define(string name, object value)
        {
            _values[name] = value;
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
                return _values[name.Lexeme];

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
