using System.Collections;

namespace LoxSharp
{
    public interface ICallable
    {
        object Call(Interpreter interpreter, params object[] arguments);
        int Arity { get; }
    }
}