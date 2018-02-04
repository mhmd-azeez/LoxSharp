namespace LoxSharp
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public class Expression : Stmt
    {
        public Expression(Expr @expr)
        {
            Expr = @expr;
        }

        public Expr Expr { get; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Print(Expr @expr)
        {
            Expr = @expr;
        }

        public Expr Expr { get; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public interface IStmtVisitor<T>
    {
        T VisitExpressionStmt(Expression stmt);
        T VisitPrintStmt(Print stmt);
    }
}
