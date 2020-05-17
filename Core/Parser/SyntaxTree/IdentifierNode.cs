namespace Core.Parser.SyntaxTree
{
    public class IdentifierNode
    {
        public string Name { get; }
        
        public IdentifierNode(string name)
        {
            Name = name;
        }
    }
}