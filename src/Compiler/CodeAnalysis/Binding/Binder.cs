using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class Binder
    {
        public MethodSymbol? Method { get; }
        private DiagnosticBag diagnostics = new();

        public DiagnosticBag Diagnostics => diagnostics;

        private BoundScope scope;
        private Binder(BoundScope parentScope, MethodSymbol? method)
        {
            Method = method;
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
                binder.BindMethodDeclaration(method);
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
                
                //FIXME: type check does not work
                // no flow logic hence this is enough for now
                if (method.Type != TypeSymbol.Void && body.Statements.OfType<BoundReturnStatement>().Count() == 0)
                    diagnostics.ReportNoReturnStatement(method.Declaration.MemberName);
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
            return syntax switch
            {
                LocalVariableDeclarationStatementSyntax => BindLocalVariableDeclarationStatement((LocalVariableDeclarationStatementSyntax)syntax),
                BlockStatementSyntax => BindBlockStatement((BlockStatementSyntax)syntax),
                ExpressionStatementSyntax => BindExpressionStatement((ExpressionStatementSyntax)syntax),
                ReturnStatementSyntax => BindReturnStatement((ReturnStatementSyntax)syntax),
                EmptyStatementSyntax => new BoundEmptyStatement(),
                _ => throw new Exception($"Unexpected syntax {syntax.NodeName}"),
            };
        }

        private BoundStatment BindReturnStatement(ReturnStatementSyntax syntax)
        {
            // As we don't have global statements yet then we should not be able to invoke
            // this without passing Method parameter to Binder constructor
            if (Method is null)
            {
                throw new Exception("Trying to bind return statement outside of method body");
            }
            var boundExpression = syntax.Expression is null ? null : BindExpression(syntax.Expression);

            if (Method.Type == TypeSymbol.Void)
            {
                if (boundExpression != null)
                    Diagnostics.ReportInvalidReturnExpression(syntax.Expression!, Method!);

            }
            else
            {
                if (boundExpression == null)
                    Diagnostics.ReportMissingReturnExpression(syntax.ReturnKeyword, Method.Type);
                else
                {
                    if (Method.Type != boundExpression.Type)
                        Diagnostics.ReportInvalidReturnExpression(syntax.Expression!, Method!, boundExpression.Type);
                }
            }

            return new BoundReturnStatement(boundExpression);
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
            
            // Variable should be bound before the expression;
            var variable = BindVariable(syntax.Identifier, type);
            var initializer = syntax.Initializer != null ? BindExpression(syntax.Initializer) : type.DefaultInitializer;

            if (initializer is not null && initializer.Type != variable.Type)
                Diagnostics.ReportCannotAssignType(syntax.Initializer!, type, initializer.Type);
            
            
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

        private void BindMethodDeclaration(MethodDeclarationSyntax syntax)
        {
            var parameters = ImmutableList.CreateBuilder<ParameterSymbol>();
            var seenParameterNames = new HashSet<string>();

            foreach (var parameter in syntax.Parameters)
            {
                var parameterName = parameter.Identifier.Text;
                var parameterType = BindTypeClause(parameter.Type);

                if (parameterType == TypeSymbol.Void)
                    diagnostics.ReportInvalidParameterType(parameter.Identifier, parameterType);
                
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
            switch (expressionSyntax)
            {
                case NumericLiteralExpressionSyntax syntax:
                    return BindNumericLiteralExpression(syntax);
                case BooleanLiteralExpressionSyntax syntax:
                    return BindBooleanLiteralExpression(syntax);
                case UnaryExpressionSyntax syntax:
                    return BindUnaryExpression(syntax);
                case BinaryExpressionSyntax syntax:
                    return BindBinaryExpression(syntax);
                case ParenthesizedExpressionSyntax syntax:
                    return BindParenthesizedExpression(syntax);
                case AssignmentExpressionSyntax syntax:
                    return BindAssignmentExpression(syntax);
                case InvocationExpressionSyntax syntax:
                    return BindInvocationExpression(syntax);
                case SimpleNameExpressionSyntax syntax:
                    return BindSimpleNameExpression(syntax);
                default:
                    throw new Exception($"Unexpected expression syntax kind: {expressionSyntax.NodeName}");
            }
        }

        private BoundExpression BindInvocationExpression(InvocationExpressionSyntax syntax)
        {
            if (syntax.PrimaryExpression is not SimpleNameExpressionSyntax nameSyntax)
            {
                Diagnostics.ReportInvalidMethodInvocation(syntax.PrimaryExpression);
                return new BoundErrorExpression();
            }

            var name = nameSyntax.Identifier.Text;

            var boundArguments = ImmutableList.CreateBuilder<BoundExpression>();
            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            var method = scope.TryLookupMethod(name);
            if (method is null)
            {
                Diagnostics.ReportUndefinedName(nameSyntax.Identifier);
                return new BoundErrorExpression();
            }

            if (syntax.Arguments.Count != method.Parameters.Count)
            {
                Diagnostics.ReportArgumentCountMismatch(nameSyntax.Identifier, method.Parameters.Count, syntax.Arguments.Count);
                return new BoundErrorExpression();
            }

            for (var i = 0; i < syntax.Arguments.Count; i++)
            {
                var argument = boundArguments[i];
                var parameter = method.Parameters[i];
                if (argument.Type != parameter.Type)
                {
                    diagnostics.ReportWrongArgumentType(syntax.Arguments[i].Span, parameter.Name, parameter.Type,
                        argument.Type);
                    return new BoundErrorExpression();
                }
            }


            return new BoundInvocationExpression(method, boundArguments.ToImmutable());
        }

        private BoundExpression BindSimpleNameExpression(SimpleNameExpressionSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            if (string.IsNullOrEmpty(name))
                return new BoundErrorExpression();
            
            if(!scope.TryLookupVariable(name, out VariableSymbol variable))
            {
                diagnostics.ReportUndefinedName(syntax.Identifier);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable);
        }

        private BoundAssignmentExpression BindAssignmentExpression(
            AssignmentExpressionSyntax syntax)
        {
            var variableName = syntax.Identifier.Text;
            if (!scope.TryLookupVariable(variableName, out VariableSymbol variable))
            {
                diagnostics.ReportUndefinedName(syntax.Identifier);
            }

            var expression = BindExpression(syntax.Right);

            if (variable.Type != TypeSymbol.Error && expression is not BoundErrorExpression)
            {
                if(variable.Type != expression.Type)
                    diagnostics.ReportCannotAssignType(syntax.Right, variable.Type, expression.Type);
            }
            
            return new BoundAssignmentExpression(variable, expression);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax expressionsSyntax)
        {
            var boundLeft = BindExpression(expressionsSyntax.Left);
            var boundRight = BindExpression(expressionsSyntax.Right);

            var boundOperator = BoundBinaryOperator.Bind(expressionsSyntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
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
            var boundOperator = BoundUnaryOperator.Bind(expressionsSyntax.OperatorToken.Text, boundOperand.Type);
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
            bool value = expressionsSyntax.BooleanToken is TrueKeyword;
            return new BoundBooleanLiteralExpression(value);
        }
    }
}