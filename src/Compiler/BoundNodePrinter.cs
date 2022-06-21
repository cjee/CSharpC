using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;

namespace Compiler;

public static class BoundNodePrinter
{
    public static void PrintBoundProgram(this BoundProgram boundProgram, TextWriter writer)
    {
        var current = Console.ForegroundColor;
        if (writer is not IndentedTextWriter)
            writer = new IndentedTextWriter(writer, "    ");

        foreach (var (symbol, body) in boundProgram.Methods)
        {
            ((IndentedTextWriter)writer).WriteNode(symbol);
            ((IndentedTextWriter)writer).WriteNode(body);
        }

        Console.ForegroundColor = current;
    }

    private static void WriteNode(this IndentedTextWriter writer, MethodSymbol symbol)
    {
        writer.WriteString(symbol.Type.Name, typeColor);
        writer.WriteSpace();
        writer.WriteString(symbol.Name, identifierColor);
        writer.WriteString(" (", ConsoleColor.White);

        if (symbol.Parameters.Any())
        {
            bool first = true;
            foreach (var param in symbol.Parameters)
            {
                if (!first)
                    writer.WriteString(", ", ConsoleColor.White);

                writer.WriteString(param.Type.Name, typeColor);
                writer.WriteSpace();
                writer.WriteString(param.Name, localColor);

                if (first)
                    first = false;
            }
        }
        writer.WriteString(")\n", ConsoleColor.White);
    }

    private static void WriteNode(this IndentedTextWriter writer, BoundNode node)
    {
        switch (node)
        {
            case BoundBinaryExpression:
                writer.WriteBinaryExpression((BoundBinaryExpression)node);
                break;

            case BoundUnaryExpression:
                writer.WriteUnaryExpression((BoundUnaryExpression)node);
                break;

            case BoundIntegralLiteralExpression:
                writer.WriteIntegralLiteralExpression((BoundIntegralLiteralExpression)node);
                break;

            case BoundBooleanLiteralExpression:
                writer.WriteBooleanLiteralExpression((BoundBooleanLiteralExpression)node);
                break;

            case BoundCharacterLiteralExpression expression:
                writer.WriteCharacterLiteralExpression(expression);
                break;

            case BoundBlockStatement:
                writer.WriteBlockStatement((BoundBlockStatement)node);
                break;

            case BoundLocalVariableDeclarationStatement:
                writer.WriteLocalVariableDeclarationStatement((BoundLocalVariableDeclarationStatement)node);
                break;

            case BoundEmptyStatement:
                writer.WriteString(SyntaxFacts.SemicolonTokenString, ConsoleColor.White);
                break;

            case BoundExpressionStatement:
                writer.WriteBoundExpressions((BoundExpressionStatement)node);
                break;

            case BoundAssignmentExpression:
                writer.WriteAssignmentExpression((BoundAssignmentExpression)node);
                break;

            case BoundErrorExpression:
                writer.WriteErrorExpression((BoundErrorExpression)node);
                break;

            case BoundVariableExpression:
                writer.WriteVariableExpression((BoundVariableExpression)node);
                break;

            case BoundInvocationExpression:
                writer.WriteInvocationExpression((BoundInvocationExpression)node);
                break;

            case BoundReturnStatement:
                writer.WriteReturnStatement((BoundReturnStatement)node);
                break;

            case BoundIfStatement ifStatement:
                writer.WriteIfStatement(ifStatement);
                break;
            default:
                throw new Exception($"Unrecognized bound node type: {node.GetType().Name}");
        }
    }

    private static void WriteReturnStatement(this IndentedTextWriter writer, BoundReturnStatement node)
    {
        writer.WriteString(SyntaxFacts.ReturnKeywordString, keywordColor);
        writer.WriteSpace();
        if (node.BoundExpression is not null)
            writer.WriteNode(node.BoundExpression);
        writer.WriteLine();
    }

    private static void WriteIfStatement(this IndentedTextWriter writer, BoundIfStatement node)
    {
        writer.WriteString(SyntaxFacts.IfKeywordString, keywordColor);
        writer.WriteSpace();
        writer.WriteString(SyntaxFacts.OpenParenthesisTokenString, ConsoleColor.White);
        writer.WriteNode(node.condition);
        writer.WriteString(SyntaxFacts.CloseBraceTokenString, ConsoleColor.White);
        writer.WriteSpace();
        writer.WriteNode(node.TrueBlock);

        if(node.falseBlock is not null)
        {
            writer.WriteString(SyntaxFacts.ElseKeywordString, keywordColor);
            writer.WriteSpace();
            writer.WriteNode(node.falseBlock);
        }
    }

    private static void WriteInvocationExpression(this IndentedTextWriter writer, BoundInvocationExpression node)
    {
        writer.WriteString($"{node.Method.Name}", identifierColor);
        writer.WriteString("(", ConsoleColor.White);

        if (node.Arguments.Count > 0)
        {
            writer.WriteNode(node.Arguments[0]);
            for (int i = 1; i < node.Arguments.Count; i++)
            {
                writer.WriteString(SyntaxFacts.CommaTokenString, ConsoleColor.White);
                writer.WriteSpace();
                writer.WriteNode(node.Arguments[i]);
            }
        }
        writer.WriteString(")", ConsoleColor.White);
    }

