using System;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }
        public Expression Expression { get; private set; }

        public FunctionCallExpression()
        {
            Args = new List<Expression>();
        }

        public override void Parse(TokensStack sTokens)
        {
            Token tName = sTokens.Pop();
            if(!(tName is Identifier))
                throw new SyntaxErrorException("Expected function name, received " + tName, tName);
            FunctionName = ((Identifier)tName).Name;
               Token tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != "(")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
            while(sTokens.Count > 0)
            {
              Expression = Expression.Create(sTokens);
              Expression.Parse(sTokens);
              Args.Add(Expression);
              if (sTokens.Count > 0 && sTokens.Peek() is Separator)//,
                   sTokens.Pop(); 
              if((sTokens.Peek() is Parentheses) && ((Parentheses)sTokens.Peek()).Name.ToString() == ")")
                        break;
            }
             tOp = sTokens.Pop();
            if (!(tOp is Parentheses) || ((Parentheses)tOp).Name.ToString() != ")")
                throw new SyntaxErrorException("Expected Operator, received " + tOp, tOp);
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}