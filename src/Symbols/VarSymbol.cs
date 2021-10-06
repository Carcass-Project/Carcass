using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;
using Yoakke.Symbols;

namespace Carcass
{
    public class VarSymbol : ISymbol
    {
        public IReadOnlyScope Scope { get; }
        public string Name { get; }

        public object Value { get; set; }

        public VarSymbol(IReadOnlyScope scope, string name, object value)
        {
            Scope = scope;
            Name = name;
            Value = value;
        }
    }
}
