using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler
{
    public class ImportDeclaration : Declaration
    {
        public string fileName;

        public ImportDeclaration(string fn)
        {
            fileName = fn;
        }
    }
}
