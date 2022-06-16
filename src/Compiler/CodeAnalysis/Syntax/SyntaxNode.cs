using Compiler.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.CodeAnalysis.Syntax;

public abstract record SyntaxNode
{
    public string NodeName => this.GetType().Name;

    public IEnumerable<SyntaxNode> GetChildren()
    {
        var result = new List<SyntaxNode>();

        foreach (var property in this.GetType().GetProperties())
        {
            var propertyType = property.PropertyType;

            if (propertyType.IsSubclassOf(typeof(SyntaxNode)))
            {
                var value = property.GetValue(this) as SyntaxNode;
                if (value is not null)
                    result.Add(value);
            }

            if (propertyType.IsAssignableTo(typeof(IEnumerable<SyntaxNode>)))
            {
                var value = property.GetValue(this) as IEnumerable<SyntaxNode>;
                if (value is not null)
                {
                    foreach (var item in value)
                    {
                        result.Add(item);
                    }
                }
            }
        }
        return result;
    }

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