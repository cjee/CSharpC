using System;
using System.Collections.Generic;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class Binder
    {
        private List<string> diagnostics = new();

        public IEnumerable<string> Diagnostics => diagnostics;

        public BoundExpression BindExpression(ExpressionsSyntax expressionsSyntax)
        {
            
            switch (expressionsSyntax.Kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return BindNumericLiteralExpression((NumericLiteralExpressionSyntax)expressionsSyntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)expressionsSyntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)expressionsSyntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)expressionsSyntax);
                default:
                    throw new Exception($"Unexpected expression syntax kind: {expressionsSyntax.Kind}");
            }
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax expressionsSyntax)
        {
            var boundLeft = BindExpression(expressionsSyntax.Left);
            var boundRight = BindExpression(expressionsSyntax.Right);

            var boundOperator = BoundBinaryOperator.Bind(expressionsSyntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperator == null)
            {
                return boundLeft;
            }

            return new BoundBinaryExpression(boundOperator, boundLeft, boundRight);

        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax expressionsSyntax)
        {
            return BindExpression(expressionsSyntax.Expression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax expressionsSyntax)
        {
            var boundOperand = BindExpression(expressionsSyntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(expressionsSyntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindNumericLiteralExpression(NumericLiteralExpressionSyntax numericLiteralExpressionSyntax)
        {
            if (int.TryParse(numericLiteralExpressionSyntax.NumberToken.Text, out var value))
            {
                return new BoundIntegralLiteralExpression(value);
            } 
            return new BoundIntegralLiteralExpression(0);
        }
    }
}