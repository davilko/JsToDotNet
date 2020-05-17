using System.Collections.Generic;
using System.Linq;
using Core.Lexer;
using NUnit.Framework;

namespace Core.Tests.Lexer
{
    public class LexerTests
    {
        [Test]
        public void Should_Parse_Let()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Let, 
                TokenType.Identifier, 
                TokenType.Assign, 
                TokenType.Number, 
                TokenType.Eof
            };
            
            var code = "let a = 1";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Parse_Function()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Function, 
                TokenType.Int,
                TokenType.Identifier, 
                TokenType.ParenOpen, 
                TokenType.Char,
                TokenType.Identifier,
                TokenType.Comma,
                TokenType.Boolean,
                TokenType.Identifier,
                TokenType.ParenClose, 
                TokenType.CurlyOpen,
                TokenType.CurlyClose,
                TokenType.Eof,
            };
            
            var code = "function int parseInt(char a, boolean b) {}";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Skip_Whitespices()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Eof,
            };
            
            var code = @"             
                        ";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Skip_OneLineComments()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Eof,
            };
            
            var code = @"   // some comments    ";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Skip_MultiLineComments()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Eof,
            };
            
            var code = @"   /* some comments    
                               bla bla bla
                            */";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Parse_StringLiterals_SingleQuotes()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Var,
                TokenType.Identifier,
                TokenType.Assign,
                TokenType.String,
                TokenType.Eof,
            };
            
            var code = @" var a = 'text'";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Parse_StringLiterals_DoubleQuotes()
        {
            // Arrange
            var expected = new[]
            {
                TokenType.Var,
                TokenType.Identifier,
                TokenType.Assign,
                TokenType.String,
                TokenType.Eof,
            };
            
            var code = " var a = \"text\"";
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);

            // Act
            var result = tokenSource.Select(t => t.Type).ToArray();
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void Should_Parse_StringLiterals_SingleQuotes_Multiline()
        {
            // Arrange
            var expectedTokens = new[]
            {
                TokenType.String,
                TokenType.Eof,
            };
            
            var expectedLocationStart = new TokenLocation(1, 1);
            var expectedLocationEnd = new TokenLocation(2, 45); 

            var code = @"'text bla bla
                          text continuation'";
            
            IEnumerable<Token> tokenSource = new Core.Lexer.Lexer(code);
            
            // Act
            var tokens = tokenSource.ToArray();

            var tokenTypes = tokens.Select(t => t.Type);

            var stringToken = tokens.First();
            
            // Assert
            Assert.AreEqual(expectedTokens, tokenTypes);
            Assert.AreEqual(expectedLocationStart, stringToken.Start);
            Assert.AreEqual(expectedLocationEnd, stringToken.End);
        }
    }
}