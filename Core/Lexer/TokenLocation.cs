namespace Core
{
    public struct TokenLocation
    {
        public readonly int Line;
        public readonly int Column;

        public TokenLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"Line: {Line} Column: {Column}";
        }
    }
}
