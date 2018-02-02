using System;
using System.Collections.Generic;
using System.Text;

namespace LoxSharp
{
    public enum TokenType
    {
        // Single-character tokens
        LeftParenthesis,
        RightParenthesis,

        LeftBrace,
        RightBrace,

        Plus,
        Minus,
        Slash,
        Star,

        Dot,
        Comma,
        Semicolon,

        // One or two character tokens
        Bang,
        BangEqual,

        Equal,
        EqualEqual,

        Greater,
        GreaterEqual,

        Less,
        LessEqual,

        // Literals
        Identifier,
        String,
        Number,

        // Keywords
        And,
        Or,
        Class,
        If,
        Else,
        True,
        False,
        Fun,
        For,
        While,
        Nil,
        Print,
        Return,
        Super,
        This,
        Var,

        EOF
    }
}
