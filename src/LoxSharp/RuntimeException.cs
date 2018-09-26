using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp
{
    public class RuntimeException : SystemException
    {
        public RuntimeException(Token token, string message) : base(message)
        {
            Token = token;
        }

        public Token Token { get; }
    }

    public class Return : RuntimeException
    {
        public Return(object value) : base (null, null)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}
