using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;

/* WORK IN PROGRESS */
namespace Carcass
{
    public class Emitter
    {
        public ILProcessor ilProcessor;
        public MethodDefinition mainMethod;
        public ModuleDefinition module;
        public object Visit(Expression expr) => expr switch
        {
            StringExpr => this.Visit((expr as StringExpr)),
            PrintExpr => this.Visit((expr as PrintExpr)),
            _ => throw new NotImplementedException("This expression has not been defined within Carcass, this is a rare error, please tell our team on Github.")
        };

        public object Visit(PrintExpr expr)
        {
            object d = this.Visit(expr.expr);
            var writeline = module.ImportReference(
            typeof(Console).GetMethod("WriteLine", new[] { d.GetType() }));
            ilProcessor.Emit(OpCodes.Call, writeline);
            return d;
        }

        public object Visit(StringExpr expr)
        {
            ilProcessor.Emit(OpCodes.Ldstr, expr.str);
            return expr.str;
        }

        public object Visit(LiteralExpr expr)
        {
            ilProcessor.Emit(OpCodes.Ldc_I8, int.Parse(expr.tok.Text));
            return int.Parse(expr.tok.Text);
        }

        public void Emit(IReadOnlyList<Statement> stmts)
        {
            var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Project", new Version()), "Project", ModuleKind.Console);
            
            module = assembly.MainModule;
            var programType = new TypeDefinition(
            "Project",
            "Program",
            TypeAttributes.Class | TypeAttributes.Public,
            module.ImportReference(typeof(object)));

            mainMethod = new MethodDefinition(
            "Main",
            MethodAttributes.Public | MethodAttributes.Static,
            module.ImportReference(typeof(void)));

            programType.Methods.Add(mainMethod);

            module.Types.Add(programType);
           

            ilProcessor = mainMethod.Body.GetILProcessor();


            foreach (var x in stmts)
            {
                switch (x.kind)
                {
                    case StatementKind.Expression:
                        Console.WriteLine(this.Visit((x as ExpressionStatement).expr));
                        break;
                }
            }
            ilProcessor.Emit(OpCodes.Pop);
            ilProcessor.Emit(OpCodes.Ret);

            assembly.EntryPoint = mainMethod;
            assembly.Write(@"HelloWorld.exe");
        }
    }
}
