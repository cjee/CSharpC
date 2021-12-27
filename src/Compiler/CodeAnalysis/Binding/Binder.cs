using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class Binder
    {
        private DiagnosticBag diagnostics = new();

        public DiagnosticBag Diagnostics => diagnostics;

        private BoundScope scope = new BoundScope(null);
        
        public static BoundGlobalScope BindGlobalScope(CompilationUnit syntax)
        {
            var binder = new Binder();

            foreach (var method in syntax.Methods)
            {
                binder.BindFunctionDeclaration(method);
            }

            var methods = binder.scope.GetDeclaredMethods();
            return new BoundGlobalScope(binder.Diagnostics, methods);
        }

        private void BindFunctionDeclaration(MethodDeclarationSyntax syntax)
        {
            var parameters = ImmutableList.CreateBuilder<ParameterSymbol>();
            var seenParameterNames = new HashSet<string>();

            foreach (var parameter in syntax.Parameters)
            {
                var parameterName = parameter.Identifier.Text;
                var parameterType = BindTypeClause(parameter.Type);
                if (!seenParameterNames.Add(parameterName))
                    diagnostics.ReportParameterAlreadyDeclared(parameter.Identifier);
                else
                {
                    var parameterSymbol = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(parameterSymbol);
                }
            }

            var methodType = BindTypeClause(syntax.ReturnType);
            var method = new MethodSymbol(syntax.MemberName.Text, methodType, parameters.ToImmutableList(), syntax);
            if (!scope.TryDeclareMethod(method))
                diagnostics.ReportMethodAlreadyDeclared(syntax.MemberName);
        }

        private TypeSymbol BindTypeClause(TypeSyntax typeSyntax)
        {
            var type = LookopType(typeSyntax.Identifier.Text);
            if (type == TypeSymbol.Error) 
                diagnostics.ReportUndefinedType(typeSyntax);
            return type;
        }

        private TypeSymbol LookopType(string name)
        {
            return name switch
            {
                "bool" => TypeSymbol.Boolean,
                "int" => TypeSymbol.Int,
                "void" => TypeSymbol.Void,
                _ => TypeSymbol.Error
            };
        }

        public BoundExpression BindExpression(ExpressionsSyntax expressionsSyntax)
        {
            
            switch (expressionsSyntax.Kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return BindNumericLiteralExpression((NumericLiteralExpressionSyntax)expressionsSyntax);
                case SyntaxKind.BooleanLiteralExpression:
                    return BindBooleanLiteralExpression((BooleanLiteralExpressionSyntax) expressionsSyntax);
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
                diagnostics.ReportUndefinedBinaryOperator(expressionsSyntax.OperatorToken, boundLeft.Type,
                    boundRight.Type);
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
                diagnostics.ReportUndefinedUnaryOperator(expressionsSyntax.OperatorToken, boundOperand.Type);
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
        
        private BoundExpression BindBooleanLiteralExpression(BooleanLiteralExpressionSyntax expressionsSyntax)
        {
            bool value = expressionsSyntax.BooleanToken.Kind == SyntaxKind.TrueKeyword;
            return new BoundBooleanLiteralExpression(value);
        }
    }
}