namespace Core.Lexer
{
    public enum TokenType
    {
        Eof,
        Semicolon,
        Comma,
        Point,
        
        // keywords
        Do,
        Else,
        Var,
        Return,
        Void,
        While,
        Function,
        This,
        If,
        Delete,
        Class,
        Let,
        Static,
        Field,
        Constructor,
        Method,
        Int,
        Char,
        Boolean,

        // Literals
        Null,
        Undefined,
        True,
        False,
        Number,
        String,
        Identifier,

        // Braces
        ParenOpen,
        ParenClose,
        CurlyOpen,
        CurlyClose,
        SquareOpen,
        SquareClose,

        // Operators
        Plus,
        Minus,
        Multiply,
        Divide,
        Less,
        Greater,
        BitAnd,
        BitOr,
        BitNegation,
        Assign,
    }
}