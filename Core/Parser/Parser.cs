using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Lexer;
using Core.Parser.SyntaxTree;

namespace Core.Parser
{
    public class Parser
    {
        private readonly IList<Token> _tokenSource;
        private int _currentIndex;

        private Token Current => _tokenSource[_currentIndex];

        public Parser(IEnumerable<Token> tokenSource)
        {
            _tokenSource = tokenSource.ToArray();
            _currentIndex = 0;
        }

        public SyntaxNode Parse()
        {
            return ParseClass();
        }

        internal ClassNode ParseClass()
        {
            Expect(TokenType.Class);
            Advanced();
            var classIdentifier = ParseIdentifier();
            
            Advanced();
            Expect(TokenType.CurlyOpen);

            var fields = new List<ClassVarDeclarationNode>();
            while (CheckAhead(TokenType.Static) || CheckAhead(TokenType.Field))
            {
                Advanced();
                var field = ParseClassField();
                fields.Add(field);
            }
            
            Advanced();
            
            var classSubRoutines = new List<ClassSubroutineDeclarationNode>();
            while (CheckCurrent(TokenType.Function, TokenType.Method, TokenType.Constructor))
            {
                var classSubRoutine = ParseClassSubroutine();
                classSubRoutines.Add(classSubRoutine);
            }
            
            Expect(TokenType.CurlyClose);
            
            return new ClassNode(classIdentifier, fields, classSubRoutines);
        }

        internal ClassSubroutineDeclarationNode ParseClassSubroutine()
        {
            Expect(TokenType.Constructor, TokenType.Method, TokenType.Function);
            var classRoutineKind = Current.Type;
            Advanced();
            var returnType = ParseType();
            Advanced();
            var subroutineName = ParseIdentifier();
            
            Advanced();
            Expect(TokenType.ParenOpen);

            var parameters = new List<ParameterNode>();
            if (!CheckAhead(TokenType.ParenClose))
            {
                do
                {
                    Advanced();
                    var parameter = ParseParameter();
                    parameters.Add(parameter);
                    if (CheckAhead(TokenType.Comma))
                    {
                        Advanced();
                    }
                } 
                while (CheckCurrent(TokenType.Comma));
            }
            else
            {
                Advanced();
            }
            
            Expect(TokenType.ParenClose);
            
            Advanced();
            var body = ParseSubroutineBody();
            
            return new ClassSubroutineDeclarationNode(classRoutineKind, returnType, subroutineName, parameters, body);
        }

        internal SubRoutineBodyNode ParseSubroutineBody()
        {
            Expect(TokenType.CurlyOpen);
            Advanced();
            var variableDeclarations = new List<VariableDeclarationNode>();
            while (CheckAhead(TokenType.Var))
            {
                var varDeclaration = ParseVariableDeclaration();
                variableDeclarations.Add(varDeclaration);
            }
            
            var statements = ParseStatements();
            
            Expect(TokenType.CurlyClose);
            Advanced();
            
            return new SubRoutineBodyNode(variableDeclarations, statements);
        }

        internal StatementNode ParseStatement()
        {
            switch (Current.Type)
            {
                case TokenType.If: return ParseIfStatement();
                case TokenType.While: return ParseWhileStatement();
                case TokenType.Let: return ParseLetStatement();
                case TokenType.Do: return ParseDoStatement();
                case TokenType.Return: return ParseReturnStatement();
                default:
                    throw new Exception($"Invalid token {Current.Type}");
            }
        }

        internal IfStatementNode ParseIfStatement()
        {
            Expect(TokenType.If);
            Advanced();
            
            Expect(TokenType.ParenOpen);
            Advanced();
            var condition = ParseExpression();
            
            Advanced();
            Expect(TokenType.ParenClose);
            
            Advanced();
            Expect(TokenType.CurlyOpen);
            
            Advanced();
            var ifStatements = ParseStatements();
            
            Expect(TokenType.CurlyClose);
            
            Advanced();
            var elseStatements = new List<StatementNode>();
            if (CheckAhead(TokenType.Else))
            {
                Advanced();
                Expect(TokenType.CurlyOpen);
                
                Advanced();
                elseStatements.AddRange(ParseStatements());
                
                Advanced();
                Expect(TokenType.CurlyClose);
                Advanced();
            }
            
            return new IfStatementNode(condition, ifStatements, elseStatements);
        }

        internal IEnumerable<StatementNode> ParseStatements()
        {
            var statements = new List<StatementNode>();
            while (
                CheckCurrent(TokenType.Let) 
                || CheckCurrent(TokenType.If) 
                || CheckCurrent(TokenType.While)
                || CheckCurrent(TokenType.Do)
                || CheckCurrent(TokenType.Return))
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            return statements;
        }
        
