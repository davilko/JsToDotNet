using System.Collections.Generic;

namespace Core.Parser.SyntaxTree
{
    public class ExpressionNode : SyntaxNode
    {
        
    }

    public class BinaryExpression : ExpressionNode
    {
        public ExpressionNode LeftExpression { get; }
        public BinaryOp BinaryOperator { get; }
        public ExpressionNode RightExpression { get; }
        
        public BinaryExpression(
            ExpressionNode leftExpression, 
            BinaryOp binaryOperator, 
            ExpressionNode rightExpression)
        {
            LeftExpression = leftExpression;
            BinaryOperator = binaryOperator;
            RightExpression = rightExpression;
        }
    }

    public class UnaryExpression : ExpressionNode
    {
        public UnaryOp Operator { get; }
        public ExpressionNode Expression { get; }
        
        public UnaryExpression(UnaryOp @operator, ExpressionNode expression)
        {
            Operator = @operator;
            Expression = expression;
        }
    }

    public class IdentifierExpression : ExpressionNode
    {
        public IdentifierNode Identifier { get; }
        public ExpressionNode ArrayAccessExpression { get; }
        public bool IsArray { get; }
        
        public IdentifierExpression(IdentifierNode identifier)
        {
            Identifier = identifier;
            IsArray = false;
        }
        
        public IdentifierExpression(IdentifierNode identifier, ExpressionNode arrayAccessExpression)
        {
            Identifier = identifier;
            ArrayAccessExpression = arrayAccessExpression;
            IsArray = true;
        }
    }

    public class KeywordExpression : ExpressionNode
    {
        public KeywordExpression(KeywordConstants value)
        {
            Value = value;
        }

        public KeywordConstants Value { get; }
    }

    public class NumberExpression : ExpressionNode
    {
        public string Value { get; }
        
        public NumberExpression(string value)
        {
            Value = value;
        }
    }
    
    
    public class StringExpression : ExpressionNode
    {
        public string Value { get; }
        
        public StringExpression(string value)
        {
            Value = value;
        }
    }

    public class SubroutineCallExpression : ExpressionNode
    {
        public IdentifierNode Identifier { get; }
        public IEnumerable<ExpressionNode> ExpressionList { get; }
        public IdentifierNode Root { get; }
        public bool IsCallOnRoot { get; }
        
        public SubroutineCallExpression(IdentifierNode identifier, IEnumerable<ExpressionNode> expressionList)
        {
            Identifier = identifier;
            ExpressionList = expressionList;
            IsCallOnRoot = false;
        }
        
        public SubroutineCallExpression(
            IdentifierNode root, 
            IdentifierNode identifier, 
            IEnumerable<ExpressionNode> expressionList)
        {
            Root = root;
            Identifier = identifier;
            ExpressionList = expressionList;
            IsCallOnRoot = true;
        }
    }
}