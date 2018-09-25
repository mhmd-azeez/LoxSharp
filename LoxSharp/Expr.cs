namespace LoxSharp
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);

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
        T VisitBinaryExpr(Expr.Binary expr);
        T VisitUnaryExpr(Expr.Unary expr);
        T VisitLiteralExpr(Expr.Literal expr);
        T VisitGroupingExpr(Expr.Grouping expr);
        T VisitVariableExpr(Expr.Variable expr);
    }
}
