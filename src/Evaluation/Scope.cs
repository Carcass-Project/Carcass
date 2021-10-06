using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass
{
    public enum ScopeKind
    {
        Function,
        Global,
        Local
    }

    public class Scope
    {
        ScopeKind kind;

    }
}
