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
            switch (node)
            {
                case BoundBinaryExpression:
                    writer.WriteBinaryExpression((BoundBinaryExpression)node);
                    break;
                case BoundUnaryExpression:
                    writer.WriteUnaryExpression((BoundUnaryExpression) node);
                    break;
                case BoundIntegralLiteralExpression:
                    writer.WriteIntegralLiteralExpression((BoundIntegralLiteralExpression) node);
                    break;
                case BoundBooleanLiteralExpression:
                    writer.WriteBooleanLiteralExpression((BoundBooleanLiteralExpression) node);
                    break;
                case BoundBlockStatement:
                    writer.WriteBlockStatement((BoundBlockStatement)node);
                    break;
                case BoundLocalVariableDeclarationStatement:
                    writer.WriteLocalVariableDeclarationStatement((BoundLocalVariableDeclarationStatement)node);
                    break;
                case BoundEmptyStatement:
                    writer.WriteString(SyntaxFacts.SemicolonTokenString);
                    break;
                case BoundExpressionStatement:
                    writer.WriteBoundExpressions((BoundExpressionStatement)node);
                    break;
                case BoundAssignmentExpression:
                    writer.WriteAssignmentExpression((BoundAssignmentExpression)node);
                    break;
                case BoundErrorExpression:
                    writer.WriteErrorExpression(((BoundErrorExpression)node));
                    break;
                case BoundVariableExpression:
                    writer.WriteVariableExpression((BoundVariableExpression)node);
                    break;
                case BoundInvocationExpression:
                    writer.WriteInvocationExpression((BoundInvocationExpression)node) ;
                    break;
                case BoundReturnStatement:
                    writer.WriteReturnStatement((BoundReturnStatement)node) ;
                    break;
                default:
                    throw new Exception($"Unrecognized bound node type: {node.GetType().Name}");
            }
        }

        private static void WriteReturnStatement(this IndentedTextWriter writer, BoundReturnStatement node)
        {
            writer.WriteString(SyntaxFacts.ReturnKeywordString);
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
                    writer.WriteString(SyntaxFacts.CommaTokenString);
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
            writer.WriteString(SyntaxFacts.EqualsTokenString);
            writer.Write(" ");
            writer.WriteNode(node.Expression);
        }

        
        private static void WriteBoundExpressions(this IndentedTextWriter writer, BoundExpressionStatement node)
        {
            writer.WriteNode(node.Expression);
        }
        
        private static void WriteBlockStatement(this IndentedTextWriter writer, BoundBlockStatement node)
        {
            writer.WriteString(SyntaxFacts.OpenBraceTokenString);
            writer.WriteLine();
            writer.Indent++;

            foreach (var statement in node.Statements)
            {
                writer.WriteNode(statement);
                writer.WriteLine();
            }
            
            writer.Indent--;
            writer.WriteString(SyntaxFacts.CloseBraceTokenString);
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
            var precedence = node.BoundOperator.SyntaxString.GetUnaryOperatorPrecedence();

            writer.WriteString(node.BoundOperator.SyntaxString);
            writer.WriteNestedExpression(node.BoundOperand, precedence);
        }

        private static void WriteBinaryExpression(this IndentedTextWriter writer, BoundBinaryExpression node)
        {
            var precedence = node.BoundOperator.SyntaxString.GetBinaryOperatorPrecedence();

            writer.WriteNestedExpression(node.LeftOperand, precedence);
            writer.WriteSpace();
            writer.WriteString(node.BoundOperator.SyntaxString);
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
                        unary.BoundOperator.SyntaxString.GetUnaryOperatorPrecedence());
                    break;
                case BoundBinaryExpression binary:
                    writer.WriteNestedExpression(binary, parentPrecedence,
                        binary.BoundOperator.SyntaxString.GetBinaryOperatorPrecedence());
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
                writer.WriteString(SyntaxFacts.OpenParenthesisTokenString);

            expression.WriteTo(writer);
            if (needParenthesis)
                writer.WriteString(SyntaxFacts.CloseParenthesisTokenString);
        }

        private static void WriteString(this IndentedTextWriter writer, string value)
        {
            writer.Write(value);
        }

        private static void WriteSpace(this IndentedTextWriter writer)
        {
            writer.Write(" ");
        }
    }
}