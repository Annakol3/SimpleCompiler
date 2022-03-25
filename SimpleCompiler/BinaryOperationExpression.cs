using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operand1 + " " + Operator + " " + Operand2 + ")";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "(")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            Operand1 = Expression.Create(sTokens);
            Operand1.Parse(sTokens);
             tOp=sTokens.Pop();
            if(!(tOp is Operator) || ((Operator)tOp).Name.ToString() == "!")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            Operator = ((Operator)tOp).Name.ToString();
            Operand2 = Expression.Create(sTokens);
            Operand2.Parse(sTokens);
             tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != ")")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);

        }
    }
}
