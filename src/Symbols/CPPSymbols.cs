using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass
{
    public static class CPPSymbols
    {
        //Preprocessor:
        public static string includeLib { get { return "#include"; } }
        public static string macroDef { get { return "#define"; } }
        //Entry point function:
        public static string mainFunc { get { return "int main()"; } }
        //Scope symbols:
        public static string scopeStart { get { return "{"; } }
        public static string scopeEnd { get { return "}"; } }
        //I/O symbols:
        public static string cout { get { return "std::cout"; } }
        public static string cin { get { return "std::cin"; } }
        public static string scanf { get { return "scanf"; } }
        public static string outSymbol { get { return "<<"; } }
        public static string inSymbol { get { return ">>"; } }
        //Line-end symbol:
        public static string lineEndSymbol { get { return ";"; } }
        //Type symbols:
        public static string intSymbol { get { return "int"; } }
        public static string stringSymbol { get { return "std::string"; } }
        public static string varSymbol { get { return "std::any"; } }
        //Regular symbols:
        public static string equalSymbol { get { return "="; } }
        public static string bracketLeft { get { return "("; } }
        public static string bracketRight { get { return ")"; } }
    }
}
