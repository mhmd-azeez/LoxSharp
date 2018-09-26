using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoxSharp.Demo
{
    public static class Utils
    {
        public static string Interpret(string source)
        {
            try
            {
                var output = new StringBuilderOutput();
                Console.WriteLine(source);

                var scanner = new Scanner(source, output);
                var tokens = scanner.ScanTokens();

                var parser = new Parser(tokens, output);
                var statements = parser.Parse();

                var interpreter = new Interpreter(output);
                interpreter.Interpret(statements);

                return output.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
