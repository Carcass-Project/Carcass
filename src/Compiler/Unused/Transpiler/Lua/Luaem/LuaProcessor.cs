using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Compiler.Transpiler.Lua.Luaem
{
    public class LuaProcessor
    {
        private StringBuilder _builder;

        public void EmitFunction(string name, params string[] parameters)
        {
            string stencil = "function %name%(%params%)";
            string full = stencil.Replace("%name%", name);
            string allParams = "";

            for(int i=0; i<parameters.Length; i++)
            {
                allParams += parameters[i];
                if (i < parameters.Length - 1)
                    allParams += ",";
            }
            full = full.Replace("%params%", allParams);

            _builder.AppendLine(full);
        }

        public LuaProcessor()
        {
            _builder = new StringBuilder();
        }
    }
}
