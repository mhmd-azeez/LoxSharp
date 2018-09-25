using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    class Scanner
    {
        readonly string _source;
        readonly List<Token> _tokens = new List<Token>();
        int _start = 0;
        int _current = 0;
        int _line = 1;

        static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "and",    TokenType.And },
            { "or",     TokenType.Or },
            { "if",     TokenType.If },
            { "else",   TokenType.Else },
            { "class",  TokenType.Class },
            { "true",   TokenType.True },
            { "false",  TokenType.False },
            { "for",    TokenType.For },
            { "while",  TokenType.While },
            { "fun",    TokenType.Fun },
            { "nil",    TokenType.Nil },
            { "print",  TokenType.Print },
            { "return", TokenType.Return },
            { "this",   TokenType.This },
            { "super",  TokenType.Super },
            { "var",    TokenType.Var },
            { "break",  TokenType.Break },
        };

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case '*': AddToken(TokenType.Star); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '%': AddToken(TokenType.Percent); break;

                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;

                case '/':
                    if (Match('/'))
                    {
                        // It's a comment, ignore the rest of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    _line++;
                    break;

                case '"': ScanString(); break;

                default:
                    if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ScanIndentifier();
                    }
                    else
                    {
                        Lox.Error(_line, $"Unexpected character: {c}");
                    }
                    break;
            }
        }

        bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        void AddToken(TokenType type, object literal)
        {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        char Advance()
        {
            return _source[_current++];
        }

        char PeekNext()
        {
            if (_current + 1 == _source.Length) return '\0';
            return _source[_current + 1];
        }

        char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        void ScanString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                    _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated String.");
                return;
            }

            // TODO: Support \n, \t http://www.craftinginterpreters.com/scanning.html#string-literals
            // Trim the surrounding quotes
            var value = _source.Substring(_start + 1, _current - _start - 1);
            AddToken(TokenType.String, value);

            Advance();
        }

        void ScanNumber()
        {
            while (IsDigit(Peek()))
                Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance(); // Consume the '.'

                while (IsDigit(Peek()))
                    Advance();
            }

            var text = _source.Substring(_start, _current - _start);
            AddToken(TokenType.Number, Double.Parse(text));
        }

        void ScanIndentifier()
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            // See if the identifier is a reserved word.
            String text = _source.Substring(_start, _current - _start);

            if (_keywords.ContainsKey(text))
                AddToken(_keywords[text]);
            else
                AddToken(TokenType.Identifier);
        }

        bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        bool IsAtEnd() => _current >= _source.Length;

    }
}
