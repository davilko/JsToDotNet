using System.Collections.Generic;
using Core.Lexer;

namespace Core.Parser.SyntaxTree
{
    public class ClassNode : SyntaxNode
    {
        public IdentifierNode Identifier { get; }
        public IEnumerable<ClassVarDeclarationNode> VariableDeclarations { get; }
        public IEnumerable<ClassSubroutineDeclarationNode> SubroutineDeclarations { get; }

        public ClassNode(
            IdentifierNode identifier, 
            IEnumerable<ClassVarDeclarationNode> variableDeclarations, 
            IEnumerable<ClassSubroutineDeclarationNode> subroutineDeclarations)
        {
            Identifier = identifier;
            VariableDeclarations = variableDeclarations;
            SubroutineDeclarations = subroutineDeclarations;
        }
    }

    public class ClassVarDeclarationNode : SyntaxNode
    {
        public TokenType Kind { get; }
        public TypeDeclarationNode Type { get; }
        public IEnumerable<IdentifierNode> Names { get; }

        public ClassVarDeclarationNode(
            TokenType kind, 
            TypeDeclarationNode type,
            IEnumerable<IdentifierNode> names)
        {
            Kind = kind;
            Type = type;
            Names = names;
        }
    }

    public class ClassSubroutineDeclarationNode : SyntaxNode
    {
        public TokenType Kind { get; }
        public TypeDeclarationNode ReturnType { get; }
        public IdentifierNode Identifier { get; }
        public IEnumerable<ParameterNode> ParameterList { get; }
        public SubRoutineBodyNode Body { get; }

        public ClassSubroutineDeclarationNode(
            TokenType kind, 
            TypeDeclarationNode returnType, 
            IdentifierNode identifier, 
            IEnumerable<ParameterNode> parameterList, 
            SubRoutineBodyNode body)
        {
            Kind = kind;
            ReturnType = returnType;
            Identifier = identifier;
            ParameterList = parameterList;
            Body = body;
        }
    }

    public class ParameterNode : SyntaxNode
    {
        public TypeDeclarationNode Type { get; }
        public IdentifierNode Identifier { get; }

        public ParameterNode(TypeDeclarationNode type, IdentifierNode identifier)
        {
            Type = type;
            Identifier = identifier;
        }
    }

    public class SubRoutineBodyNode : SyntaxNode
    {
        public IEnumerable<VariableDeclarationNode> Variables { get; }
        public IEnumerable<StatementNode> Statements { get; }
        
        public SubRoutineBodyNode(IEnumerable<VariableDeclarationNode> variables, IEnumerable<StatementNode> statements)
        {
            Variables = variables;
            Statements = statements;
        }
    }
}