using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Values;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;
using Token = Yoakke.Lexer.IToken<Carcass.TokenType>;

namespace Carcass
{
    public static class Helper
    {
        public static List<string> ToStrList(IReadOnlyList<IToken<TokenType>> s)
        {
            List<string> b = new List<string>();
            foreach(var t in s)
            {
                b.Add(t.Text);
            }
            return b;
        }
    }
    [Parser(typeof(TokenType))]
    public partial class PrintFunction
    {
        public string toPrint;

        public PrintFunction(string _toPrint)
        {
            toPrint = _toPrint;
        }
    }
    public enum StatementKind
    {
        Print,
        PrintVar,
        If,
        Else,
        Switch,
        Variable,
        Block,
        Function,
        CallFn,
        Expression,
        Return,
        Import
    }
    #nullable enable
    public class Statement
    {
        public object? value;
        public StatementKind kind;

        public Statement(object? _value, StatementKind _kind)
        {
            value = _value;
            kind = _kind;
        }
        public Statement()
        {

        }
    }
    public enum ExpressionKind
    {
        Binary,
        Literal,
        Variable,
        Call,
        If,
    }
    public abstract class Expression { public ExpressionKind kind; }
    public class BinaryExpr : Expression { public Expression left; public Token op; public Expression right; public BinaryExpr(Expression _left, Token _op, Expression _right) { left = _left; op = _op; right = _right; kind = ExpressionKind.Binary; } }
    public class LiteralExpr : Expression { public Token tok; public LiteralExpr(Token tk) { tok = tk; kind = ExpressionKind.Literal; } }
    public class VarExpr : Expression { public string? identifier; public Expression? expr; public VarExpr(string ident, Expression ex) { identifier = ident; expr = ex; kind = ExpressionKind.Variable; } }
    public class CallFunction : Expression
    {
        public string? funcName;
        public IReadOnlyValueList<Expression>? args;

        public CallFunction(string fnName, IReadOnlyValueList<Expression> _args)
        {
            funcName = fnName;
            args = _args;
        }
    }
    public class VarLiteralExpr : Expression
    {
        public string? identifier;

        public VarLiteralExpr(string ident)
        {
            identifier = ident;
        }
    }

    public class ImportStatement : Statement
    {
        public string fileName;

        public ImportStatement(string fLn)
        {
            fileName = fLn;
            kind = StatementKind.Import;
        }
    }

    public class IfStatement : Statement
    {
        public Expression expr;
        public BlockStatement stmt;

        public IfStatement(Expression ex, BlockStatement stmtt)
        {
            expr = ex;
            stmt = stmtt;
            kind = StatementKind.If;
        }
    }
    public class ReturnStatement : Statement
    {
        public Expression expr;

        public ReturnStatement(Expression ex)
        {
            expr = ex;
            kind = StatementKind.Return;
        }
    }

    public class PrintExpr : Expression { public Expression expr; public PrintExpr(Expression ex) { expr = ex; } }
    public class InputExpr : Expression { public string msg; public InputExpr(string _msg) { msg = _msg; } }
    public class VariableDeclaration : Statement
    {
        public string? identifier;
        public object? _value;

        public VariableDeclaration(string identif, object val)
        {
            identifier = identif;
            _value = val;
            kind = StatementKind.Variable;
        }
    }
    public class BlockStatement : Statement
    {
        public IReadOnlyList<Statement> body;

        public BlockStatement(IReadOnlyList<Statement> bd)
        {
            body = bd;
            kind = StatementKind.Block;
        }
    }
    
    public class StringExpr : Expression { public string str; public StringExpr(string st) { str = st; } }

    public class ExpressionStatement : Statement
    {
        public Expression expr;

        public ExpressionStatement(Expression _expr)
        {
            expr = _expr;
            kind = StatementKind.Expression;
        }
    }
    public class FunctionStatement : Statement
    {
        public string? name;
        public BlockStatement BlockStmt;
        public IReadOnlyValueList<string> parameters;

