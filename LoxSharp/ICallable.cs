using System.Collections;

namespace LoxSharp
{
    public interface ICallable
    {
        object Call(Interpreter interpreter, IEnumerable arguments);
        int Arity { get; }
    }
}