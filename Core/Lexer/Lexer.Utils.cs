using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core.Lexer
{
    public partial class Lexer
    {
        private static IDictionary<TokenType, string> KeywordTokens = new Dictionary<TokenType, string>
        {
            { TokenType.Do, "do" },
            { TokenType.Else, "else" },
            { TokenType.Var, "var" },
            { TokenType.Return, "return" },
            { TokenType.Void, "void" },
            { TokenType.While, "while" },
            { TokenType.Function, "function" },
            { TokenType.This, "this" },
            { TokenType.If, "if" },
            { TokenType.Class, "class" },
            { TokenType.Let, "let" },
            { TokenType.Static, "static" },
            { TokenType.Int, "int" },
            { TokenType.Char, "char" },
            { TokenType.Boolean, "boolean" },
            { TokenType.Constructor, "constructor" },
        };

        private IDictionary<TokenType, string> OperatorTokens = new Dictionary<TokenType, string>
        {
            {TokenType.Plus, "+"},
            {TokenType.Minus, "-"},
            {TokenType.Multiply, "*"},
            {TokenType.Divide, "/"},
            {TokenType.Less, "<"},
            {TokenType.Greater, ">"},
            {TokenType.BitAnd, "&"},
            {TokenType.BitOr, "|"},
            {TokenType.BitNegation, "~"},
            {TokenType.Assign, "="},
        };
        
        private IDictionary<TokenType, string> SymbolTokens = new Dictionary<TokenType, string>
        {
            {TokenType.ParenOpen, "("},
            {TokenType.ParenClose, ")"},
            {TokenType.CurlyOpen, "{"},
            {TokenType.CurlyClose, "}"},
            {TokenType.SquareOpen, "["},
            {TokenType.SquareClose, "]"},

            {TokenType.Comma, ","},
            {TokenType.Semicolon, ";"},
            {TokenType.Point, "."},
        };
        
        private IDictionary<TokenType, string> LiteralTokens = new Dictionary<TokenType, string>
        {
            {TokenType.Null, "null"},
            {TokenType.Undefined, "undefined"},
            {TokenType.True, "true"},
            {TokenType.False, "false"},
        };
        
        private IDictionary<TokenType, Regex> LiteralTokensRegex = new Dictionary<TokenType, Regex>
        {
            {TokenType.Identifier, new Regex("[a-zA-Z_][0-9a-zA-Z_]*", RegexOptions.Compiled)},
            {TokenType.Number, new Regex(@"(0|[1-9][0-9]*)(\.[0-9]+)?", RegexOptions.Compiled)},
        };
    }
}