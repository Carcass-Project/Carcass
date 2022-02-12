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
        public List<AssemblyDefinition> assemblies = new List<AssemblyDefinition>();

        public object Visit(Expression expr) => expr switch
        {
            StringExpr => this.Visit((expr as StringExpr)),
            PrintExpr => this.Visit((expr as PrintExpr)),
            BinaryExpr => this.Visit((expr as BinaryExpr)),
            LiteralExpr => this.Visit((expr as LiteralExpr)),
            HexLiteralExpr => this.Visit((expr as HexLiteralExpr)),
            _ => throw new NotImplementedException("This expression has not been defined within Carcass, this is a rare error, please tell our team on Github.")
        };
        TypeReference ResolveType(AssemblyDefinition assembly, string metadataName)
        {
            var foundTypes = assemblies.SelectMany(a => a.Modules)
                                       .SelectMany(m => m.Types)
                                       .Where(t => t.FullName == metadataName)
                                       .ToArray();
            if (foundTypes.Length == 1)
            {
                return assembly.MainModule.ImportReference(foundTypes[0]);
            }
            else
            {
                // something went wrong
                return null;
            }
        }

        public object Visit(PrintExpr expr)
        {
            object d = this.Visit(expr.expr);
            var writeline = module.ImportReference(
            typeof(Console).GetMethod("WriteLine", new[] { d.GetType() }));
            ilProcessor.Emit(OpCodes.Call, writeline);
            return d;
        }

        public object Visit(HexLiteralExpr expr)
        {

            byte b = byte.Parse(expr.tok.Text);
            ilProcessor.Emit(OpCodes.Ldc_I4, b);
            return b;
        }

        public enum CalcType { ADD, SUB, MUL, DIV, MOD }
        public int DoCalculations(BinaryExpr bin, CalcType type)
        {
            switch(type)
            {
                case CalcType.ADD:
                    this.Visit(bin.left);
                    this.Visit(bin.right);
                    ilProcessor.Emit(OpCodes.Add);
                    break;
                case CalcType.SUB:
                    this.Visit(bin.left);
                    this.Visit(bin.right);
                    ilProcessor.Emit(OpCodes.Sub);
                    break;
                case CalcType.MUL:
                    this.Visit(bin.left);
                    this.Visit(bin.right);
                    ilProcessor.Emit(OpCodes.Mul);
                    break;
                case CalcType.DIV:
                    this.Visit(bin.left);
                    this.Visit(bin.right);
                    ilProcessor.Emit(OpCodes.Div);
                    break;
            }

            return 1;
        }

        public object DoComparisons(Expression a, TokenType type, Expression b)
        {
            switch (type)
            {
                case TokenType.Greater:
                    var ax = (int)this.Visit(a);
                    var bx = (int)this.Visit(b);

                    //ilProcessor.Emit(OpCodes.Ldc_I4, ax);
                    //ilProcessor.Emit(OpCodes.Ldc_I4, bx);
                    ilProcessor.Emit(OpCodes.Cgt);


                    return ax > bx;
            }

            return false;
        }

        public object Visit(BinaryExpr bin) => bin.op.Kind switch
        {
            TokenType.Plus =>  DoCalculations(bin, CalcType.ADD),
            TokenType.Minus => DoCalculations(bin, CalcType.SUB),
            TokenType.Multi => DoCalculations(bin, CalcType.MUL),
            TokenType.Divide => DoCalculations(bin, CalcType.DIV),
            TokenType.Greater => DoComparisons(bin.left, TokenType.Greater, bin.right),
            TokenType.GreaterEq => (int)this.Visit(bin.left) >= (int)this.Visit(bin.right),
            TokenType.Lower => (int)this.Visit(bin.left) < (int)this.Visit(bin.right),
            TokenType.LowerEq => (int)this.Visit(bin.left) <= (int)this.Visit(bin.right),
            TokenType.EqualTo => this.Visit(bin.left) == this.Visit(bin.right),
            _ => throw new NotImplementedException("This operation is not supported by the Carcass runtime. -> UNCOMMON ERROR <- Ask our team on Github if you think it should be valid!")
        };

        public object Visit(StringExpr expr)
        {
            ilProcessor.Emit(OpCodes.Ldstr, expr.str);
            return expr.str;
        }

        public object Visit(LiteralExpr expr)
        {
            ilProcessor.Emit(OpCodes.Ldc_I4, int.Parse(expr.tok.Text));
            return int.Parse(expr.tok.Text);
        }

        public void Emit(IReadOnlyList<Statement> stmts)
        {
            var assembly = AssemblyDefinition.CreateAssembly(
                    new AssemblyNameDefinition("HelloWorld", new Version(6, 0, 0, 0)), "HelloWorld", ModuleKind.Console);
            



            module = assembly.MainModule;

            assemblies.Add(AssemblyDefinition.ReadAssembly("output\\NETLibraries\\System.Runtime.dll"));
            assemblies.Add(AssemblyDefinition.ReadAssembly("output\\NETLibraries\\System.Runtime.Extensions.dll"));
            assemblies.Add(AssemblyDefinition.ReadAssembly("output\\NETLibraries\\System.Private.CoreLib.dll"));

            var ObjectTypeReference = ResolveType(assembly, "System.Object");
            var VoidTypeReference = ResolveType(assembly, "System.Void");
            
           // module.RuntimeVersion = "v6.0.0.0";
            var programType = new TypeDefinition(
            "Project",
            "Program",
            TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed,
            ObjectTypeReference);

            mainMethod = new MethodDefinition(
            "Main",
            MethodAttributes.Public | MethodAttributes.Static,
            VoidTypeReference);

            programType.Methods.Add(mainMethod);


            module.Types.Add(programType);
            
           /* module.AssemblyReferences.Add(new AssemblyNameReference("System.Private.CoreLib", new Version(6, 0, 0, 0)));
            module.AssemblyReferences.Add(new AssemblyNameReference("System", new Version(6, 0, 0, 0)));
            module.AssemblyReferences.Add(new AssemblyNameReference("System.Runtime", new Version(6, 0, 0, 0)));
            module.AssemblyReferences.Add(new AssemblyNameReference("System.Console", new Version(6, 0, 0, 0)));
            module.AssemblyReferences.Add(new AssemblyNameReference("System.Core", new Version(6, 0, 0, 0)));*/




           

         
            ilProcessor = mainMethod.Body.GetILProcessor();

            

            

            foreach (var x in stmts)
            {
                switch (x.kind)
                {
                    case StatementKind.Expression:
                        this.Visit((x as ExpressionStatement).expr);
                        break;
                    case StatementKind.InlineIL:
                        var inlILStmt = x as StringInlineILStatement;
                        var str = this.Visit(inlILStmt?.ILCode);

                        if (str.GetType() != typeof(StringExpr))
                            throw new ArgumentException("CX007: You must use a string to inline IL with basic inlining.");

                        
                        break;
                }
            }
            ilProcessor.Emit(OpCodes.Nop);
            ilProcessor.Emit(OpCodes.Ret);

            assembly.EntryPoint = mainMethod;
            
            assembly.Write(@"output\HelloWorld.dll");
        }
    }
}
