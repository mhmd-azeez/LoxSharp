using System;

namespace LoxSharp.Repl
{
    class Program
    {
        static bool _hadError = false;
        static bool _hadRuntimeError = false;
        static readonly IOutput _output = new ConsoleOutput();
        static readonly Interpreter _interpreter = new Interpreter(_output);

        static int Main(string[] args)
        {
            int exitCode = 0;

            RunPrompt();

            return exitCode;
        }

        static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();
                if (!line.TrimEnd().EndsWith(";"))
                    line = line + ";";

                Run(line, printExpressions: true);

                _hadError = false;
            }
        }

        static void Run(string source, bool printExpressions = false)
        {
            Scanner scanner = new Scanner(source, _output);
            var tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens, _output);
            var statements = parser.Parse();

            // Stop if there was a syntax error.
            if (_hadError) return;

            if (printExpressions && statements.Count == 1 && statements[0] is Stmt.Expression exprStmt)
            {
                statements[0] = new Stmt.Print(exprStmt.Expr);
            }

            _interpreter.Interpret(statements);
        }
    }
}
