using Compiler.CodeAnalysis.Binding;

namespace Compiler.CodeAnalysis.FlowAnalysis;

public class ReturnAnalysis
{
    public static bool BaundStatementReturns(BoundStatment statement)
    {
        switch(statement)
        {
            case BoundReturnStatement:
                return true;
            case BoundBlockStatement block:
                return BlockStatementAllPathsReturn(block);
            case BoundIfStatement ifStatement:
                return IfStatementAllPathsReturn(ifStatement);
            default:
                return false;
        }
    }

    private static bool BlockStatementAllPathsReturn(BoundBlockStatement block)
    {
        foreach (var statement in block.Statements)
        {
           if(ReturnAnalysis.BaundStatementReturns(statement))
                return true;
        }
        return false;
    }

    private static bool IfStatementAllPathsReturn(BoundIfStatement ifStatement)
    {
        bool trueState = false;
        bool falseState = false;
        //TODO: check if expression is always true
        if (ifStatement.falseBlock is not null) //not worth checking if both blocks are not present
        {
            trueState = ReturnAnalysis.BaundStatementReturns(ifStatement.TrueBlock);
            falseState = ReturnAnalysis.BaundStatementReturns(ifStatement.falseBlock);
        }
        return trueState && falseState;
    }
}
