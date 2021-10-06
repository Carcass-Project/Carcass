using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    class CallDeclaration : Declaration
    {
        public string name;
        public object[] args;

        public CallDeclaration(string nm, object[] argss)
        {
            name = nm;
            args = argss;
        }
    }
}
