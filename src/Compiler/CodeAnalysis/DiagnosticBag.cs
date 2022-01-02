using System.Collections;
using System.Collections.Generic;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis
{
    public class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics = new();
        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag bag) => diagnostics.AddRange(bag.diagnostics);

        private void Report(TextSpan span, string message) => diagnostics.Add(new Diagnostic(span, message));

        public void ReportBadCharacter(int position, char current)
        {
            var span = new TextSpan(position, 1);
            var message = $"Unexpected character '{current}'";
            Report(span, message);
        }

        public void ReportBadToken(int start, int length, string text)
        {
            var span = new TextSpan(start, length);
            var message = $"Unrecognized keyword: {text}";
            Report(span, message);
        }

        public void ReportBadIntegral(int start, int length)
        {
            var span = new TextSpan(start, length);
            var message = "Integral constant is too large";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind currentKind, SyntaxKind expected)
        {
            var message = $"Unexpected token <{currentKind}>, expected <{expected}>";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(SyntaxToken operatorToken, TypeSymbol boundLeftType,
            TypeSymbol boundRightType)
        {
            var message =
                $"Binary operator '{SyntaxFacts.GetText(operatorToken.Kind)}' is not defined for type '{boundLeftType.Name}' and '{boundRightType.Name}'";
            Report(operatorToken.TextSpan, message);
        }

        public void ReportUndefinedUnaryOperator(SyntaxToken operatorToken, TypeSymbol boundOperandType)
        {
            var message =
                $"Unary operator '{SyntaxFacts.GetText(operatorToken.Kind)}' is not defined for type '{boundOperandType.Name}'";
            Report(operatorToken.TextSpan, message);
        }

        public void ReportIdentifierExpected(TextSpan span)
        {
            var message = "Identifier expected.";
            Report(span, message);
        }

        public void ReportMissingExpression(TextSpan span)
        {
            var message = "Expected an expression";
            Report(span, message);
        }

        public void ReportParameterAlreadyDeclared(SyntaxToken parameterIdentifier)
        {
            var message =
                $"Parameter with name '{parameterIdentifier.Text}' is already declared'";
            Report(parameterIdentifier.TextSpan, message);
        }

        public void ReportUndefinedType(TypeSyntax typeSyntax)
        {
            var message =
                $"Undefined type: '{typeSyntax.Identifier.Text}'";
            Report(typeSyntax.Identifier.TextSpan, message);
        }

        public void ReportMethodAlreadyDeclared(SyntaxToken syntaxMemberName)
        {
            var message =
                $"Method with name '{syntaxMemberName.Text}' is already declared'";
            Report(syntaxMemberName.TextSpan, message);
        }

        public void ReportCannotDeclareVariableWithTypeVoid(SyntaxToken typeIdentifier)
        {
            const string message = $"Cannot declare variable with type 'void'";
            Report(typeIdentifier.TextSpan, message);
        }

        public void ReportVariableAlreadyDeclared(SyntaxToken identifier)
        {
            var message =
                $"Variable with name '{identifier.Text}' is already declared'";
            Report(identifier.TextSpan, message);
        }

        public void ReportOnlyAssignmentOrInvocationExpressionAllowed(ExpressionSyntax syntaxExpression)
        {
            const string message =
                $"Only assignment and call expressions can be used as a statement";
            Report(syntaxExpression.Span, message);
        }

        public void ReportUndefinedName(SyntaxToken identifier)
        {
            var message =
                $"The name '{identifier.Text}' does not exist in current context'";
            Report(identifier.TextSpan, message);
        }

        public void ReportInvalidMethodInvocation(ExpressionSyntax expression)
        {
            const string message = $"Invalid method invocation";
            
            Report(expression.Span, message);
        }

        public void ReportArgumentCountMismatch(SyntaxToken identifier, int parametersCount, int argumentsCount)
        {
            var message =
                $"The method '{identifier.Text}' requires {parametersCount}' parameter{ (parametersCount == 1 ? "" : "s") } but invocation has {argumentsCount} parameter{ (argumentsCount == 1 ? "" : "s")}";
            Report(identifier.TextSpan, message);
        }

        public void ReportWrongArgumentType(TextSpan span, string parameterName, TypeSymbol parameterType, TypeSymbol argumentType)
        {
            var message = $"Parameter '{parameterName}' requires a value of type '{parameterType.Name}' but was given a value of type '{argumentType.Name}'.";
            Report(span, message);
        }

        public void ReportInvalidReturnExpression(ExpressionSyntax expression, MethodSymbol method)
        {
            var message =
                    $"Since function '{method.Name}' does not return value the 'return' keyword cant be followed by an expression";
            Report(expression.Span, message);
        }
        
        public void ReportInvalidReturnExpression(ExpressionSyntax expression, MethodSymbol method, TypeSymbol type)
        {
            var message =
                    $"Function '{method.Name}' has return type '{method.Type.Name}' but returned expression is of type '{type.Name}'";
            
            Report(expression.Span, message);
        }

        public void ReportMissingReturnExpression(SyntaxToken syntaxReturnKeyword, TypeSymbol methodType)
        {
            var message = $"A return expression of type '{methodType.Name}' is expected";
            Report(syntaxReturnKeyword.TextSpan, message);
        }

        public void ReportNoReturnStatement(SyntaxToken declarationMemberName)
        {
            var message = $"A function '{declarationMemberName.Text}' does not return value";
            Report(declarationMemberName.TextSpan, message);
        }
    }
}