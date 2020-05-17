using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Lexer
{
    public partial class Lexer : IEnumerable<Token>
    {
        private int _line;
        private int _column;
        private int _position;
        private string _source;

        public Lexer(string source)
        {
            _source = source;
            _line = 1;
            _column = 1;
            _position = 0;
        }

        public Token NextToken()
        {
            SkipWhiteSpaces();
            
            if (InBounds && IsComment)
            {
                ProcessComment();
            }
            
            if (!InBounds)
            {
                return ProcessEof();
            }
            
            if (IsStringLiteral)
            {
                return ProcessStringLiteral();
            }
            
            var token = ProcessStaticToken(KeywordTokens)
                        ?? ProcessStaticToken(LiteralTokens)
                        ?? ProcessStaticToken(SymbolTokens)
                        ?? ProcessStaticToken(OperatorTokens)
                        ?? ProcessRegexToken(LiteralTokensRegex);
            
            if (token == null)
            {
                return ProcessEof();
            }
            
            return token;
        }

        private Token ProcessEof()
        {
            return new Token(TokenType.Eof, CurrentTokenLocation, CurrentTokenLocation);
        }

        private Token ProcessStaticToken(IDictionary<TokenType, string> tokenDefinitions)
        {
            foreach (var tokenDefinition in tokenDefinitions)
            {
                var tokenValue = tokenDefinition.Value;
                var tokenType = tokenDefinition.Key;
                if (_position + tokenValue.Length > _source.Length)
                    continue;
                if (_source.Substring(_position, tokenValue.Length) != tokenValue)
                    continue;

                var endTokenLocation = CalculateTokenLocation(tokenValue.Length);
                var token = new Token(tokenType, CurrentTokenLocation, endTokenLocation);
                Skip(tokenValue.Length);
                return token;
            }

            return null;
        }

        private Token ProcessRegexToken(IDictionary<TokenType, Regex> tokenDefinitions)
        {
            foreach (var tokenDefinition in tokenDefinitions)
            {
                var tokenRegex = tokenDefinition.Value;
                var tokenType = tokenDefinition.Key;
                var match = tokenRegex.Match(_source, _position);
                if (!match.Success)
                    continue;

                var tokenValue = match.Value;
                var token = new Token(tokenType, CurrentTokenLocation, CalculateTokenLocation(tokenValue.Length));
                Skip(match.Length);
                return token;
            }

            return null;
        }
        
        private Token ProcessStringLiteral()
        {
            var startPosition = CurrentTokenLocation;
            if (CurrentChar == '\"' || CurrentChar == '\'')
            {
                Skip();
            }
            
            var stringBuilder = new StringBuilder();

            while (InBounds && (CurrentChar != '\"' && CurrentChar != '\''))
            {
                if (CurrentChar == '\\') // ' example string can\'t '
                {
                    stringBuilder.Append(CurrentChar); // \ 
                    Skip();
                    stringBuilder.Append(CurrentChar); // '
                    Skip();
                }

                if (IsNewLine)
                {
                    NewLine();
                }
                
                stringBuilder.Append(CurrentChar);
                Skip();
            }

            var endLocation = CurrentTokenLocation; 
            var token = new Token(TokenType.String, startPosition, endLocation, stringBuilder.ToString());
            
            Skip();
            return token;
        }
        
        private void SkipWhiteSpaces()
        {
            while (InBounds && char.IsWhiteSpace(_source[_position]))
            {
                _position++;
            }
        }

        private void ProcessComment()
        {
            if (NextChar == '/')
            {
                while (InBounds)
                {
                    if (IsNewLine)
                    {
                        NewLine();
                        break;
                    }

                    Skip();
                }

                return;
            }

            if (NextChar == '*')
            {
                while (InBounds)
                {
                    if (IsNewLine)
                    {
                        NewLine();
                    }

                    if (CurrentChar == '*' && NextChar == '/')
                    {
                        Skip(2);
                        return;
                    }

                    Skip();
                }
            }
        }

        private bool InBounds => _position < _source.Length;
        private char CurrentChar => _source[_position];
        private char NextChar => _source[_position + 1];
        private bool IsComment => CurrentChar == '/' && (NextChar == '/' || NextChar == '*');

        private bool IsStringLiteral => CurrentChar == '\"' || CurrentChar == '\'';

        private bool IsNewLine => CurrentChar == '\n';

        private void Skip(int charCount = 1)
        {
            _position += charCount;
            _column += charCount;
        }

        private void NewLine()
        {
            _line++;
            _column = 1;
        }

        private TokenLocation CurrentTokenLocation => new TokenLocation(_line, _column);
        
        private TokenLocation CalculateTokenLocation(int tokenLength) => new TokenLocation(_line, _column + tokenLength);

        public IEnumerator<Token> GetEnumerator()
        {
            while (true)
            {
                var lastToken = NextToken();
                yield return lastToken;
                
                if(lastToken.Type == TokenType.Eof)
                    yield break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}