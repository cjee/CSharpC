using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class SeperatedSyntaxList<T> : IEnumerable<T> where  T: SyntaxNode
    {
        private readonly ImmutableList<SyntaxNode> nodesAndSeparators;

        internal SeperatedSyntaxList(ImmutableList<SyntaxNode> nodesAndSeparators)
        {
            this.nodesAndSeparators = nodesAndSeparators;
        }

        public T this[int index] => (T) nodesAndSeparators[index * 2];

        public int Count => (this.nodesAndSeparators.Count + 1) / 2;
        
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0 ; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static SeperatedSyntaxList<T> Empty() => new(ImmutableList<SyntaxNode>.Empty);
  
    }
}