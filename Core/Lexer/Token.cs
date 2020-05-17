namespace Core.Lexer
{
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Value;
        public readonly TokenLocation Start;
        public readonly TokenLocation End;

        public Token(TokenType type, TokenLocation start, TokenLocation end, string value = null)
        {
            Type = type;
            Start = start;
            End = end;
            Value = value;
        }
        
        // Debug
        public override string ToString()
        {
            return $"{Type}: Start {Start} End {End}";
        }
    }
}