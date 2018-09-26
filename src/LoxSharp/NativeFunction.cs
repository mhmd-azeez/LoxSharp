using System;
using System.Collections;
using System.Collections.Generic;

namespace LoxSharp
{
    public class NativeFunction : ICallable
    {
        public NativeFunction(string name, Func<Interpreter, IReadOnlyList<object>, object> action, int arity = 0)
        {
            Name = name;
            Arity = arity;
            _action = action;
        }

        public string Name { get; }
        private readonly Func<Interpreter, IReadOnlyList<object>, object> _action;
        public int Arity { get; }

        public object Call(Interpreter interpreter, params object[] arguments)
        {
            return _action(interpreter, arguments);
        }

        public override string ToString()
        {
            return $"<native fn {Name}>";
        }
    }
}
