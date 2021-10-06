using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    //EMITTER. // WIP.
    public class CProcessor
    {
        private StringBuilder _builder;

        /*private string arrayToCSV(string[] arr)
        {
            string str = "";
            for(int i = 0; i < arr.Length; i++)
            {
                str += arr[i];
                if (i < arr.Length - 1)
                    str += ', ';
            }
        }*/

        public void EmitType(object v)
        {
            switch (v)
            {
                case int:
                    _builder.Append("int ");
                    break;
                case string:
                    _builder.Append("char* ");
                    break;
                case float:
                    _builder.Append("float ");
                    break;
                case char:
                    _builder.Append("char ");
                    break;
            }
        }

        public void Emit(Declaration decl)
        {
            switch (decl) 
            {
                case VariableDeclaration:
                     var decl_ = (decl as VariableDeclaration);
                     EmitType(decl_.val);
                    _builder.AppendLine(decl_.name + " = " + decl_.val);
                    break;
                case FuncDeclaration:
                     var declf_ = (decl as FuncDeclaration);
                     EmitType(declf_.retType);
                    _builder.AppendLine(declf_.name + "(");
                    break;
            }
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
        public CProcessor()
        {
            _builder = new StringBuilder();
        }
    }
}
