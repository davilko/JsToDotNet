using System.Collections.Generic;

namespace Core.Parser.SyntaxTree
{
    public abstract class StatementNode : SyntaxNode {}

    public class IfStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public IEnumerable<StatementNode> IfStatements { get; }
        public IEnumerable<StatementNode> ElseStatements { get; }

        public IfStatementNode(
            ExpressionNode condition, 
            IEnumerable<StatementNode> ifStatements, 
            IEnumerable<StatementNode> elseStatements)
        {
            Condition = condition;
            IfStatements = ifStatements;
            ElseStatements = elseStatements;
        }
    }

    public class WhileStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public IEnumerable<StatementNode> Statements { get; }

        public WhileStatementNode(ExpressionNode condition, IEnumerable<StatementNode> statements)
        {
            Condition = condition;
            Statements = statements;
        }
    }

    public class LetStatementNode : StatementNode
    {
        public IdentifierNode Name { get; }
        public bool IsArray { get; }
        public ExpressionNode ArrayAccessExpression { get; }
        public ExpressionNode Value { get; }
        
        public LetStatementNode(IdentifierNode name, ExpressionNode value)
        {
            Name = name;
            Value = value;
            IsArray = false;
        }
        
        public LetStatementNode(IdentifierNode name, ExpressionNode arrayAccessExpression, ExpressionNode value)
        {
            Name = name;
            ArrayAccessExpression = arrayAccessExpression;
            Value = value;
            IsArray = true;
        }
    }

    public class DoStatement : StatementNode
    {
        public SubroutineCallExpression CallExpression { get; }
        
        public DoStatement(SubroutineCallExpression callExpression)
        {
            CallExpression = callExpression;
        }
    }

    public class ReturnStatement : StatementNode
    {
        public ExpressionNode Expression { get; }
        public bool EmptyExpression { get; }
        public ReturnStatement(ExpressionNode expression)
        {
            Expression = expression;
            EmptyExpression = false;
        }

        public ReturnStatement()
        {
            EmptyExpression = true;
        }
    }
}