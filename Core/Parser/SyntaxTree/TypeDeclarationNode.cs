using Core.Lexer;

namespace Core.Parser.SyntaxTree
{
    public class TypeDeclarationNode : SyntaxNode
    {
        public TokenType CommonType { get; }
        public IdentifierNode CustomType { get; }
        public bool isCommonType { get; }

        public TypeDeclarationNode(TokenType commonType)
        {
            CommonType = commonType;
            isCommonType = true;
        }

        public TypeDeclarationNode(IdentifierNode customType)
        {
            CustomType = customType;
            isCommonType = false;
        }
    }
}