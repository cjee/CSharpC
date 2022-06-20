using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler;

public class Evaluator
{
    private Dictionary<MethodSymbol, BoundBlockStatement> Methods = new();
    private Stack<Dictionary<VariableSymbol, object>> LocalScopes = new();

    public object? Evaluate(BoundProgram program)
    {
        Methods = new();
        foreach (var (symbol, body) in program.Methods)
            Methods.Add(symbol, body);

        LocalScopes.Push(new());

        //FIXME: Main method should be identified during compile time
        var mainFunction = Methods.First(x => x.Key.Name == "main");

        return EvaluateBody(mainFunction.Value);
    }

    private object? EvaluateBody(BoundBlockStatement body)
    {
        for (int index = 0; index < body.Statements.Count; index++)
        {
            switch (body.Statements[index])
            {
                case BoundBlockStatement block:
                    //FIXME: this should be removed, when nested blocks are folded in
                    Dictionary<VariableSymbol, object> locals = new();
                    foreach (var local in LocalScopes.Peek())
                        locals.Add(local.Key, local.Value);
                    
                    LocalScopes.Push(locals);
                    EvaluateBody(block);
                    LocalScopes.Pop();
                    index++;
                    break;
                case BoundEmptyStatement:
                    index++;
                    break;
                case BoundExpressionStatement expressionStatement:
                    EvaluateExpression(expressionStatement.Expression);
                    break;
                case BoundLocalVariableDeclarationStatement localVariable:
                    object assignmentResult;
                    if(localVariable.Initializer is null)
                    {
                        if (localVariable.Variable.Type.DefaultInitializer is not null)
                            assignmentResult = EvaluateExpression(localVariable.Variable.Type.DefaultInitializer);
                        else
                            throw new Exception($"Default initializer is not defined for type {localVariable.Variable.Type}");
                    }
                    else
                        assignmentResult = EvaluateExpression(localVariable.Initializer);
                    LocalScopes.Peek().Add(localVariable.Variable, assignmentResult);
                    break;
                case BoundReturnStatement returnStatement:
                    if(returnStatement.BoundExpression is null)
                        return null;
                    var returnResult = EvaluateExpression(returnStatement.BoundExpression);
                    return returnResult;
                default:
                    throw new Exception($"Unable to evaluate {body.Statements[index].GetType()} statement");
            }
        }

        return null;
    }

    private object EvaluateExpression(BoundExpression expression)
    {
        switch(expression)
        {
            case BoundAssignmentExpression boundAssignmentExpression:
                var result = EvaluateExpression(boundAssignmentExpression.Expression);
                var scope = LocalScopes.Peek();
                scope[boundAssignmentExpression.Variable] = result;
                return result;
            case BoundBinaryExpression binaryExpression:
                return EvaluateBinaryExpression(binaryExpression);
            case BoundBooleanLiteralExpression boundBooleanLiteral:
                return boundBooleanLiteral.Value;
            case BoundErrorExpression:
                throw new Exception("Trying to execute Bound Error exception");
            case BoundIntegralLiteralExpression integralLiteralExpression:
                return integralLiteralExpression.Value;
            case BoundInvocationExpression invocationExpression:
                return InvokeMethod(invocationExpression);
            case BoundUnaryExpression unaryExpression :
                return EvaluateUnaryExpression(unaryExpression);
            case BoundVariableExpression variable:
                return LocalScopes.Peek()[variable.Variable];
            default:
                throw new Exception($"Unable to evaluate {expression.GetType()} expression");
        }
    }

    private object EvaluateUnaryExpression(BoundUnaryExpression unaryExpression)
    {
        var right = EvaluateExpression(unaryExpression.BoundOperand);

        switch (unaryExpression.BoundOperator.BoundUnaryOperatorKind)
        {
            case BoundUnaryOperatorKind.Identity : {
                if(right is int a)
                    return a;
                }
                break;
            case BoundUnaryOperatorKind.Negation : {
                if(right is int a)
                    return -a;
                }
                break;
            case BoundUnaryOperatorKind.LogicalNegation :{
                if(right is bool a)
                    return !a;
                }
                break;
        }
        throw new Exception($"Operator {unaryExpression.BoundOperator.BoundUnaryOperatorKind} is not supported for type {right.GetType().Name}");
    }

    private object InvokeMethod(BoundInvocationExpression invocationExpression)
    {
        (MethodSymbol method, BoundBlockStatement body) = Methods.First(x => x.Key.Name == invocationExpression.Method.Name);
        Dictionary<VariableSymbol, object> locals = new();
        for (int i = 0; i < method.Parameters.Count; i++)
        {
            locals.Add(method.Parameters[i], EvaluateExpression(invocationExpression.Arguments[i]));
        }

        LocalScopes.Push(locals);
        var result = EvaluateBody(body);

        if(result is null)
            return 0;
        return result;
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression binaryExpression)
    {
        var left = EvaluateExpression(binaryExpression.LeftOperand);
        var right = EvaluateExpression(binaryExpression.RightOperand);

        switch (binaryExpression.BoundOperator.BoundBinaryOperatorKind)
        {
            case BoundBinaryOperatorKind.Addition: {

                if (left is int a && right is int b)
                    return a + b;
                }
                break;
            case BoundBinaryOperatorKind.Subtraction: {
                 if (left is int a && right is int b)
                    return a + b;
                }
                break;
            case BoundBinaryOperatorKind.Multiplication: {
                 if (left is int a && right is int b)
                    return a * b;
                }
                break;
            case BoundBinaryOperatorKind.Division: {
                 if (left is int a && right is int b)
                    return a / b;
                }
                break;
            case BoundBinaryOperatorKind.Modulus: {
                 if (left is int a && right is int b)
                    return a % b;
                }
                break;
            case BoundBinaryOperatorKind.Equals: {
                if (left is int a && right is int b)
                    return a == b;
                if(left is bool c && right is bool d)
                    return c == d;
                }
                break;
            case BoundBinaryOperatorKind.NotEquals: {
                if (left is int a && right is int b)
                    return a != b;
                if(left is bool c && right is bool d)
                    return c != d;
                }
                break;
            case BoundBinaryOperatorKind.Less: {
                 if (left is int a && right is int b)
                    return a < b;
                }
                break;
            case BoundBinaryOperatorKind.LessOrEquals:  {
                 if (left is int a && right is int b)
                    return a <= b;
                }
                break;
            case BoundBinaryOperatorKind.Greater:  {
                 if (left is int a && right is int b)
                    return a > b;
                }
                break;
            case BoundBinaryOperatorKind.GreaterOrEquals: {
                 if (left is int a && right is int b)
                    return a >= b;
                }
                break;
        }
        throw new Exception ($"Operator {binaryExpression.BoundOperator.BoundBinaryOperatorKind} is not supported for types: {left.GetType().Name} and {right.GetType().Name}");
    }
}