namespace LoxSharp
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

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

        public class Var : Stmt
        {
            public Var(Token @name, Expr @intializer)
            {
                Name = @name;
                Intializer = @intializer;
            }

            public Token Name { get; }
            public Expr Intializer { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

    }

    public interface IStmtVisitor<T>
    {
        T VisitExpressionStmt(Stmt.Expression stmt);
        T VisitPrintStmt(Stmt.Print stmt);
        T VisitVarStmt(Stmt.Var stmt);
    }
}
