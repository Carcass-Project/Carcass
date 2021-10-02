using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using Yoakke.SyntaxTree;
using Yoakke.SyntaxTree.Attributes;
using Yoakke.Text;
using Yoakke.Symbols;

namespace Carcass
{
    public partial class UnknownFunctionException : Exception
    {
        public UnknownFunctionException()
        {
        }

        public UnknownFunctionException(string message)
            : base(message)
        {
        }

        public UnknownFunctionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class StackFrame
    {
        public readonly Dictionary<string, object> Values = new();
    }

    public partial class Evaluator
    {
        private class Return : Exception
        {
            public readonly object? Value;

            public Return(object? value)
            {
                this.Value = value;
            }
        }

        object InputFn() //UNUSED.
        {
            throw new Return(Console.ReadLine());
        }
        public object StrToIntFn(object str)
        {
            throw new Return(int.Parse(str as string));
        }
        Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        Dictionary<string, object> GlobalVariableDict = new Dictionary<string, object> { };
        Dictionary<string, FunctionStatement> FunctionDict = new Dictionary<string, FunctionStatement>();
        Dictionary<string, int> BuiltInFunctions = new Dictionary<string, int>();
        private readonly StackFrame globalFrame = new();
        public readonly Stack<StackFrame> callStack = new();

        public object Visit(Expression expr) => expr switch
        {
            BinaryExpr => this.Visit((expr as BinaryExpr)),
            LiteralExpr => this.Visit((expr as LiteralExpr)),
            VarExpr => this.Visit((expr as VarExpr)),
            CallFunction => this.Visit((expr as CallFunction)),
            PrintExpr => this.Visit((expr as PrintExpr)),
            VarLiteralExpr => this.Visit((expr as VarLiteralExpr)),
            InputExpr => this.Visit((expr as InputExpr)),
            StringExpr => this.Visit((expr as StringExpr)),
            _ => throw new NotImplementedException("This expression is not supported by the Carcass runtime. -> RARE ERROR <- Please report to our team on Github.")
        };
        public object Visit(StringExpr expr)
        {
            return expr.str;
        }
        public object Visit(InputExpr expr)
        {
            if(expr.msg != null)
            {
                Console.Write(expr.msg);
            }
            throw new Return(Console.ReadLine());
        }
        public object Visit(PrintExpr print)
        {
            var c = this.Visit(print.expr);
            Console.WriteLine(c);
            return c;
        }
#nullable enable
        public object? Visit(VarLiteralExpr ex)
        {
            return this.callStack.Peek().Values[ex.identifier];        
        }
        public object Visit(BinaryExpr bin) => bin.op.Kind switch
        {
            TokenType.Plus => PerformAdd(this.Visit(bin.left), (int)this.Visit(bin.right)),
            TokenType.Minus => (int)this.Visit(bin.left) - (int)this.Visit(bin.right),
            TokenType.Multi => (int)this.Visit(bin.left) * (int)this.Visit(bin.right),
            TokenType.Divide => (int)this.Visit(bin.left) / (int)this.Visit(bin.right),
            TokenType.Greater => (int)this.Visit(bin.left) > (int)this.Visit(bin.right),
            TokenType.GreaterEq => (int)this.Visit(bin.left) >= (int)this.Visit(bin.right),
            TokenType.Lower => (int)this.Visit(bin.left) < (int)this.Visit(bin.right),
            TokenType.LowerEq => (int)this.Visit(bin.left) <= (int)this.Visit(bin.right),
            TokenType.EqualTo => (int)this.Visit(bin.left) == (int)this.Visit(bin.right),
            _ => throw new NotImplementedException("This operation is not supported by the Carcass runtime. -> COMMON ERROR <- Ask our team on Github if you think it should be valid!")
        };
        public object Visit(LiteralExpr lit)
        {
            return int.Parse(lit.tok.Text);
        }
        public object Visit(VarExpr exp)
        {
            var evalVal = this.Visit(exp.expr);
            BindVariable(exp.identifier, evalVal);
            return evalVal;
        }
        protected void Visit(FunctionStatement stmt)
        { /*no-op*/

        }
        public Evaluator()
        {
            BuiltInFunctions.Add("input", 0);
            BuiltInFunctions.Add("asInt", 0);
        }
        protected object? Visit(CallFunction call)
        {
            FunctionStatement c;
            int ccout;
            if (FunctionDict.TryGetValue(call.funcName, out c))
            {
                var func = FunctionDict[call.funcName];
                var args = call.args.Select(this.Visit).ToArray();

                if (func is FunctionStatement)
                {
                    if (func.parameters.ToArray().Length < args.Length)
                    {
                        throw new InvalidOperationException("Argument Count Mismatch. ->");
                    }
                    else if (func.parameters.ToArray().Length > args.Length)
                    {
                        throw new InvalidOperationException("Argument Count Mismatch. <-");
                    }

                    var frame = new StackFrame();
                    this.callStack.Push(frame);

                    int i = 0;
                    object? returnValue = null;

                    foreach (var f in args)
                    {
                        frame.Values.Add(func.parameters[i], f);
                        i++;
                    }
                    try
                    {
                        this.Evaluate(func.BlockStmt.body);
                    }
                    catch (Return ret)
                    {
                        returnValue = ret.Value;
                    }
                    this.callStack.Pop();
                    return returnValue;
                }
            }
            
