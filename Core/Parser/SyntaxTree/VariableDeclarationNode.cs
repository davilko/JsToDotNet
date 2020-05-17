using System.Collections.Generic;

namespace Core.Parser.SyntaxTree
{
    public class VariableDeclarationNode
    {
        public TypeDeclarationNode Type { get; }
        public IEnumerable<IdentifierNode> Names { get; }

        public VariableDeclarationNode(TypeDeclarationNode type, IEnumerable<IdentifierNode> names)
        {
            Type = type;
            Names = names;
        }
    }
}