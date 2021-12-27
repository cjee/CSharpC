using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Binding;

public class BoundBlockStatement : BoundStatment
{
    public ImmutableList<BoundStatment> Statements { get; }

    public BoundBlockStatement(ImmutableList<BoundStatment> statements)
    {
        Statements = statements;
    }
    public override BoundNodeKind Kind => BoundNodeKind.BoundBlockStatement;
}