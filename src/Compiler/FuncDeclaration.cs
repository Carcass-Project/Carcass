using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    public interface Declaration { }
    public class FuncDeclaration : Declaration
    {
        public string name;
        public object retType;
        public string[] parameters;

        public FuncDeclaration(object retType, string nm, string[] prms)
        {
            name = nm;
            parameters = prms;
        }
    }
}
