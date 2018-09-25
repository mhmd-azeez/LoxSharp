using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;

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
            if (Match(TokenType.Print))
                return PrintStatement();

            return ExpressionStatement();
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
            return Equality();
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

            while(Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
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

            while(Match(TokenType.Minus, TokenType.Plus))
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

            while(Match(TokenType.Slash, TokenType.Star))
            {
                var operater = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, operater, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            while(Match(TokenType.Bang, TokenType.Minus))
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

            while(!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon) return;

                switch(Peek().Type)
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
