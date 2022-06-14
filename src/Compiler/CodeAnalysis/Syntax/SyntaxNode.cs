using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax
{
    public abstract record SyntaxNode
    {
        public virtual SyntaxKind Kind => SyntaxFacts.GetSyntaxKindFromType(this);
        public abstract IEnumerable<SyntaxNode> GetChildren();

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
    }
}