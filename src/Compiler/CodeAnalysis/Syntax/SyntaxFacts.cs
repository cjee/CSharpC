using System;

namespace Compiler.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static string GetText(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => "+",
                SyntaxKind.MinusToken => "-",
                SyntaxKind.StarToken => "*",
                SyntaxKind.SlashToken => "/",
                SyntaxKind.PercentToken => "%",
                SyntaxKind.BangToken => "!",
                SyntaxKind.LessToken => "<",
                SyntaxKind.LessOrEqualToken => "<=",
                SyntaxKind.GreaterToken => ">",
                SyntaxKind.GreaterOrEqualToken => ">=",
                SyntaxKind.BangEqualsToken => "!=",
                SyntaxKind.EqualsEqualsToken => "==",
                SyntaxKind.EqualsToken => "=",
                SyntaxKind.OpenParenthesisToken => "(",
                SyntaxKind.CloseParenthesisToken => ")",
                SyntaxKind.OpenBraceToken => "{",
                SyntaxKind.CloseBraceToken => "}",
                SyntaxKind.SemicolonToken => ";",
                SyntaxKind.DotToken => ".",
                SyntaxKind.CommaToken => ",",
                
                SyntaxKind.FalseKeyword => "false",
                SyntaxKind.TrueKeyword => "true",
                SyntaxKind.VoidKeyword => "void",
                SyntaxKind.BoolKeyword => "bool",
                SyntaxKind.IntKeyword => "int",
                SyntaxKind.ReturnKeyword => "return",
                _ => string.Empty,
            };
        }

        public static SyntaxKind GetKeywordToken(string text)
        {
            return text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "void" => SyntaxKind.VoidKeyword,
                "bool" => SyntaxKind.BoolKeyword,
                "int" => SyntaxKind.IntKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                _ => SyntaxKind.Identifier,
            };
        }
        
        public static int GetUnaryOperatorPrecedence(this SyntaxKind currentKind)
        {
            switch (currentKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.BangToken:
                    return 13; //Based on C# operator count
                default:
                    return 0;
            }
        }
        
        public static int GetBinaryOperatorPrecedence(this SyntaxKind currentKind)
        {
            switch (currentKind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                    return 12;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 11;
                case SyntaxKind.LessToken:
                case SyntaxKind.LessOrEqualToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualToken:
                    return 9;
                case SyntaxKind.BangEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                    return 8;
                case  SyntaxKind.EqualsToken:
                    return 1;
                default:
                    return 0;
            }
        }
        
        public static bool IsBuiltInType(SyntaxKind currentKind)
        {
            return currentKind switch
            {
                SyntaxKind.VoidKeyword => true,
                SyntaxKind.BoolKeyword => true,
                SyntaxKind.IntKeyword => true,
                _ => false,
            };
        }

        public static SyntaxKind GetSyntaxKindFromType<T>(T type) => type switch
        {
            AssignmentExpressionSyntax => SyntaxKind.AssignmentExpression,
            BinaryExpressionSyntax => SyntaxKind.BinaryExpression,
            BlockStatementSyntax => SyntaxKind.BlockStatement,
            BooleanLiteralExpressionSyntax => SyntaxKind.BooleanLiteralExpression,
            CompilationUnit => SyntaxKind.CompilationUnit,
            EmptyStatementSyntax => SyntaxKind.EmptyStatement,
            ExpressionStatementSyntax => SyntaxKind.ExpressionStatement,
            InvocationExpressionSyntax => SyntaxKind.InvocationExpression,
            LocalVariableDeclarationStatementSyntax => SyntaxKind.LocalVariableDeclarationStatement,
            MethodDeclarationSyntax => SyntaxKind.MethodDeclaration,
            NumericLiteralExpressionSyntax => SyntaxKind.NumericLiteralExpression,
            ParameterSyntax => SyntaxKind.Parameter,
            ParenthesizedExpressionSyntax => SyntaxKind.ParenthesizedExpression,
            ReturnStatementSyntax => SyntaxKind.ReturnStatement,
            SimpleNameExpressionSyntax => SyntaxKind.SimpleNameExpression,
            TypeSyntax => SyntaxKind.Type,
            UnaryExpressionSyntax => SyntaxKind.UnaryExpression,
            _ => throw new ArgumentException($"The type {nameof(T)} does not have corresponding SyntaxKind"),
        };
    }
}