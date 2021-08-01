using System.Collections;
using System.Collections.Generic;
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

        public void ReportBadCharacter(int position, char current)
        {
            var span = new TextSpan(position, 1);
            var message = $"Unexpected character '{current}'";
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
        
        private void Report(TextSpan span, string message) => diagnostics.Add(new Diagnostic(span, message));

        public void ReportUndefinedBinaryOperator(SyntaxToken operatorToken, TypeSymbol boundLeftType, TypeSymbol boundRightType)
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
    }
}