        public FunctionStatement(string nm, BlockStatement blck, IReadOnlyValueList<string> param)
        {
            name = nm;
            BlockStmt = blck;
            parameters = param;
            kind = StatementKind.Function;
        }
    }
    [Parser(typeof(TokenType))]
    public partial class Parser

    {
        [Rule("program: stmt*")]
        private static IReadOnlyList<Statement> Program(IReadOnlyList<Statement> stmtList)
        {
            return stmtList;
        }
        [Rule("stmt: KwFunc Identifier'(' (Identifier (',' Identifier)*)? ')' block_stmt")]
        private static Statement FunctionStatm(Token _1, Token name,
            Token _2, Punctuated<Token, Token> paramList, Token _3,
            BlockStatement body)
        {
            return new FunctionStatement(name.Text,body, paramList.Values.Select(t => t.Text).ToList().ToValue());
        }

        [Rule("stmt: KwPrint String")]
        private static Statement PrintStatement(IToken<TokenType> type, IToken<TokenType> str)
        {
            return new Statement(str.Text, StatementKind.Print);
        }
        [Rule("expression: KwPrint expression")]
        private static Expression PrintSStatement(IToken<TokenType> type, Expression str)
        {
            return new PrintExpr(str);
        }
        [Rule("stmt: KwPrint Identifier")]
        private static Statement PrintVarStatement(IToken<TokenType> type, IToken<TokenType> ident)
        {
            return new Statement(ident.Text, StatementKind.PrintVar);
        }
        [Rule("stmt: KwImport String")]
        private static Statement ImportStatement(Token kw, Token str)
        {
            return new ImportStatement(str.Text);
        }
        [Rule("stmt: KwVar Identifier Equal String")]
        private static Statement VariableStrStatement(IToken<TokenType> type, IToken<TokenType> ident, IToken<TokenType> equal, IToken<TokenType> str)
        {
            return new VariableDeclaration(ident.Text, str.Text);
        }
        [Rule("expression: KwVar Identifier Equal expression")]
        private static Expression VariableExprStatement(IToken<TokenType> type, IToken<TokenType> ident, IToken<TokenType> equal, Expression intlit)
        {
            return new VarExpr(ident.Text, intlit);
        }
        [Rule("expression : Identifier")]
        private static Expression VarLit(Token ident)
        {
            return new VarLiteralExpr(ident.Text);
        }
        [Rule("block_stmt: '{' stmt* '}'")]
        private static BlockStatement BlockStmt(IToken<TokenType> _1, IReadOnlyList<Statement> statements, IToken<TokenType> _2)
        {
            return new BlockStatement(statements);
        }
        [Rule("expression : Identifier'('(expression(',' expression) *) ? ')'")]
        public static Expression callFunction(Token name, Token _1, Punctuated<Expression, Token> argsList, Token _2)
        {
            return new CallFunction(name.Text, argsList.Values.Select(t => t).ToList().ToValue());
        }
        [Rule("stmt: expression")]
        private static Statement ExprStmt(Expression expr)
        {
            return new ExpressionStatement(expr);
        }
        [Rule("stmt: KwReturn expression?")]
        private static Statement Return(Token kw, Expression expr)
        {
            return new ReturnStatement(expr);
        }
        [Rule("expression: String")]
        private static Expression strth(Token str)
        {
            return new StringExpr(str.Text);
        }
        [Right("^")]
        [Left("*", "/", "%")]
        [Left("+", "-")]
        [Left(">=","<=",">","<")]
        [Left("==")]
        [Rule("expression")]
        public static Expression BinOp(Expression a, Token op, Expression b)
        {
            return new BinaryExpr(a, op, b);
        }
        [Rule("stmt: KwIf expression block_stmt")]
        public static Statement If(Token tk, Expression expr, BlockStatement block)
        {
            return new IfStatement(expr, block);
        }
        [Rule("expression : '(' expression ')'")]
        public static Expression Grouping(IToken _1, Expression n, IToken _2) => n;

        [Rule("expression : IntLiteral")]
        public static Expression IntLiteral(IToken<TokenType> token) { return new LiteralExpr(token); }

       
    }
}
