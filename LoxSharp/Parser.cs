using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private int _loopDepth = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var list = new List<Stmt>();
            try
            {
                while (!IsAtEnd())
                {
                    list.Add(Declaration());
                }
            }
            catch (ParseException)
            {
                return null;
            }

            return list;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.Var))
                    return VarDeclaration();

                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.Equal))
                initializer = Expression();

            Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");

            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(TokenType.Break))
                return BreakStatement();

            if (Match(TokenType.For))
                return ForStatement();

            if (Match(TokenType.If))
                return IfStatement();

            if (Match(TokenType.Print))
                return PrintStatement();

            if (Match(TokenType.While))
                return WhileStatement();

            if (Match(TokenType.LeftBrace))
                return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        private Stmt BreakStatement()
        {
            if (_loopDepth == 0)
            {
                Error(Previous(), "Must be inside a loop to use 'break'.");
            }

            Consume(TokenType.Semicolon, "Expect ';' after 'break'.");

            return new Stmt.Break();
        }

        private Stmt ForStatement()
        {
            // TODO: for loop: condition can't be nothing
            try
            {
                _loopDepth++;

                Consume(TokenType.LeftParenthesis, "Expect '(' after 'for'.");

                Stmt initializer;
                if (Match(TokenType.Semicolon))
                    initializer = null;
                else if (Match(TokenType.Var))
                    initializer = VarDeclaration();
                else
                    initializer = ExpressionStatement();

                Expr condition = null;
                if (!Match(TokenType.Semicolon))
                {
                    condition = Expression();
                    Consume(TokenType.Semicolon, "Expect ';' after loop condition.");
                }

                Expr increment = null;
                if (!Match(TokenType.Semicolon))
                    increment = Expression();
                Consume(TokenType.RightParenthesis, "Expect ')' after for clauses");

                var body = Statement();

                if (increment != null)
                {
                    body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
                }

                body = new Stmt.While(condition ?? new Expr.Literal(true), body);

                if (initializer != null)
                {
                    body = new Stmt.Block(new List<Stmt> { initializer, body });
                }

                return body;
            }
            finally
            {
                _loopDepth--;
            }
        }

        private Stmt WhileStatement()
        {
            try
            {
                _loopDepth++;

                Consume(TokenType.LeftParenthesis, "Expect '(' after 'while'.");
                Expr condition = Expression();
                Consume(TokenType.RightParenthesis, "Expect ')' after condition.");
                Stmt body = Statement();

                return new Stmt.While(condition, body);
            }
            finally
            {
                _loopDepth--;
            }
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LeftParenthesis, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RightParenthesis, "Expect ')' after if condition.");

            var thenBranch = Statement();

            Stmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private IEnumerable<Stmt> Block()
        {
            var statements = new List<Stmt>();

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RightBrace, "Expect '}' after block.");
            return statements;
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after value.");
            return new Stmt.Expression(value);
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after value.");
            return new Stmt.Print(value);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(TokenType.Equal))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Expr.Variable variable)
                {
                    var name = variable.Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.EqualEqual, TokenType.BangEqual))
            {
                var operater = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, operater, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Addition();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var operater = Previous();
                var right = Addition();
                expr = new Expr.Binary(expr, operater, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            var expr = Multiplication();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                var operater = Previous();
                var right = Multiplication();
                expr = new Expr.Binary(expr, operater, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            var expr = Unary();

            while (Match(TokenType.Slash, TokenType.Star))
            {
                var operater = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, operater, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            while (Match(TokenType.Bang, TokenType.Minus))
            {
                var operater = Previous();
                var right = Unary();

                return new Expr.Unary(operater, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.False))
                return new Expr.Literal(false);

            if (Match(TokenType.True))
                return new Expr.Literal(true);

            if (Match(TokenType.String, TokenType.Number))
                return new Expr.Literal(Previous().Literal);

            if (Match(TokenType.Identifier))
                return new Expr.Variable(Previous());

            if (Match(TokenType.LeftParenthesis))
            {
                var expr = Expression();
                Consume(TokenType.RightParenthesis, "Expcted ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression.");
        }

        private Token Consume(TokenType token, string message)
        {
            if (Match(token)) return Previous();

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message)
        {
            Lox.Error(token, message);

            return new ParseException();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon) return;

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] tokenTypes)
        {
            foreach (var type in tokenTypes)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return _tokens[_current].Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
