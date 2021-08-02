using System;
using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class  LocalVariableDeclarationStatement : Statement
    {
        public SyntaxToken Type { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken? EqualsToken { get; }
        public ExpressionsSyntax? Initializer { get; }
        public SyntaxToken Semicolon { get; }

        public LocalVariableDeclarationStatement(
            SyntaxToken type,
            SyntaxToken identifier,
            SyntaxToken? equalsToken,
            ExpressionsSyntax? initializer,
            SyntaxToken semicolon)
        {
            Type = type;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
            Semicolon = semicolon;
            if (equalsToken is null && initializer is not null)
                throw new Exception("Tried to initialize LocalVariableDeclaration statement with partial initialization");
        }


        public override SyntaxKind Kind => SyntaxKind.LocalVariableDeclarationStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Type;
            yield return Identifier;

            if (EqualsToken is not null && Initializer is not null)
            {
                yield return EqualsToken;
                yield return Initializer;
            }

            yield return Semicolon;
        }
    }
}