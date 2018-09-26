using System.Collections.Generic;

namespace LoxSharp
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);

        public class Assign : Expr
        {
            public Assign(Token @name, Expr @value)
            {
                Name = @name;
                Value = @value;
            }

            public Token Name { get; }
            public Expr Value { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

        public class Binary : Expr
        {
            public Binary(Expr @left, Token @operator, Expr @right)
            {
                Left = @left;
                Operator = @operator;
                Right = @right;
            }

            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Unary : Expr
        {
            public Unary(Token @operator, Expr @right)
            {
                Operator = @operator;
                Right = @right;
            }

            public Token Operator { get; }
            public Expr Right { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Literal : Expr
        {
            public Literal(object @value)
            {
                Value = @value;
            }

            public object Value { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Logical : Expr
        {
            public Logical(Expr @left, Token @operator, Expr @right)
            {
                Left = @left;
                Operator = @operator;
                Right = @right;
            }

            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr @expression)
            {
                Expression = @expression;
            }

            public Expr Expression { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Call : Expr
        {
            public Call(Expr @callee, Token @parenthesis, IEnumerable<Expr> @arguments)
            {
                Callee = @callee;
                Parenthesis = @parenthesis;
                Arguments = @arguments;
            }

            public Expr Callee { get; }
            public Token Parenthesis { get; }
            public IEnumerable<Expr> Arguments { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        public class Variable : Expr
        {
            public Variable(Token @name)
            {
                Name = @name;
            }

            public Token Name { get; }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

    }

    public interface IExprVisitor<T>
    {
        T VisitAssignExpr(Expr.Assign expr);
        T VisitBinaryExpr(Expr.Binary expr);
        T VisitUnaryExpr(Expr.Unary expr);
        T VisitLiteralExpr(Expr.Literal expr);
        T VisitLogicalExpr(Expr.Logical expr);
        T VisitGroupingExpr(Expr.Grouping expr);
        T VisitCallExpr(Expr.Call expr);
        T VisitVariableExpr(Expr.Variable expr);
    }
}
