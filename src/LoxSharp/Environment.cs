using System;
using System.Collections.Generic;

namespace LoxSharp
{
    public class Environment
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();
        private readonly Environment _enclosing;

        public Environment()
        { }

        public Environment(Environment enclosing)
        {
            _enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            _values[name] = value;
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
                return _values[name.Lexeme];

            if (_enclosing != null)
                return _enclosing.Get(name);

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
