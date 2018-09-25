using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    public class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expression)
        {
            return expression.Accept(this);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return $"{expr.Name} <= {expr.Value.Accept(this)}";
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.Name.Lexeme;
        }

        private string Parenthesize(string name, params Expr[] expressions)
        {
            var builder = new StringBuilder();

            builder.Append($"({name}");

            foreach (var expr in expressions)
            {
                builder.Append($" {expr.Accept(this)}");
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
