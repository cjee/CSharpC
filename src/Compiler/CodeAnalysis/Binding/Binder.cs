using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class Binder
    {
        private DiagnosticBag diagnostics = new();

        public DiagnosticBag Diagnostics => diagnostics;

        private BoundScope scope;
        private Binder(BoundScope parentScope, MethodSymbol? method)
        {
            scope = new BoundScope(parentScope);
            if (method != null)
            {
                foreach (var parameter in method.Parameters)
                {
                    scope.TryDeclareLocalVariable(parameter);
                }
            }
        }

        public static BoundGlobalScope BindGlobalScope(CompilationUnit syntax)
        {
            var binder = new Binder(new BoundScope(null), null);

            foreach (var method in syntax.Methods)
            {
                binder.BindFunctionDeclaration(method);
            }

            var methods = binder.scope.GetDeclaredMethods();
            return new BoundGlobalScope(binder.Diagnostics, methods);
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);

            var methodBodies
                = ImmutableDictionary.CreateBuilder<MethodSymbol, BoundBlockStatement>();
            var diagnostics = new DiagnosticBag();

            foreach (var method in globalScope.Methods)
            {
                var binder = new Binder(parentScope, method);
                var body = binder.BindBlockStatement(method.Declaration.Body);
                
                methodBodies.Add(method, body);
                diagnostics.AddRange(binder.diagnostics);
            }

            return new BoundProgram(diagnostics, methodBodies.ToImmutable());
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableList.CreateBuilder<BoundStatment>();

            scope = new BoundScope(scope);
            
            foreach (var statement in syntax.Statements)
            {
                var boundStatement = BindStatement(statement);
                statements.Add(boundStatement);
            }

            scope = scope.Parent!;
            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatment BindStatement(StatementSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.LocalVariableDeclarationStatement => BindLocalVariableDeclarationStatement((LocalVariableDeclarationStatementSyntax)syntax),
                SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
                SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
                SyntaxKind.EmptyStatement => new BoundEmptyStatement(),
                _ => throw new Exception($"Unexpected syntax {syntax.Kind}"),
            };
        }

        private BoundStatment BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            if (syntax.Expression is not AssignmentExpressionSyntax && syntax.Expression is not InvocationExpressionSyntax)
            {
                diagnostics.ReportOnlyAssignmentOrInvocationExpressionAllowed(syntax.Expression);
                return new BoundEmptyStatement();
            }

            var expression = BindExpression(syntax.Expression);
            
            return new BoundExpressionStatement(expression);
        }

        private BoundLocalVariableDeclarationStatement BindLocalVariableDeclarationStatement(
            LocalVariableDeclarationStatementSyntax syntax)
        {
            var type = BindTypeClause(syntax.Type);
            if (type == TypeSymbol.Void)
            {
                diagnostics.ReportCannotDeclareVariableWithTypeVoid(syntax.Type.Identifier);
            }
            if (type == TypeSymbol.Error)
            {
                diagnostics.ReportUndefinedType(syntax.Type);
            }
            var initializer = syntax.Initializer != null ? BindExpression(syntax.Initializer) : type.DefaultInitializer;

            var variable = BindVariable(syntax.Identifier, type);
            return new BoundLocalVariableDeclarationStatement(variable, initializer);

        }

        private VariableSymbol BindVariable(SyntaxToken identifier, TypeSymbol type)
        {
            var variable = new VariableSymbol(identifier.Text, type);
            if (!scope.TryDeclareLocalVariable(variable))
            {
                diagnostics.ReportVariableAlreadyDeclared(identifier);
            }

            return variable;
        }

        private static BoundScope CreateParentScope(BoundGlobalScope globalScope)
        {
            var scope = new BoundScope(null);
            foreach (var method in globalScope.Methods)
            {
                scope.TryDeclareMethod(method);
            }

            return scope;
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

        public BoundExpression BindExpression(ExpressionSyntax expressionSyntax)
        {
            
            switch (expressionSyntax.Kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return BindNumericLiteralExpression((NumericLiteralExpressionSyntax)expressionSyntax);
                case SyntaxKind.BooleanLiteralExpression:
                    return BindBooleanLiteralExpression((BooleanLiteralExpressionSyntax) expressionSyntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)expressionSyntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)expressionSyntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)expressionSyntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax);
                case SyntaxKind.InvocationExpression:
                    throw new NotImplementedException();
                default:
                    throw new Exception($"Unexpected expression syntax kind: {expressionSyntax.Kind}");
            }
        }

        private BoundAssignmentExpression BindAssignmentExpression(
            AssignmentExpressionSyntax syntax)
        {
            var variableName = syntax.Identifier.Text;
            if (!scope.TryLookupVariable(variableName, out VariableSymbol variable))
            {
                
            }

            var expression = BindExpression(syntax.Right);
            return new BoundAssignmentExpression(variable, expression);
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