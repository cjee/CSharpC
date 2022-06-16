using Compiler.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Binding;

public abstract record BoundStatment : BoundNode;

public sealed record BoundBlockStatement(ImmutableList<BoundStatment> Statements) : BoundStatment;
public sealed record BoundEmptyStatement : BoundStatment;
public sealed record BoundExpressionStatement(BoundExpression Expression) : BoundStatment;
public sealed record BoundLocalVariableDeclarationStatement(VariableSymbol Variable, BoundExpression? Initializer) : BoundStatment;
public sealed record BoundReturnStatement(BoundExpression? BoundExpression) : BoundStatment;