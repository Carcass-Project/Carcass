using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    public class VariableDeclaration : Declaration
    {
        public string name;
        public object val;

        public VariableDeclaration(string nm, object v)
        {
            name = nm;
            val = v;
        }
    }
}
