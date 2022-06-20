using Compiler;
using System;
using System.Collections.Generic;

Console.WriteLine($"CSharpC Compiler v{typeof(Program).Assembly.GetName().Version}");

if(args.Length == 0)
    Interactive.LaunchInteractive();
else
    Engine.Run(HandleConsoleArguments(args));


Arguments HandleConsoleArguments(string[] args)
{
    Arguments arguments = new();
    foreach (var arg in args)
    {
        switch(arg)
        {
            case "-eval":
                arguments.Options.Add(Options.Eval);
                break;
            default:
                if(arg.EndsWith(".cs"))
                    arguments.Files.Add(arg);
                else
                    throw new Exception($"Unrecogized argument ({arg})");
                break;
        }
    }
    return arguments;
}

public class Arguments
{
    public List<string> Files = new();
    public List<Options> Options = new();
}

public enum Options
{
    Eval,
}