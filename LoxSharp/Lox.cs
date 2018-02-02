using System;
using System.IO;

namespace LoxSharp
{
    class Lox
    {
        static bool _hadError = false;

        static void Main(string[] args)
        {
            var expression = new Binary(
                new Unary(new Token(TokenType.Minus, "-", null, 1), new Literal("11")),
                new Token(TokenType.Star, "*", null, 1),
                new Grouping(new Literal("45.62"))
                );

            Console.WriteLine(new AstPrinter().Print(expression));

            //if (args.Length > 1)
            //{
            //    Console.WriteLine("Usage: loxsharp [script]");
            //}
            //else if (args.Length == 1)
            //{
            //    RunFile(args[0]);
            //}
            //else
            //{
            //    RunPrompt();
            //}
        }

        static void RunFile(string path)
        {
            var code = File.ReadAllText(path);
            Run(code);

            if (_hadError)
                Environment.Exit(65);
        }

        static void RunPrompt()
        {
            while(true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());

                _hadError = false;
            }
        }

        static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach(var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[Line {line:N0}] Error{where}: {message}");
            _hadError = true;
        }
    }
}