        internal WhileStatementNode ParseWhileStatement()
        {
            Expect(TokenType.While);
            Advanced();
            
            Expect(TokenType.ParenOpen);
            Advanced();
            var condition = ParseExpression();
            
            Advanced();
            Expect(TokenType.ParenClose);
            
            Advanced();
            Expect(TokenType.CurlyOpen);
            
            Advanced();
            var statements = ParseStatements();
            
            Expect(TokenType.CurlyClose);
            Advanced();
            
            return new WhileStatementNode(condition, statements);
        }

        internal LetStatementNode ParseLetStatement()
        {
            Expect(TokenType.Let);
            Advanced();
            
            Expect(TokenType.Identifier);
            var identifier = ParseIdentifier();

            ExpressionNode arrayAccessExpression = null;
            if (CheckAhead(TokenType.SquareOpen))
            {
                Advanced();
                Advanced();
                arrayAccessExpression = ParseExpression();
                Advanced();
                Expect(TokenType.SquareClose);
            }
            
            Advanced();
            Expect(TokenType.Assign);

            Advanced();
            var valueExpression = ParseExpression();
            
            Advanced();
            Expect(TokenType.Semicolon);
            Advanced();

            if (arrayAccessExpression != null)
            {
                return new LetStatementNode(identifier, arrayAccessExpression, valueExpression);
            }

            return new LetStatementNode(identifier, valueExpression);
        }
        
        internal DoStatement ParseDoStatement()
        {
            Expect(TokenType.Do);
            Advanced();

            var expression = ParseSubroutineCallExpression();
            Advanced();
            
            Expect(TokenType.Semicolon);
            Advanced();
            return new DoStatement(expression);
        }
        
        internal ReturnStatement ParseReturnStatement()
        {
            Expect(TokenType.Return);
            Advanced();

            if (CheckCurrent(TokenType.Semicolon))
            {
                Advanced();
                return new ReturnStatement();
            }
            var returnExpression = ParseExpression();
            Advanced();
            Expect(TokenType.Semicolon);
            Advanced();
            return new ReturnStatement(returnExpression);
        }

        internal ExpressionNode ParseExpression()
        {
            ExpressionNode expression = ParseTermExpression();

            while (CheckAhead(TokenType.Plus, 
                TokenType.Minus,
                TokenType.BitAnd,
                TokenType.BitOr,
                TokenType.Multiply,
                TokenType.Delete,
                TokenType.Assign,
                TokenType.Greater,
                TokenType.Less,
                TokenType.BitNegation))
            {
                Advanced();
                BinaryOp? binaryOperator;
                switch (Current.Type)
                {
                    case TokenType.Plus:
                        binaryOperator = BinaryOp.Plus;
                        break;
                    case TokenType.Minus:
                        binaryOperator = BinaryOp.Minus;
                        break;
                    case TokenType.Divide:
                        binaryOperator = BinaryOp.Divide;
                        break;
                    case TokenType.Multiply:
                        binaryOperator = BinaryOp.Multiply;
                        break;
                    case TokenType.BitAnd:
                        binaryOperator = BinaryOp.BitAnd;
                        break;
                    case TokenType.BitOr:
                        binaryOperator = BinaryOp.BitOr;
                        break;
                    case TokenType.BitNegation:
                        binaryOperator = BinaryOp.BitNegation;
                        break;
                    // Equals
                    case TokenType.Assign:
                        binaryOperator = BinaryOp.Assign;
                        break;
                    case TokenType.Greater:
                        binaryOperator = BinaryOp.Greater;
                        break;
                    case TokenType.Less:
                        binaryOperator = BinaryOp.Less;
                        break;
                    default:
                        throw new Exception($"Expected binary operator but got {Current.Type}");
                }
                Advanced();
                var termExpressionRight = ParseTermExpression();
                expression = new BinaryExpression(expression, binaryOperator.Value, termExpressionRight);
            }

            return expression;
        }

        internal ExpressionNode ParseTermExpression()
        {
            switch (Current.Type)
            {   
                case TokenType.False:
                    return new KeywordExpression(KeywordConstants.False);
                case TokenType.True:
                    return new KeywordExpression(KeywordConstants.True);
                case TokenType.Null:
                    return new KeywordExpression(KeywordConstants.Null);
                case TokenType.This:
                    return new KeywordExpression(KeywordConstants.This);
                case TokenType.Number: 
                    return new NumberExpression(Current.Value);
                case TokenType.String: 
                    return new StringExpression(Current.Value);
                case TokenType.Minus:
                    Advanced();
                    var unaryExpression = ParseExpression();
                    return new UnaryExpression(UnaryOp.Minus, unaryExpression);
                case TokenType.BitNegation:
                    Advanced();
                    var bitNegationExpression = ParseExpression();
                    return new UnaryExpression(UnaryOp.BitNegation, bitNegationExpression);
                case TokenType.ParenOpen:
                    Advanced();
                    var expression = ParseExpression();
                    Advanced();
                    Expect(TokenType.ParenClose);
                    return expression;
                case TokenType.Identifier:
                    if (CheckAhead(TokenType.SquareOpen))
                    {
                        Advanced();
                        Advanced();
                        var accessArrayExpression = ParseExpression();
                        Advanced();
                        Expect(TokenType.SquareClose);
                        return new IdentifierExpression(new IdentifierNode(Current.Value), accessArrayExpression);
                    }

                    if (CheckAhead(TokenType.Point, TokenType.ParenOpen))
                    {
                        return ParseSubroutineCallExpression();
                    }
                    return new IdentifierExpression(new IdentifierNode(Current.Value));
                default:
                    throw new Exception($"Can not parse expression starts with {Current.Type}");
            }
        }

