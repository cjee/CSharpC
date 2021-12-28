using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

internal class BoundLocalVariableDeclarationStatement : BoundStatment
{
    public VariableSymbol Variable { get; }
    public BoundExpression? Initializer { get; }

    public BoundLocalVariableDeclarationStatement(VariableSymbol variable, BoundExpression? initializer)
    {
        Variable = variable;
        Initializer = initializer;
    }

    public override BoundNodeKind Kind => BoundNodeKind.BoundLocalVariableDeclarationStatement;
}