            else if (BuiltInFunctions.TryGetValue(call.funcName, out ccout))
            {               
                object? returnValue = null;
                try
                {
                    if (call.funcName == "input")
                        InputFn();
                    if(call.funcName == "asInt")
                    {
                        if(call.args.Count > 0)
                        {
                            throw new Return(StrToIntFn(this.Visit(call.args[0])));
                        }
                        else
                        {
                            Console.WriteLine("No instance of asInt uses 0 arguments. It supports only strings.");
                        }
                    }
                }
                catch (Return ret)
                {
                    returnValue = ret.Value;
                }
                return returnValue;
            }

            return null;

        }

        
        public void Evaluate(IReadOnlyList<Statement> stmts)
        {
            List<Statement> stmtss = new List<Statement>();
            stmtss.AddRange(stmts);
            
            foreach (Statement st in stmtss)
            {
                
                switch (st.kind)
                {
                    case StatementKind.Print:
                        Console.WriteLine(st.value);
                        break;
                    case StatementKind.PrintVar:
                        object value = 0;
                        if (callStack.Peek().Values.TryGetValue((st.value as string), out value))
                        {
                            Console.WriteLine(callStack.Peek().Values[(st.value as string)]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error evaluating AST: No such variable \"" + st.value + "\".");
                            Console.ResetColor();
                        }
                        break;
                    case StatementKind.Variable:
                        this.callStack.Peek().Values.Add((st as VariableDeclaration).identifier, (st as VariableDeclaration)._value);
                        break;
                    case StatementKind.Function:
                        var s = (st as FunctionStatement);
                        FunctionDict.Add(s.name, s);
                        break;
                    case StatementKind.Expression:
                        var stmt = (st as ExpressionStatement);

                        this.Visit(stmt.expr);
                        break;
                    case StatementKind.Import:
                        stmtss.AddRange(new Parser(new Lexer(System.IO.File.ReadAllText((st as ImportStatement).fileName))).ParseProgram().Ok.Value);
                        break;
                    case StatementKind.If:
                        var iff = (st as IfStatement);

                        var x = this.Visit(iff.expr) as bool?;
                        if(x == true) { this.Evaluate(iff.stmt.body); }
                        else { continue; }
                        break;
                    case StatementKind.Return:
                        var ret = (st as ReturnStatement);
                        throw new Return(ret.expr == null ? null : this.Visit(ret.expr));
                    /*case StatementKind.CallFn:
                        var a = (st as CallFunction);
                        var f = FunctionDict[a.funcName];
                        int i = 0;
                        foreach(var c in a.args)
                        {
                            
                            VariableDict.Add(f.parameters[i], VariableDict[c]);
                            i++;
                        }
                        this.Evaluate(f.BlockStmt.body);
                        break;*/
                }
            }
        }
        public void BindVariable(string ident, object val)
        {
            this.callStack.Peek().Values.Add(ident, val);
        }
        public object PerformAdd(object left, object right)
        {
            if (left is string || right is string) return $"{left}{right}";
            return (int)left + (int)right;
        }
    }
}
