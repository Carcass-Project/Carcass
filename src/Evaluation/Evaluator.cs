using System;
using System.Diagnostics;
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
using Raylib_cs;

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
            return (int.Parse((str as string).Replace("\"", "")));
        }
        public void ShowNewWindow(object title, object x, object y)
        {
            Raylib.InitWindow((int)(x as int?), (int)(y as int?), title as string);
        }
        public object strlen(object str)
        {
            return ((str as string).Length);
        }
        public object asString(object num)
        {
            return ((num as int?).ToString());
        }
        public object arrLen(object ident)
        {
            var f = ident;

            if (f is List<object>)
            {
                var fx = (f as List<object>);
                return fx.Count;
            }
            else
            {
                throw new Exception("Invalid variable used in insert, the variable used has to be an array.");
            }
        }
        public void InsertToArray(object ident, object val)
        {
            var f = ident;

            if(f is List<object>)
            {
                var fx = (f as List<object>);

                fx.Add(val);
            }
            else
            {
                throw new Exception("Invalid variable used in insert, the variable used has to be an array.");
            }
        }
        public object JoinStr(object str1, object str2)
        {
            return $"{str1}{str2}";
        }
        Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        Dictionary<string, object> GlobalVariableDict = new Dictionary<string, object> { };
        Dictionary<string, FunctionStatement> FunctionDict = new Dictionary<string, FunctionStatement>();
        Dictionary<string, int> BuiltInFunctions = new Dictionary<string, int>();
        string directory;
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
            ArrayExpr => this.Visit((expr as ArrayExpr)),
            ArrayRefExpr => this.Visit((expr as ArrayRefExpr)),
            UnaryExpr => this.Visit((expr as UnaryExpr)),
            BoolExpr => this.Visit((expr as BoolExpr)),
            _ => throw new NotImplementedException("This expression is not supported by the Carcass runtime. -> RARE ERROR <- Please report to our team on Github.")
        };
        public object Visit(BoolExpr expr) => expr.boolean switch
        {
            TokenType.BTrue => true,
            TokenType.BFalse => false
        };
        public object Visit(UnaryExpr expr) => expr.op.Kind switch
        {
            TokenType.Minus => -(int)this.Visit(expr.a),
            TokenType.Plus => (int)this.Visit(expr.a),
            TokenType.BoolNegate => !(bool)this.Visit(expr.a)
        };
        public object Visit(StringExpr expr)
        {
            return expr.str.Replace("\"", "");
        }
        public object Visit(ArrayExpr expr)
        {
            List<object> stuff = new List<object>();

            foreach (var c in expr.symbols)
            {
                stuff.Add(this.Visit(c));
            }

            return stuff;
        }
        public object Visit(ArrayRefExpr expr)
        {
            return (this.callStack.Peek().Values[expr.ident] as List<object>)[((int)(this.Visit(expr.atPos) as int?))];
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
            TokenType.EqualTo => this.Visit(bin.left) == this.Visit(bin.right),
            _ => throw new NotImplementedException("This operation is not supported by the Carcass runtime. -> UNCOMMON ERROR <- Ask our team on Github if you think it should be valid!")
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
            BuiltInFunctions.Add("strlen", 0);
            BuiltInFunctions.Add("OpenWindow", 0);
            BuiltInFunctions.Add("insert", 0);
            BuiltInFunctions.Add("arrLen", 0);
            BuiltInFunctions.Add("asStr", 0);
            BuiltInFunctions.Add("join", 0);
            BuiltInFunctions.Add("WindowShouldClose", 0);
            BuiltInFunctions.Add("CloseWindow", 0);
            BuiltInFunctions.Add("InitDraw", 0);
            BuiltInFunctions.Add("EndDraw", 0);
            BuiltInFunctions.Add("Start2D", 0);
            BuiltInFunctions.Add("End2D", 0);
            BuiltInFunctions.Add("Start3D", 0);
            BuiltInFunctions.Add("End3D", 0);
            BuiltInFunctions.Add("ClearBG", 0);
            BuiltInFunctions.Add("DrawRectangle", 0);
            BuiltInFunctions.Add("sys", 0);
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
                    if(call.funcName == "strlen")
                    {
                        if (call.args.Count > 0)
                        {
                            throw new Return(strlen(this.Visit(call.args[0])));
                        }
                        else
                        {
                            Console.WriteLine("No instance of strlen uses 0 arguments. It supports only strings.");
                        }
                    }
                    if(call.funcName == "sys")
                    {
                        if (call.args.Count > 0)
                        {
                            Process proc = new Process();
                            proc.StartInfo = new ProcessStartInfo("cmd.exe", "/c " + this.Visit(call.args[0]));

                            proc.Start();
                        }
                        else
                        {
                            Console.WriteLine("No instance of sys uses 0 arguments. It supports a command string.");
                        }
                    }
                    if (call.funcName == "arrLen")
                    {
                        if (call.args.Count > 0)
                        {
                            throw new Return(arrLen(this.Visit(call.args[0])));
                        }
                        else
                        {
                            Console.WriteLine("No instance of arrLen uses 0 arguments. It supports only arrays.");
                        }
                    }
                    if(call.funcName == "WindowOpen")
                    {
                        throw new Return(Raylib.WindowShouldClose());
                    }
                    if(call.funcName == "CloseWindow")
                    {
                        Raylib.CloseWindow();
                    }
                    if(call.funcName == "InitDraw")
                    {
                        Raylib.BeginDrawing();
                    }
                    if (call.funcName == "EndDraw")
                    {
                        Raylib.EndDrawing();
                    }

                    if (call.funcName == "ClearBG")
                    {
                        if (call.args.Count > 0)
                        {
                            Raylib.ClearBackground(Raylib.ColorFromNormalized(new System.Numerics.Vector4((float)this.Visit(call.args[0])/255f, (float)this.Visit(call.args[1])/255f, (float)this.Visit(call.args[2])/255f, (float)this.Visit(call.args[3])/255f)));
                        }
                        else
                        {
                            Raylib.ClearBackground(Color.RAYWHITE);
                            //Console.WriteLine("No instance of ClearBG uses 0 arguments. It supports numbers R, G, B, A.");
                        }
                    }

                    if(call.funcName == "DrawRectangle")
                    {
                        if (call.args.Count > 0)
                        {
                            Raylib.DrawRectangle((int)this.Visit(call.args[0]), (int)this.Visit(call.args[1]), (int)this.Visit(call.args[2]), (int)this.Visit(call.args[3]), Color.RED);
                        }
                        else
                        {
                            Console.WriteLine("No instance of DrawRectangle uses 0 arguments. It supports numbers sizeX, sizeY, X, Y, R, G, B, A.");
                        }
                    }

                    if (call.funcName == "join")
                    {
                        if (call.args.Count > 0)
                        {
                            throw new Return(JoinStr(this.Visit(call.args[0]), this.Visit(call.args[1])));
                        }
                        else
                        {
                            Console.WriteLine("No instance of join uses 0 arguments. It supports String str1, String str2.");
                        }
                    }
                    if (call.funcName == "insert")
                    {
                        if (call.args.Count > 0)
                        {
                            InsertToArray(this.Visit(call.args[0]), this.Visit(call.args[1]));
                        }


                        else
                        {
                            Console.WriteLine("No instance of insert uses 0 arguments. It supports Array identifier, any value.");
                        }
                    }
                    if (call.funcName == "asStr")
                    {
                        if (call.args.Count > 0)
                        {
                            asString(this.Visit(call.args[0]));
                        }
                        else
                        {
                            Console.WriteLine("No instance of asStr uses 0 arguments. It supports only numbers.");
                        }
                    }
                    if (call.funcName == "OpenWindow")
                    {
                        if (call.args.Count > 0)
                        {
                            ShowNewWindow(this.Visit(call.args[0]), this.Visit(call.args[1]), this.Visit(call.args[2]));
                        }
                        else
                        {
                            Console.WriteLine("No instance of OpenWindow uses 0 arguments. It supports string title, number x, number y.");
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
                    case StatementKind.While:
                        while((this.Visit((st as WhileStatement).expr) as bool?) == true)
                        {
                            this.Evaluate((st as WhileStatement).stmt.body);
                        }
                        break;
                    case StatementKind.Expression:
                        var stmt = (st as ExpressionStatement);

                        this.Visit(stmt.expr);
                        break;
                    case StatementKind.With:
                        directory = (st as WithStatement).dir;
                        break;
                    case StatementKind.Import:
                        this.Evaluate(new Parser(new Lexer(System.IO.File.ReadAllText(System.IO.Path.Combine(directory, (st as ImportStatement).fileName.Replace("\"", ""))))).ParseProgram().Ok.Value);
                        break;
                    case StatementKind.SetVar:
                        var t = (st as VarSetStatement);
                        this.callStack.Peek().Values[t.ident] = this.Visit(t.val as Expression);
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
                    
                    /*case  StatementKind.CallFn:
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
            if (left is string || right is string) return (left as string)+(right as string);
            return (int)left + (int)right;
        }
    }
}
