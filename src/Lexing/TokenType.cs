using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke;
using Yoakke.Lexer.Attributes;
using Yoakke.Lexer;

namespace Carcass
{
    public enum TokenType
    {
        [Error] Error,
        [End] End,
        [Ignore] [Regex(@"[ \t\r\n]")] Whitespace,
        [Ignore] [Regex(@"//[^\r\n]*")] LineComment,

        [Token("if")] KwIf,
        [Token("else")] KwElse,
        [Token("print")] KwPrint,
        [Token("var")] KwVar,
        [Token("while")] KwWhile,
        [Token("fn")]  KwFunc,
        [Token("add")] KwAdd,
        [Token("pass")] KwPass,
        [Token("return")] KwReturn,
        [Token("import")] KwImport,
 
        [Regex(@"[A-Za-z_][A-Za-z0-9_]*")] Identifier,
        [Regex(Regexes.StringLiteral)] String,

        [Token("+")] Plus,
        [Token("-")] Minus,
        [Token("/")] Divide,
        [Token("*")] Multi,
        [Token("=")] Equal,
        [Token(":")] Colon,
        [Token(">")] Greater,
        [Token("<")] Lower,
        [Token(">=")] GreaterEq,
        [Token("<=")] LowerEq,
        [Token("==")] EqualTo,

        [Regex(@"[0-9]+")] IntLiteral,
    }

    [Lexer(typeof(TokenType))]
    public partial class Lexer { }
}
