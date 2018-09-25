using System;
using System.Collections;
using System.Linq;

namespace LoxSharp
{
    public class Function : ICallable
    {
        public Function(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        private readonly Stmt.Function _declaration;
        public int Arity => _declaration.Params.Count();

        public object Call(Interpreter interpreter, params object[] arguments)
        {
            var environment = new Environment(interpreter.Globals);

            for (int i = 0; i < arguments.Length; i++)
            {
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch(Return r)
            {
                return r.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