    private static void WriteErrorExpression(this IndentedTextWriter writer, BoundErrorExpression node)
    {
        writer.WriteString("_ErrorExpression_", errorColor);
    }

    private static void WriteVariableExpression(this IndentedTextWriter writer, BoundVariableExpression node)
    {
        writer.WriteString(node.Variable.Name, localColor);
    }

    private static void WriteAssignmentExpression(this IndentedTextWriter writer, BoundAssignmentExpression node)
    {
        writer.WriteString($"{node.Variable.Name} ", localColor);
        writer.WriteString(SyntaxFacts.EqualsTokenString, ConsoleColor.White);
        writer.WriteSpace();
        writer.WriteNode(node.Expression);
    }

    private static void WriteBoundExpressions(this IndentedTextWriter writer, BoundExpressionStatement node)
    {
        writer.WriteNode(node.Expression);
    }

    private static void WriteBlockStatement(this IndentedTextWriter writer, BoundBlockStatement node)
    {
        writer.WriteString(SyntaxFacts.OpenBraceTokenString, ConsoleColor.White);
        writer.WriteLine();
        writer.Indent++;

        foreach (var statement in node.Statements)
        {
            writer.WriteNode(statement);
            writer.WriteLine();
        }

        writer.Indent--;
        writer.WriteString(SyntaxFacts.CloseBraceTokenString, ConsoleColor.White);
        writer.WriteLine();
    }

    private static void WriteLocalVariableDeclarationStatement(this IndentedTextWriter writer, BoundLocalVariableDeclarationStatement node)
    {
        writer.WriteString(node.Variable.Type.Name, typeColor);
        writer.WriteSpace();
        writer.WriteString(node.Variable.Name, localColor);
        if (node.Initializer != null)
        {
            writer.WriteString(" = ", ConsoleColor.White);
            writer.WriteNode(node.Initializer);
        }
    }

    private static void WriteUnaryExpression(this IndentedTextWriter writer, BoundUnaryExpression node)
    {
        var precedence = node.BoundOperator.SyntaxString.GetUnaryOperatorPrecedence();

        writer.WriteString(node.BoundOperator.SyntaxString, ConsoleColor.White);
        writer.WriteNestedExpression(node.BoundOperand, precedence);
    }

    private static void WriteBinaryExpression(this IndentedTextWriter writer, BoundBinaryExpression node)
    {
        var precedence = node.BoundOperator.SyntaxString.GetBinaryOperatorPrecedence();

        writer.WriteNestedExpression(node.LeftOperand, precedence);
        writer.WriteSpace();
        writer.WriteString(node.BoundOperator.SyntaxString, ConsoleColor.White);
        writer.WriteSpace();
        writer.WriteNestedExpression(node.RightOperand, precedence);
    }

    private static void WriteIntegralLiteralExpression(this IndentedTextWriter writer,
        BoundIntegralLiteralExpression node)
    {
        writer.WriteString(node.Value.ToString(), integerColor);
    }

    private static void WriteBooleanLiteralExpression(this IndentedTextWriter writer,
        BoundBooleanLiteralExpression node)
    {
        writer.WriteString(node.Value ? "true" : "false", boolColor);
    }

    private static void WriteCharacterLiteralExpression(this IndentedTextWriter writer,
        BoundCharacterLiteralExpression node)
    {
        writer.WriteString($"'{node.Value}'", textColor);
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
                writer.WriteNode(expression);
                break;
        }
    }

    private static void WriteNestedExpression(this IndentedTextWriter writer, BoundExpression expression,
        int parentPrecedence, int currentPrecedence)
    {
        var needParenthesis = parentPrecedence >= currentPrecedence;
        if (needParenthesis)
            writer.WriteString(SyntaxFacts.OpenParenthesisTokenString, ConsoleColor.White);

        writer.WriteNode(expression);
        if (needParenthesis)
            writer.WriteString(SyntaxFacts.CloseParenthesisTokenString, ConsoleColor.White);
    }

    private static void WriteString(this IndentedTextWriter writer, string value, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        writer.Write(value);
    }

    private static void WriteSpace(this IndentedTextWriter writer)
    {
        writer.Write(" ");
    }

    private static ConsoleColor errorColor = ConsoleColor.Red;
    private static ConsoleColor typeColor = ConsoleColor.DarkBlue;
    private static ConsoleColor boolColor = ConsoleColor.DarkBlue;
    private static ConsoleColor identifierColor = ConsoleColor.Yellow;
    private static ConsoleColor localColor = ConsoleColor.Blue;
    private static ConsoleColor integerColor = ConsoleColor.Green;
    private static ConsoleColor keywordColor = ConsoleColor.Magenta;
    private static ConsoleColor textColor = ConsoleColor.DarkMagenta;
}