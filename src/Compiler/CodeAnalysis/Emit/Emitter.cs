using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;
using System;
using System.IO;
using System.Linq;

namespace Compiler.CodeAnalysis.Emit;

public class Emitter
{
    private readonly TextWriter writer;

    public Emitter(TextWriter writer)
    {
        this.writer = writer;
    }

    public void EmitGlobalScope(BoundProgram program)
    {
        foreach (var method in program.Methods)
        {
            string methodSignature = GenerateMethodsignature(method.Key);
            writer.WriteLine($"{methodSignature};");
        }
    }

    private string ResolveCType(TypeSymbol type)
    {
        if (type == TypeSymbols.Error)
            throw new Exception("Can't  emit built in Error Type");

        if (type == TypeSymbols.Boolean || type == TypeSymbols.Int)
            return "int";

        if (type == TypeSymbols.Void)
            return "void";

        if (type == TypeSymbols.Char)
            return "char";

        throw new Exception($"Trying to emit unknown type {type.Name}");
    }

    internal void EmitBoundProgram(BoundProgram boundProgram)
    {
        foreach ((MethodSymbol symbol, BoundBlockStatement block) in boundProgram.Methods)
        {
            var methodSignature = GenerateMethodsignature(symbol);
            writer.WriteLine(methodSignature);
            EmitStatementBlock(block);
        }
    }

    private void EmitStatementBlock(BoundBlockStatement blockStatement)
    {
        writer.WriteLine("{");

        foreach (var statement in blockStatement.Statements)
        {
            switch (statement)
            {
                case BoundBlockStatement block:
                    EmitStatementBlock(block);
                    break;

                case BoundEmptyStatement:
                    break;

                case BoundExpressionStatement expression:
                    EmitExpression(expression.Expression);
                    writer.WriteLine(";");
                    break;

                case BoundLocalVariableDeclarationStatement localVariableDeclarationStatement:
                    EmitLocalVariableDecalration(localVariableDeclarationStatement);
                    writer.WriteLine(";");
                    break;

                case BoundReturnStatement boundReturnStatement:
                    if (boundReturnStatement.BoundExpression is not null)
                    {
                        writer.Write("return ");
                        EmitExpression(boundReturnStatement.BoundExpression);
                        writer.WriteLine(";");
                    }
                    break;

                default:
                    throw new Exception($"Trying to emit unknown statement {statement.GetType().Name}");
            }
        }

        writer.WriteLine("}");
    }

    private void EmitLocalVariableDecalration(BoundLocalVariableDeclarationStatement localVariableDeclarationStatement)
    {
        var type = ResolveCType(localVariableDeclarationStatement.Variable.Type);
        string variableDeclaration = $"{type} {localVariableDeclarationStatement.Variable.Name} = ";

        writer.Write(variableDeclaration);

        if (localVariableDeclarationStatement.Initializer is null)
        {
            if (localVariableDeclarationStatement.Variable.Type.DefaultInitializer is not null)
                EmitExpression(localVariableDeclarationStatement.Variable.Type.DefaultInitializer);
            else
                throw new Exception($"Trying to emit variable declaration for type that can't be declared: {localVariableDeclarationStatement.Variable.Type}");
        }
        else
            EmitExpression(localVariableDeclarationStatement.Initializer);

    }

