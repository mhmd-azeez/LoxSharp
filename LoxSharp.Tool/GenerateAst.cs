using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoxSharp.Tool
{
    class GenerateAst
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: generate_ast [output directory]");
                Environment.Exit(1);
            }

            var outputDir = args[0];

            DefineAst(outputDir, "Expr", new List<string>
            {
                "Assign    : Token Name, Expr Value",
                "Binary    : Expr Left, Token Operator, Expr Right",
                "Unary     : Token Operator, Expr Right",
                "Literal   : object Value",
                "Logical   : Expr Left, Token Operator, Expr Right",
                "Grouping  : Expr Expression",
                "Call      : Expr Callee, Token Parenthesis, IEnumerable<Expr> Arguments",
                "Variable  : Token Name",
            });

            DefineAst(outputDir, "Stmt", new List<string>
            {
                "Block          : IEnumerable<Stmt> Statements",
                "Break          : ",
                "Expression     : Expr Expr",
                "If             : Expr Condition, Stmt ThenBranch, Stmt ElseBranch",
                "Print          : Expr Expr",
                "Var            : Token Name, Expr Initializer",
                "While          : Expr Condition, Stmt Body",
            });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> children)
        {
            var path = Path.Combine(outputDir, baseName + ".cs");

            using (var writer = File.CreateText(path))
            {
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();

                writer.WriteLine("namespace LoxSharp");
                writer.WriteLine("{");

                writer.WriteLine($"    public abstract class {baseName}");
                writer.WriteLine("    {");
                writer.WriteLine($"        public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");

                foreach (var child in children)
                {
                    writer.WriteLine();

                    var parts = child.Split(":");

                    var className = parts[0].Trim();
                    var fields = parts[1].Trim();
                    DefineClass(writer, baseName, className, fields);
                }

                writer.WriteLine();
                writer.WriteLine("    }");
                DefineVisitor(writer, baseName, children);
                writer.WriteLine("}");
            }

        }

        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> children)
        {
            writer.WriteLine();
            writer.WriteLine($"    public interface I{baseName}Visitor<T>");
            writer.WriteLine("    {");

            foreach(var child in children)
            {
                var name = child.Split(":")[0].Trim();

                writer.WriteLine($"        T Visit{name}{baseName}({baseName}.{name} {baseName.ToLower()});");
            }

            writer.WriteLine("    }");
        }

        private static void DefineClass(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"        public class {className} : {baseName}");
            writer.WriteLine("        {");

            var fields = fieldList.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            var parameters = fields.Select(f =>
            {
                var parts = f.Split(" ");
                return $"{parts[0]} @{parts[1].ToLower()}";
            }).ToArray();

            writer.WriteLine($"            public {className}({string.Join(", ", parameters)})");
            writer.WriteLine("            {");

            foreach (var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine($"                {name} = @{name.ToLower()};");
            }

            writer.WriteLine("            }");

            writer.WriteLine();

            foreach (var field in fields)
            {
                writer.WriteLine($"            public {field} {{ get; }}");
            }

            writer.WriteLine();

            writer.WriteLine($"            public override T Accept<T>(I{baseName}Visitor<T> visitor)");
            writer.WriteLine("            {");
            writer.WriteLine($"                return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("            }");

            writer.WriteLine("        }");
        }
    }
}
