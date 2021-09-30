using System;
using System.Linq;
using System.Collections.Generic;
using Yoakke;
using Yoakke.Collections.Values;
using Carcass;

namespace CarcassI
{
    class Program
    {
        public static string ElementsToStr(IReadOnlyDictionary<string, Yoakke.Parser.ParseErrorElement> dict)
        {
            string s = "";
            foreach (var c in dict.Keys)
            {
                s += dict[c].Context + " Expected: " + dict[c].Expected.ToString();
            }
            return s;
        }

        public static void Main(string[] args)
        {
            if (args[0] == "cmd")
                while (true)
                {
                    var start = DateTime.Now.Millisecond;
                    Console.Write(">> ");
                    var Lexer = new Lexer(Console.ReadLine());
                    var Parser = new Parser(Lexer);
                    var Evaluator = new Evaluator();
                    Evaluator.callStack.Push(new StackFrame());
                    var Result = Parser.ParseProgram();


                    if (Result.IsOk)
                    {
                        Console.WriteLine("Parsed Sucessfully.");
                        Evaluator.Evaluate(Result.Ok.Value);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error Parsing Carcass code.");
                        Console.WriteLine("Got: " + Result.Error.Got.ToString() + " at Position " + Result.Error.Position + " " + ElementsToStr(Result.Error.Elements));
                        Console.ResetColor();
                    }
                    var end = DateTime.Now.Millisecond;
                    Console.Write("Finished all in " + (end - start) + " ms.");
                }
            if (args[0] == "run")
            {
                if (System.IO.File.Exists(args[1]))
                {
                    var start = DateTime.Now.Millisecond;
                    var Lexer = new Lexer(System.IO.File.ReadAllText(args[1]));
                    var Parser = new Parser(Lexer);
                    var Evaluator = new Evaluator();
                    Evaluator.callStack.Push(new StackFrame());
                    var Result = Parser.ParseProgram();


                    if (Result.IsOk)
                    {
                        Console.WriteLine("Parsed Sucessfully.");
                        Evaluator.Evaluate(Result.Ok.Value);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error Parsing Carcass code.");
                        Console.WriteLine("Got: " + Result.Error.Got.ToString() + " at Position " + Result.Error.Position + " " + ElementsToStr(Result.Error.Elements));
                        Console.ResetColor();
                    }
                    var end = DateTime.Now.Millisecond;
                    Console.Write("Finished all in " + (end - start) + " ms.");
                }
                else
                {
                    throw new Exception("Incorrect file name using the run command. Please specify a valid Carcass file.");
                }
            }
        }
    }
}
