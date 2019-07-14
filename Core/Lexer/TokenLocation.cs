namespace Core
{
    public struct TokenLocation
    {
        public readonly int Line;
        public readonly int Offset;

        public TokenLocation(int line, int offset)
        {
            Line = line;
            Offset = offset;
        }

        public override string ToString()
        {
            return $"Line: {Line} Offset: {Offset}";
        }
    }
}
