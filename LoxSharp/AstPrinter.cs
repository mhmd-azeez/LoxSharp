using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expression)
        {
            return expression.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] expressions)
        {
            var builder = new StringBuilder();

            builder.Append($"({name}");

            foreach(var expr in expressions)
            {
                builder.Append($" {expr.Accept(this)}");
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
