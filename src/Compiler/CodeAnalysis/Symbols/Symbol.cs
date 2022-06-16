using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Symbols;

public abstract record Symbol(string Name);

public record MethodSymbol(string Name, TypeSymbol Type, ImmutableList<ParameterSymbol> Parameters, MethodDeclarationSyntax Declaration) : Symbol(Name);
public record ParameterSymbol(string Name, TypeSymbol Type) : VariableSymbol(Name, Type);
public record TypeSymbol(string Name, BoundExpression? DefaultInitializer) : Symbol(Name);
public record VariableSymbol(string Name, TypeSymbol Type) : Symbol(Name);