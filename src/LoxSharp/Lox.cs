using System;
using System.IO;

namespace LoxSharp
{
    class Lox
    {
        static bool _hadError = false;
        static bool _hadRuntimeError = false;
        static readonly IOutput _output = new ConsoleOutput();
        static readonly Interpreter _interpreter = new Interpreter(_output);

        static int Main(string[] args)
        {
            int exitCode = 0;

            if (args.Length > 2)
            {
                Console.WriteLine("Usage: loxsharp script [stay]");
            }
            else if (args.Length > 0)
            {
                exitCode = RunFile(args[0]);

                if (args.Length == 2 && string.Equals(args[1], "stay", StringComparison.InvariantCultureIgnoreCase))
                    Console.ReadLine();
            }
            else
            {
                RunPrompt();
            }

            return exitCode;
        }

        public static string RuntimeError(RuntimeException ex)
        {
            _hadRuntimeError = true;
            return $"{ex.Message}\n[line {ex.Token.Line:N0}]";
        }

        static int RunFile(string path)
        {
            var code = File.ReadAllText(path);
            Run(code);

            if (_hadError)
                return 65;
            else if (_hadRuntimeError)
                return 70;
            else
                return 0;
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

        public static string Error(int line, string message)
        {
            return Report(line, "", message);
        }

        static string Report(int line, string where, string message)
        {
            _hadError = true;
            return $"[Line {line:N0}] Error{where}: {message}";
        }

        public static string Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
                return Report(token.Line, " at end", message);
            else
                return Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }
}
