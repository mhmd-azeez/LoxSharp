using System.Text;

namespace LoxSharp.Demo
{
    public class StringBuilderOutput : IOutput
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void WriteError(string text)
        {
            _builder.AppendLine(text);
        }

        public void WriteLine(string text)
        {
            _builder.AppendLine(text);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
