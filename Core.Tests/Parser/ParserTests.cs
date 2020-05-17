using System.Collections.Generic;
using System.Linq;
using Core.Lexer;
using Core.Parser.SyntaxTree;
using NUnit.Framework;

namespace Core.Tests.Parser
{
    public class ParserTests
    {
        [Test]
        public void Should_Parse_Class()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                new Token(TokenType.Class, new TokenLocation(), new TokenLocation(), "class"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "Array"),
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseClass();
            
            // Assert
            Assert.AreEqual("Array", result.Identifier.Name);
        }
        
        [Test]
        public void Should_Parse_ClassSubroutine_Function()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // function int parseInt(char x, boolean y) { }
                new Token(TokenType.Function, new TokenLocation(), new TokenLocation(), "function"), 
                new Token(TokenType.Int, new TokenLocation(), new TokenLocation(), "int"), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "parseInt"), 
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("), 
                new Token(TokenType.Char, new TokenLocation(), new TokenLocation(), "char"), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"), 
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","), 
                new Token(TokenType.Boolean, new TokenLocation(), new TokenLocation(), "boolean"), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "y"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseClassSubroutine();
            
            // Assert
            Assert.AreEqual("parseInt", result.Identifier.Name);
            Assert.AreEqual(TokenType.Int, result.ReturnType.CommonType);

            var parameters = result.ParameterList.ToArray();
            Assert.AreEqual(2, parameters.Length);
            
            Assert.AreEqual("x", parameters[0].Identifier.Name);
            Assert.AreEqual(TokenType.Char, parameters[0].Type.CommonType);
            
            Assert.AreEqual("y", parameters[1].Identifier.Name);
            Assert.AreEqual(TokenType.Boolean, parameters[1].Type.CommonType);
        }
        
         [Test]
        public void Should_Parse_ClassSubroutine_Two_Static_Arrays()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // static Array myArray1, myArray2;
                new Token(TokenType.Static, new TokenLocation(), new TokenLocation(), "static"), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "Array"), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "myArray1"), 
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","), 
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "myArray2"), 
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseClassField();
            
            // Assert
            Assert.AreEqual(TokenType.Static, result.Kind);
            Assert.AreEqual("Array", result.Type.CustomType.Name);

            var nameIdentifiers = result.Names.ToArray();
            Assert.AreEqual(2, nameIdentifiers.Length);
            
            Assert.AreEqual("myArray1", nameIdentifiers[0].Name);
            Assert.AreEqual("myArray2", nameIdentifiers[1].Name);
        }


        [Test]
        public void Should_Parse_Number_Add_Expression()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "5"),
                new Token(TokenType.Plus, new TokenLocation(), new TokenLocation(), "+"),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "4"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseExpression();
            
            // Assert
            Assert.IsInstanceOf<BinaryExpression>(result);
        }
        
        [Test]
        public void Should_Parse_Variable_Add_Expression()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Plus, new TokenLocation(), new TokenLocation(), "+"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "y"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseExpression();
            
            // Assert
            Assert.IsInstanceOf<BinaryExpression>(result);
        }
        
        [Test]
        public void Should_Parse_ParameterList_Expression()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // (x - 4, array.length())
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Minus, new TokenLocation(), new TokenLocation(), "-"),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "4"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "array"),
                new Token(TokenType.Point, new TokenLocation(), new TokenLocation(), "."),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "length"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseExpressionParameterList().ToArray();
            
            // Assert
            Assert.IsInstanceOf<BinaryExpression>(result[0]);
            Assert.IsInstanceOf<SubroutineCallExpression>(result[1]);
        }
        
        [Test]
        public void Should_Parse_Array_Access_Expression()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // myArray[5]
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "myArray"),
                new Token(TokenType.SquareOpen, new TokenLocation(), new TokenLocation(), "["),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "5"),
                new Token(TokenType.SquareClose, new TokenLocation(), new TokenLocation(), "]"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseTermExpression();
            
            // Assert
            Assert.IsInstanceOf<IdentifierExpression>(result);
        }

        [Test]
        public void Should_Parse_Do_Statement()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // do myClass.DoSomethingUseful(x, true, false, null, "just string");
                new Token(TokenType.Do, new TokenLocation(), new TokenLocation(), "do"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "myClass"),
                new Token(TokenType.Point, new TokenLocation(), new TokenLocation(), "."),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "DoSomethingUseful"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.True, new TokenLocation(), new TokenLocation(), "true"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.False, new TokenLocation(), new TokenLocation(), "false"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.Null, new TokenLocation(), new TokenLocation(), "null"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.String, new TokenLocation(), new TokenLocation(), "just string"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseDoStatement();
            
            // Assert
            Assert.IsInstanceOf<DoStatement>(result);
        }

        [Test]
        public void Should_Parse_IfStatement()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                // if(x > 1) { let x = 4 + y; return x; }
                new Token(TokenType.If, new TokenLocation(), new TokenLocation(), "if"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Greater, new TokenLocation(), new TokenLocation(), ">"),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "1"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                new Token(TokenType.Let, new TokenLocation(), new TokenLocation(), "let"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Assign, new TokenLocation(), new TokenLocation(), "="),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "4"),
                new Token(TokenType.Plus, new TokenLocation(), new TokenLocation(), "+"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "y"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
                new Token(TokenType.Return, new TokenLocation(), new TokenLocation(), "return"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseIfStatement();
            
            // Assert
            Assert.IsInstanceOf<IfStatementNode>(result);
            Assert.IsInstanceOf<BinaryExpression>(result.Condition);
            
            var statements = result.IfStatements.ToArray();
            Assert.IsInstanceOf<LetStatementNode>(statements[0]);
            Assert.IsInstanceOf<ReturnStatement>(statements[1]);
        }
        
        [Test]
        public void Should_Parse_WhileStatement()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                /*
                 * while(true)
                 * {
                 *     let x[x.length() - 1] = null;
                 *     if(x[0])
                 *     {
                 *         while(false)
                 *         {
                 *             return;
                 *         }
                 *     }
                 * }
                 */
                new Token(TokenType.While, new TokenLocation(), new TokenLocation(), "while"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.True, new TokenLocation(), new TokenLocation(), "true"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                
                new Token(TokenType.Let, new TokenLocation(), new TokenLocation(), "let"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.SquareOpen, new TokenLocation(), new TokenLocation(), "["),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.Point, new TokenLocation(), new TokenLocation(), "."),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "length"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.Minus, new TokenLocation(), new TokenLocation(), "-"),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "1"),
                new Token(TokenType.SquareClose, new TokenLocation(), new TokenLocation(), "]"),
                new Token(TokenType.Assign, new TokenLocation(), new TokenLocation(), "="),
                new Token(TokenType.Null, new TokenLocation(), new TokenLocation(), "null"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
                
                new Token(TokenType.If, new TokenLocation(), new TokenLocation(), "if"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "x"),
                new Token(TokenType.SquareOpen, new TokenLocation(), new TokenLocation(), "["),
                new Token(TokenType.Number, new TokenLocation(), new TokenLocation(), "0"),
                new Token(TokenType.SquareClose, new TokenLocation(), new TokenLocation(), "]"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                
                new Token(TokenType.While, new TokenLocation(), new TokenLocation(), "while"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.True, new TokenLocation(), new TokenLocation(), "false"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                new Token(TokenType.Return, new TokenLocation(), new TokenLocation(), "return"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
                
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
                
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
            };
            
            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.ParseWhileStatement();
            
            // Assert
            Assert.IsInstanceOf<WhileStatementNode>(result);
            Assert.IsInstanceOf<KeywordExpression>(result.Condition);
            var statements = result.Statements.ToArray();
            
            Assert.IsInstanceOf<LetStatementNode>(statements[0]);
            Assert.IsInstanceOf<IfStatementNode>(statements[1]);

            var ifStatements = ((IfStatementNode) statements[1]).IfStatements.ToArray();
            Assert.IsInstanceOf<WhileStatementNode>(ifStatements[0]);
            var whileStatements = ((WhileStatementNode) ifStatements[0]).Statements.ToArray();
            Assert.IsInstanceOf<ReturnStatement>(whileStatements[0]);
        }
        
        
        /*
         class List {
            field int data;
            field List next;
            
            constructor List new(int car, List cdr) 
            {
                let data = car;
                let next = cdr;
                return this;
            } 
            
            method void dispose() 
            {
                if (~(next = null)) 
                {
                    do next.dispose();
                }
                do Memory.deAlloc(this);
                return;
            }
         }
         */
        [Test]
        public void Should_Parse_Full_Class()
        {
            // Arrange
            IEnumerable<Token> tokens = new[]
            {
                new Token(TokenType.Class, new TokenLocation(), new TokenLocation(), "class"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "List"),

                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),

                new Token(TokenType.Field, new TokenLocation(), new TokenLocation(), "field"),
                new Token(TokenType.Int, new TokenLocation(), new TokenLocation(), "int"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "data"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.Field, new TokenLocation(), new TokenLocation(), "field"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "List"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "next"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.Constructor, new TokenLocation(), new TokenLocation(), "constructor"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "List"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "new"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Int, new TokenLocation(), new TokenLocation(), "int"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "car"),
                new Token(TokenType.Comma, new TokenLocation(), new TokenLocation(), ","),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "List"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "cdr"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),

                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),

                new Token(TokenType.Let, new TokenLocation(), new TokenLocation(), "let"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "data"),
                new Token(TokenType.Assign, new TokenLocation(), new TokenLocation(), "="),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "car"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.Let, new TokenLocation(), new TokenLocation(), "let"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "next"),
                new Token(TokenType.Assign, new TokenLocation(), new TokenLocation(), "="),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "cdr"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.Return, new TokenLocation(), new TokenLocation(), "return"),
                new Token(TokenType.This, new TokenLocation(), new TokenLocation(), "this"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),

                new Token(TokenType.Method, new TokenLocation(), new TokenLocation(), "method"),
                new Token(TokenType.Void, new TokenLocation(), new TokenLocation(), "void"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "dispose"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),

                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),

                new Token(TokenType.If, new TokenLocation(), new TokenLocation(), "if"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.BitNegation, new TokenLocation(), new TokenLocation(), "~"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "next"),
                new Token(TokenType.Assign, new TokenLocation(), new TokenLocation(), "="),
                new Token(TokenType.Null, new TokenLocation(), new TokenLocation(), "null"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.CurlyOpen, new TokenLocation(), new TokenLocation(), "{"),
                new Token(TokenType.Do, new TokenLocation(), new TokenLocation(), "do"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "next"),
                new Token(TokenType.Point, new TokenLocation(), new TokenLocation(), "."),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "dispose"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),
                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),

                new Token(TokenType.Do, new TokenLocation(), new TokenLocation(), "do"),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "Memory"),
                new Token(TokenType.Point, new TokenLocation(), new TokenLocation(), "."),
                new Token(TokenType.Identifier, new TokenLocation(), new TokenLocation(), "deAlloc"),
                new Token(TokenType.ParenOpen, new TokenLocation(), new TokenLocation(), "("),
                new Token(TokenType.This, new TokenLocation(), new TokenLocation(), "this"),
                new Token(TokenType.ParenClose, new TokenLocation(), new TokenLocation(), ")"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.Return, new TokenLocation(), new TokenLocation(), "return"),
                new Token(TokenType.Semicolon, new TokenLocation(), new TokenLocation(), ";"),

                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),


                new Token(TokenType.CurlyClose, new TokenLocation(), new TokenLocation(), "}"),
            };

            var parser = new Core.Parser.Parser(tokens);

            // Act
            var result = parser.Parse();

            // Assert
            Assert.IsInstanceOf<ClassNode>(result);
        }

    }
}