    private void EmitExpression(BoundExpression boundExpression)
    {
        switch (boundExpression)
        {
            case BoundAssignmentExpression expression:
                writer.Write($"{expression.Variable.Name} = ");
                EmitExpression(expression.Expression);
                break;

            case BoundBinaryExpression expression:
                writer.Write("(");
                EmitExpression(expression.LeftOperand);
                writer.Write($") {ResolceCOperator(expression.BoundOperator.BoundBinaryOperatorKind)} (");
                EmitExpression(expression.RightOperand);
                writer.Write(")");
                break;

            case BoundBooleanLiteralExpression expression:
                var value = expression.Value ? "1" : "0";
                writer.Write(value);
                break;

            case BoundErrorExpression:
                throw new Exception($"Trying to emit Error expression");

            case BoundIntegralLiteralExpression expression:
                writer.Write(expression.Value.ToString());
                break;

            case BoundCharacterLiteralExpression expression:
                writer.Write($"'{expression.Value}'");
                break;
            case BoundInvocationExpression expression:
                if(expression.Method.Name == "Write")
                {
                    switch(expression.Arguments[0])
                    {
                        case BoundIntegralLiteralExpression arg:
                            writer.Write($"printf(\"{arg.Value}\")");
                            break;
                        case BoundCharacterLiteralExpression arg:
                            writer.Write($"printf(\"{arg.Value}\")");
                            break;
                    }
                }
                else
                {
                    writer.Write(expression.Method.Name);
                    writer.Write("(");
                    if (expression.Arguments.Count > 0)
                    {
                        EmitExpression(expression.Arguments[0]);
                        for (int i = 1; i < expression.Arguments.Count; i++)
                        {
                            writer.Write(", ");
                            EmitExpression(expression.Arguments[i]);
                        }
                    }
                    writer.Write(")");
                }
                break;

            case BoundUnaryExpression expression:
                writer.Write($"{ResolceCOperator(expression.BoundOperator.BoundUnaryOperatorKind)} (");
                EmitExpression(expression.BoundOperand);
                writer.Write(")");
                break;

            case BoundVariableExpression expression:
                writer.Write(expression.Variable.Name);
                break;

            default:
                throw new Exception($"Trying to emit unknown expression {boundExpression.GetType().Name}");
        }
    }

    private string ResolceCOperator(BoundUnaryOperatorKind boundUnaryOperatorKind) => boundUnaryOperatorKind switch
    {
        BoundUnaryOperatorKind.Identity => "",
        BoundUnaryOperatorKind.Negation => SyntaxFacts.MinusTokenString,
        BoundUnaryOperatorKind.LogicalNegation => SyntaxFacts.BangTokenString,
        _ => throw new Exception($"Trying to emit unknown UnaryOperator {boundUnaryOperatorKind}"),
    };

    private string ResolceCOperator(BoundBinaryOperatorKind boundBinaryOperatorKind) => boundBinaryOperatorKind switch
    {
        BoundBinaryOperatorKind.Addition => SyntaxFacts.PlusTokenString,
        BoundBinaryOperatorKind.Subtraction => SyntaxFacts.MinusTokenString,
        BoundBinaryOperatorKind.Multiplication => SyntaxFacts.StarTokenString,
        BoundBinaryOperatorKind.Division => SyntaxFacts.SlashTokenString,
        BoundBinaryOperatorKind.Modulus => SyntaxFacts.PercentTokenString,
        BoundBinaryOperatorKind.Equals => SyntaxFacts.EqualsEqualsTokenString,
        BoundBinaryOperatorKind.NotEquals => SyntaxFacts.BangEqualsTokenString,
        BoundBinaryOperatorKind.Less => SyntaxFacts.LessTokenString,
        BoundBinaryOperatorKind.LessOrEquals => SyntaxFacts.LessOrEqualTokenString,
        BoundBinaryOperatorKind.Greater => SyntaxFacts.GreaterTokenString,
        BoundBinaryOperatorKind.GreaterOrEquals => SyntaxFacts.GreaterOrEqualTokenString,
        _ => throw new Exception($"Trying to emit unknown BinaryOperator {boundBinaryOperatorKind}"),
    };

    private string GenerateMethodsignature(MethodSymbol method)
    {
        var type = ResolveCType(method.Type);
        var methodName = method.Name;
        var parameters = string.Join(", ", method.Parameters.Select(x => $"{ResolveCType(x.Type)} {x.Name}"));
        var methodSignature = $"{type} {methodName}({parameters})";
        return methodSignature;
    }
}