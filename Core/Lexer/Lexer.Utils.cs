using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core
{
    public partial class Lexer
    {
        private static IDictionary<TokenType, string> KeywordTokens = new Dictionary<TokenType, string>
        {
            { TokenType.Typeof, "typeof" },
            { TokenType.Break, "break" },
            { TokenType.Do, "do" },
            { TokenType.Instanceof, "instanceof" },
            { TokenType.Case, "case" },
            { TokenType.Else, "else" },
            { TokenType.New, "new" },
            { TokenType.Var, "var" },
            { TokenType.Catch, "catch" },
            { TokenType.Finally, "finally" },
            { TokenType.Return, "return" },
            { TokenType.Void, "void" },
            { TokenType.Continue, "continue" },
            { TokenType.For, "for" },
            { TokenType.Switch, "switch" },
            { TokenType.While, "while" },
            { TokenType.Debugger, "debugger" },
            { TokenType.Function, "function" },
            { TokenType.This, "this" },
            { TokenType.With, "with" },
            { TokenType.Default, "default" },
            { TokenType.If, "if" },
            { TokenType.Throw, "throw" },
            { TokenType.Delete, "delete" },
            { TokenType.In, "in" },
            { TokenType.Try, "try" },
            { TokenType.Class, "class" },
            { TokenType.Enum, "enum" },
            { TokenType.Extends, "extends" },
            { TokenType.Super, "super" },
            { TokenType.Const, "const" },
            { TokenType.Export, "export" },
            { TokenType.Import, "import" },
            { TokenType.Implements, "implements" },
            { TokenType.Let, "let" },
            { TokenType.Private, "private" },
            { TokenType.Public, "public" },
            { TokenType.Interface, "interface" },
            { TokenType.Package, "package" },
            { TokenType.Protected, "protected" },
            { TokenType.Static, "static" },
            { TokenType.Yield, "yield" },
        };

        private IDictionary<TokenType, string> OperatorTokens = new Dictionary<TokenType, string>
        {
            {TokenType.ParenOpen, "("},
            {TokenType.ParenClose, ")"},
            {TokenType.CurlyOpen, "{"},
            {TokenType.CurlyClose, "}"},
            {TokenType.SquareOpen, "["},
            {TokenType.SquareClose, "]"},
            
            {TokenType.Plus, "+"},
            {TokenType.Minus, "-"},
            {TokenType.Multiply, "*"},
            {TokenType.Divide, "/"},
            {TokenType.Power, "**"},
            {TokenType.Equal, "==="},
            {TokenType.NotEqual, "!=="},
            {TokenType.Less, "<"},
            {TokenType.LessEqual, "<="},
            {TokenType.Greater, ">"},
            {TokenType.GreaterEqual, ">="},
            {TokenType.BitAnd, "&"},
            {TokenType.BitOr, "|"},
            {TokenType.BitXor, "^"},
            {TokenType.And, "&&"},
            {TokenType.Or, "||"},
            {TokenType.Xor, "^^"},
            {TokenType.ShiftLeft, "<<"},
            {TokenType.ShiftRight, ">>"},
            {TokenType.Assign, "="},
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