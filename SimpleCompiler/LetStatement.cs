using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token tlet = sTokens.Pop();
            if (!(tlet is Statement) || ((Statement)tlet).Name != "let")
                throw new SyntaxErrorException("Expected Statement received: " + tlet, tlet);
            Token tvar = sTokens.Pop();
            if (!(tvar is Identifier))
                throw new SyntaxErrorException("Expected Identifier received: " + tvar, tvar);
            Variable= ((Identifier)tvar).Name;
            Token tOp = sTokens.Pop();
            if (!(tOp is Operator) || ((Operator)tOp).Name.ToString() != "=")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            Value = Expression.Create(sTokens);
            Value.Parse(sTokens);
            Token tEnd = sTokens.Pop();
            if (!(tEnd is Separator) || ((Separator)tEnd).Name.ToString() != ";")
              throw new SyntaxErrorException("Expected Operator, received " + tEnd, tEnd);
        }

    }
}