        internal SubroutineCallExpression ParseSubroutineCallExpression()
        {
            Expect(TokenType.Identifier);
            var subroutineIdentifier = ParseIdentifier();
            Advanced();

            IdentifierNode rootIdentifier = null;
            if (CheckCurrent(TokenType.Point))
            {
                rootIdentifier = subroutineIdentifier;
                Advanced();
                Expect(TokenType.Identifier);
                subroutineIdentifier = ParseIdentifier();
            }
            
            Advanced();
            var parameters = ParseExpressionParameterList();
            if (rootIdentifier != null)
            {
                return new SubroutineCallExpression(rootIdentifier, subroutineIdentifier, parameters);
            }
            return new SubroutineCallExpression(subroutineIdentifier, parameters);
        }

        internal IEnumerable<ExpressionNode> ParseExpressionParameterList()
        {
            Expect(TokenType.ParenOpen);
            var expressions = new List<ExpressionNode>();
            if (!CheckAhead(TokenType.ParenClose))
            {
                do
                {
                    Advanced();
                    var expression = ParseExpression();
                    expressions.Add(expression);
                    if (CheckAhead(TokenType.Comma))
                    {
                        Advanced();
                    }
                } 
                while (CheckCurrent(TokenType.Comma));
            }
            
            Advanced();
            Expect(TokenType.ParenClose);
            return expressions;
        }

        internal VariableDeclarationNode ParseVariableDeclaration()
        {
            Expect(TokenType.Var);
            Advanced();
            
            var type = ParseType();
            Advanced();
            
            var names = new List<IdentifierNode>();
            do
            {
                var varName = ParseIdentifier();
                names.Add(varName);
            } 
            while (CheckCurrent(TokenType.Comma));

            return new VariableDeclarationNode(type, names);
        }

        internal ParameterNode ParseParameter()
        {
            var type = ParseType();
            Advanced();
            var parameterName = ParseIdentifier();
            Advanced();
            return new ParameterNode(type, parameterName);
        }

        internal ClassVarDeclarationNode ParseClassField()
        {
            var fieldKind = Current.Type;
            Advanced();
            var type = ParseType();
            
            var identifiers = new List<IdentifierNode>();
            do
            {
                Advanced();
                var fieldName = ParseIdentifier();
                identifiers.Add(fieldName);
                Advanced();
            } 
            while (CheckCurrent(TokenType.Comma));
            
            Expect(TokenType.Semicolon);

            return new ClassVarDeclarationNode(fieldKind, type, identifiers);
        }

        internal TypeDeclarationNode ParseType()
        {
            if (CheckCurrent(TokenType.Int) 
                || CheckCurrent(TokenType.Char) 
                || CheckCurrent(TokenType.Boolean) 
                || CheckCurrent(TokenType.Void))
            {
                return new TypeDeclarationNode(Current.Type);
            }

            var identifierType = ParseIdentifier();
            return new TypeDeclarationNode(identifierType);
        }

        internal IdentifierNode ParseIdentifier()
        {
            Expect(TokenType.Identifier);
            return new IdentifierNode(Current.Value);
        }

        private bool CheckCurrent(params TokenType[] tokenTypes)
        {
            if (_currentIndex < _tokenSource.Count)
            {
                return tokenTypes.Contains(Current.Type);
            }

            return false;
        }
        
        private bool CheckAhead(params TokenType[] tokenTypes)
        {
            if (_currentIndex + 1 < _tokenSource.Count)
            {
                var aheadToken = _tokenSource[_currentIndex + 1];
                return tokenTypes.Contains(aheadToken.Type);
            }

            return false;
        }

        private void Expect(params TokenType[] tokenTypes)
        {
            if (!tokenTypes.Contains(Current.Type))
            {
                throw new Exception($"Got {Current.Type} but expected {tokenTypes}");
            }
        }

        private void Advanced()
        {
            _currentIndex++;
        }
    }
}