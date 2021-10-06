using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    Prototype Carcass to C++ transpiler.
    Status: Unused, deprecated -> Do not use it.

    Additional Files that go into the rule above(^):
        -CPPTrans.cs.
 */
namespace Carcass
{
    public struct CPPTransResult 
    {
        public string result;
        public DateTime ready;

        public CPPTransResult(string _Result)
        {
            result = _Result;
            ready = DateTime.Now;
        }
    }

    public class CPPTrans
    {

        public CPPTransResult Transpile(IReadOnlyList<Statement> stmts)
        {
            StringBuilder _builder = new StringBuilder();
            _builder.AppendLine(CPPSymbols.includeLib + " <iostream>");
            _builder.AppendLine(CPPSymbols.mainFunc);
            _builder.AppendLine("{");
            
            foreach(var s in stmts)
            {
                switch (s.kind)
                {
                    case StatementKind.Print:
                        _builder.AppendLine(CPPSymbols.cout + " " + CPPSymbols.outSymbol + " " + s.value + CPPSymbols.lineEndSymbol);
                        break;
                    case StatementKind.PrintVar:
                        _builder.AppendLine(CPPSymbols.cout + " " + CPPSymbols.outSymbol + " " + s.value + CPPSymbols.lineEndSymbol);
                        break;
                    case StatementKind.Variable:
                        _builder.AppendLine(CPPSymbols.varSymbol+" "+(s as VariableDeclaration).identifier+" "+CPPSymbols.equalSymbol+" "+(s as VariableDeclaration)._value+CPPSymbols.lineEndSymbol);
                        break;
                    default:
                        throw new Exception("FATAL ERROR WHILE TRANSPILING CARCASS CODE: ERROR CODE: 111 -> COULD NOT FIND KIND OF STATEMENT HERE.");
                }
            }
            _builder.AppendLine("}");
            return new CPPTransResult(_builder.ToString());
        }
    }
}
