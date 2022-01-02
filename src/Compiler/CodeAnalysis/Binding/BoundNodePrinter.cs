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
                case BoundNodeKind.BoundBlockStatement:
                    writer.WriteBlockStatement((BoundBlockStatement)node);
                    break;
                case BoundNodeKind.BoundLocalVariableDeclarationStatement:
                    writer.WriteLocalVariableDeclarationStatement((BoundLocalVariableDeclarationStatement)node);
                    break;
                case BoundNodeKind.BoundEmptyStatement:
                    writer.WriteToken(SyntaxKind.SemicolonToken);
                    break;
                case BoundNodeKind.BoundExpressionStatement:
                    writer.WriteBoundExpressions((BoundExpressionStatement)node);
                    break;
                case BoundNodeKind.AssignmentExpression:
                    writer.WriteAssignmentExpression((BoundAssignmentExpression)node);
                    break;
                case BoundNodeKind.ErrorExpression:
                    writer.WriteErrorExpression(((BoundErrorExpression)node));
                    break;
                case BoundNodeKind.VariableExpression:
                    writer.WriteVariableExpression((BoundVariableExpression)node);
                    break;
                case BoundNodeKind.InvocationExpression:
                    writer.WriteInvocationExpression((BoundInvocationExpression)node) ;
                    break;
                case BoundNodeKind.ReturnStatement:
                    writer.WriteReturnStatement((BoundReturnStatement)node) ;
                    break;
                default:
                    throw new Exception($"Unrecognized bound node kind: {node.Kind}");
            }
        }

        private static void WriteReturnStatement(this IndentedTextWriter writer, BoundReturnStatement node)
        {
            writer.WriteToken(SyntaxKind.ReturnKeyword);
            writer.Write(" ");
            if(node.BoundExpression is not null)
                writer.WriteNode(node.BoundExpression);
            writer.WriteLine();
        }

        private static void WriteInvocationExpression(this IndentedTextWriter writer, BoundInvocationExpression node)
        {
            writer.Write($"{node.Method.Name}(");

            if (node.Arguments.Count > 0)
            {
                writer.WriteNode(node.Arguments[0]);
                for (int i = 1; i < node.Arguments.Count; i++)
                {
                    writer.WriteToken(SyntaxKind.CommaToken);
                    writer.Write(" ");
                    writer.WriteNode(node.Arguments[i]);
                }
            }
            writer.Write(")");
        }
        
        private static void WriteErrorExpression(this IndentedTextWriter writer, BoundErrorExpression node)
        {
            writer.Write("_ErrorExpression_");
        }

        private static void WriteVariableExpression(this IndentedTextWriter writer, BoundVariableExpression node)
        {
            writer.Write(node.Variable.Name);
        }
        
        private static void WriteAssignmentExpression(this IndentedTextWriter writer, BoundAssignmentExpression node)
        {
            writer.Write($"{node.Variable.Name} ");
            writer.WriteToken(SyntaxKind.EqualsToken);
            writer.Write(" ");
            writer.WriteNode(node.Expression);
        }

        
        private static void WriteBoundExpressions(this IndentedTextWriter writer, BoundExpressionStatement node)
        {
            writer.WriteNode(node.Expression);
        }
        
        private static void WriteBlockStatement(this IndentedTextWriter writer, BoundBlockStatement node)
        {
            writer.WriteToken(SyntaxKind.OpenBraceToken);
            writer.WriteLine();
            writer.Indent++;

            foreach (var statement in node.Statements)
            {
                writer.WriteNode(statement);
                writer.WriteLine();
            }
            
            writer.Indent--;
            writer.WriteToken(SyntaxKind.CloseBraceToken);
            writer.WriteLine();
        }

        private static void WriteLocalVariableDeclarationStatement(this IndentedTextWriter writer, BoundLocalVariableDeclarationStatement node)
        {
            writer.Write($"{node.Variable.Type.Name} {node.Variable.Name}");
            if (node.Initializer != null)
            {
                writer.Write(" = ");
                writer.WriteNode(node.Initializer);
            }
        }
        
        private static void WriteUnaryExpression(this IndentedTextWriter writer, BoundUnaryExpression node)
        {
            var precedence = node.BoundOperator.SyntaxKind.GetUnaryOperatorPrecedence();

            writer.WriteToken(node.BoundOperator.SyntaxKind);
            writer.WriteNestedExpression(node.BoundOperand, precedence);
        }

        private static void WriteBinaryExpression(this IndentedTextWriter writer, BoundBinaryExpression node)
        {
            var precedence = node.BoundOperator.SyntaxKind.GetBinaryOperatorPrecedence();

            writer.WriteNestedExpression(node.LeftOperand, precedence);
            writer.WriteSpace();
            writer.WriteToken(node.BoundOperator.SyntaxKind);
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
                writer.WriteToken(SyntaxKind.OpenParenthesisToken);

            expression.WriteTo(writer);
            if (needParenthesis)
                writer.WriteToken(SyntaxKind.CloseParenthesisToken);
        }

        private static void WriteToken(this IndentedTextWriter writer, SyntaxKind kind)
        {
            writer.Write(SyntaxFacts.GetText(kind));
        }
        
        private static void WriteSpace(this IndentedTextWriter writer)
        {
            writer.Write(" ");
        }
    }
}