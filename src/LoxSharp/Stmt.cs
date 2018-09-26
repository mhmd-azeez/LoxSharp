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

        public class Break : Stmt
        {
            public Break()
            {
            }


            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitBreakStmt(this);
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

        public class Function : Stmt
        {
            public Function(Token @name, IReadOnlyList<Token> @params, IReadOnlyList<Stmt> @body)
            {
                Name = @name;
                Params = @params;
                Body = @body;
            }

            public Token Name { get; }
            public IReadOnlyList<Token> Params { get; }
            public IReadOnlyList<Stmt> Body { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
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

        public class Return : Stmt
        {
            public Return(Token @keyword, Expr @value)
            {
                Keyword = @keyword;
                Value = @value;
            }

            public Token Keyword { get; }
            public Expr Value { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
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

        public class While : Stmt
        {
            public While(Expr @condition, Stmt @body)
            {
                Condition = @condition;
                Body = @body;
            }

            public Expr Condition { get; }
            public Stmt Body { get; }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }

    }

    public interface IStmtVisitor<T>
    {
        T VisitBlockStmt(Stmt.Block stmt);
        T VisitBreakStmt(Stmt.Break stmt);
        T VisitExpressionStmt(Stmt.Expression stmt);
        T VisitFunctionStmt(Stmt.Function stmt);
        T VisitIfStmt(Stmt.If stmt);
        T VisitPrintStmt(Stmt.Print stmt);
        T VisitReturnStmt(Stmt.Return stmt);
        T VisitVarStmt(Stmt.Var stmt);
        T VisitWhileStmt(Stmt.While stmt);
    }
}
