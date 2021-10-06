using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    public class Parameter
    {
        object val;
        string paramName;

        public Parameter(object v, string prnName)
        {
            val = v;
            paramName = prnName;
        }
    }
}
