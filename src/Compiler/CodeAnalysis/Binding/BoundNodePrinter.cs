using System;
using System.CodeDom.Compiler;
using System.IO;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public static class BoundNodePrinter
    {
        public static void WriteTo(this BoundNode node, TextWriter writer)
        {
            if (writer is not IndentedTextWriter)
                writer = new IndentedTextWriter(writer, "    ");

            ((IndentedTextWriter) writer).WriteNode(node);
        }

        private static void WriteNode(this IndentedTextWriter writer, BoundNode node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.BinaryExpression:
                    writer.WriteBinaryExpression((BoundBinaryExpression) node);
                    break;
                case BoundNodeKind.UnaryExpression:
                    writer.WriteUnaryExpression((BoundUnaryExpression) node);
                    break;
                case BoundNodeKind.IntegralLiteralExpression:
                    writer.WriteIntegralLiteralExpression((BoundIntegralLiteralExpression) node);
                    break;
                case BoundNodeKind.BooleanLiteralExpression:
                    writer.WriteBooleanLiteralExpression((BoundBooleanLiteralExpression) node);
                    break;
                default:
                    throw new Exception($"Unrecognized bound node kind: {node.Kind}");
            }
        }

        private static void WriteUnaryExpression(this IndentedTextWriter writer, BoundUnaryExpression node)
        {
            var precedence = node.BoundOperator.SyntaxKind.GetUnaryOperatorPrecedence();

            writer.WritePunctuation(node.BoundOperator.SyntaxKind);
            writer.WriteNestedExpression(node.BoundOperand, precedence);
        }

        private static void WriteBinaryExpression(this IndentedTextWriter writer, BoundBinaryExpression node)
        {
            var precedence = node.BoundOperator.SyntaxKind.GetBinaryOperatorPrecedence();

            writer.WriteNestedExpression(node.LeftOperand, precedence);
            writer.WriteSpace();
            writer.WritePunctuation(node.BoundOperator.SyntaxKind);
            writer.WriteSpace();
            writer.WriteNestedExpression(node.RightOperand, precedence);
        }

        private static void WriteIntegralLiteralExpression(this IndentedTextWriter writer,
            BoundIntegralLiteralExpression node)
        {
            writer.Write(node.Value);
        }
        
        private static void WriteBooleanLiteralExpression(this IndentedTextWriter writer,
            BoundBooleanLiteralExpression node)
        {
            writer.Write(node.Value ? "true" : "false");
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, BoundExpression expression,
            int parentPrecedence)
        {
            switch (expression)
            {
                case BoundUnaryExpression unary:
                    writer.WriteNestedExpression(unary, parentPrecedence,
                        unary.BoundOperator.SyntaxKind.GetUnaryOperatorPrecedence());
                    break;
                case BoundBinaryExpression binary:
                    writer.WriteNestedExpression(binary, parentPrecedence,
                        binary.BoundOperator.SyntaxKind.GetBinaryOperatorPrecedence());
                    break;
                default:
                    expression.WriteTo(writer);
                    break;
            }
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, BoundExpression expression,
            int parentPrecedence, int currentPrecedence)
        {
            var needParenthesis = parentPrecedence >= currentPrecedence;
            if (needParenthesis)
                writer.WritePunctuation(SyntaxKind.OpenParenthesis);

            expression.WriteTo(writer);
            if (needParenthesis)
                writer.WritePunctuation(SyntaxKind.CloseParenthesis);
        }

        private static void WritePunctuation(this IndentedTextWriter writer, SyntaxKind kind)
        {
            writer.Write(SyntaxFacts.GetText(kind));
        }

        private static void WriteSpace(this IndentedTextWriter writer)
        {
            writer.Write(" ");
        }
    }
}