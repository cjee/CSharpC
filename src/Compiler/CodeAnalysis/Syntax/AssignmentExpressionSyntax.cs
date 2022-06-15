using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax;

public sealed record AssignmentExpressionSyntax(SyntaxToken Identifier, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax
{

}