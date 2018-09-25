using System.Collections.Generic;

namespace LoxSharp
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

        public class Block : Stmt
        {
            public Block(IEnumerable<Stmt> @statements)
            {
                Statements = @statements;
            }

            public IEnumerable<Stmt> Statements { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
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

        public class If : Stmt
        {
            public If(Expr @condition, Stmt @thenbranch, Stmt @elsebranch)
            {
                Condition = @condition;
                ThenBranch = @thenbranch;
                ElseBranch = @elsebranch;
            }

            public Expr Condition { get; }
            public Stmt ThenBranch { get; }
            public Stmt ElseBranch { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
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
            public Var(Token @name, Expr @initializer)
            {
                Name = @name;
                Initializer = @initializer;
            }

            public Token Name { get; }
            public Expr Initializer { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

    }

    public interface IStmtVisitor<T>
    {
        T VisitBlockStmt(Stmt.Block stmt);
        T VisitExpressionStmt(Stmt.Expression stmt);
        T VisitIfStmt(Stmt.If stmt);
        T VisitPrintStmt(Stmt.Print stmt);
        T VisitVarStmt(Stmt.Var stmt);
    }
}
