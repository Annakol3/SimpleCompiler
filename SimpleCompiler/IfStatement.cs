using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class IfStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }

        public IfStatement()
        {
            DoIfTrue = new List<StatetmentBase>();
            DoIfFalse = new List<StatetmentBase>();
        }
        public override void Parse(TokensStack sTokens)
        {
            Token tif = sTokens.Pop();
            if (!(tif is Statement) || ((Statement)tif).Name != "if")
                throw new SyntaxErrorException("Expected Statement received: " + tif, tif);
            Token tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "(")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            Term = Expression.Create(sTokens);
            Term.Parse(sTokens);
            tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != ")")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "{")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                s.Parse(sTokens);
                DoIfTrue.Add(s);
            }
             tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "}")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            Token telse = sTokens.Peek();
            if (telse.ToString()=="else")
              {
            telse = sTokens.Pop();
            if (!(telse is Statement) || ((Statement)telse).Name != "else")
                throw new SyntaxErrorException("Expected Statement received: " + telse, telse);
            tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "{")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                s.Parse(sTokens);
                DoIfFalse.Add(s);
            }
             tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "}")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse.Count > 0)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }
            return sIf;
        }

    }
}
