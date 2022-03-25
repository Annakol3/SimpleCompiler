using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {
       


        public Compiler()
        {

        }


        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for(int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }
            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while(sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);

            }
            return lParsed;
        }

 

        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
             
            List<string> lAssembly = new List<string>();
            if(aSimple.Value is BinaryOperationExpression && !(((BinaryOperationExpression)aSimple.Value).Operand1 is NumericExpression) && !(((BinaryOperationExpression)aSimple.Value).Operand2 is NumericExpression))
             {
                String str = ((BinaryOperationExpression)aSimple.Value).Operand1.ToString();
               // str = str.Substring(1,str.IndexOf(' ')-1);
                String str2 =((BinaryOperationExpression)aSimple.Value).Operand2.ToString();
              //  str2 = str2.Substring(str2.IndexOf(' ')+1);
               // str2 = str.Substring(str2.IndexOf(' ')+1,str2.Length-2-str2.IndexOf(' '));
             if(!dSymbolTable.ContainsKey(str) || !dSymbolTable.ContainsKey(str2) )
              throw new SyntaxErrorException("This name not exists", new Identifier( str,0,0));
               lAssembly.AddRange(CopytoOp(dSymbolTable[str],dSymbolTable[str2],((BinaryOperationExpression)aSimple.Value).Operator));
             }
            else if (aSimple.Value is BinaryOperationExpression && (((BinaryOperationExpression)aSimple.Value).Operand1 is NumericExpression))
            {
              int something = 0;
                String str = ((BinaryOperationExpression)aSimple.Value).Operand1.ToString();
                //str = str.Substring(1,str.IndexOf(' ')-1);
                Int32.TryParse(str, out  something );
                String str2 =((BinaryOperationExpression)aSimple.Value).Operand2.ToString();
               // str2 = str2.Substring(str2.IndexOf(' ')+1);
               // str2 = str.Substring(str2.IndexOf(' ')+1,str2.Length-2-str2.IndexOf(' '));
               if(!dSymbolTable.ContainsKey(str2) )
              throw new SyntaxErrorException("This name not exists", new Identifier( str2,0,0));
                lAssembly.AddRange(CopytoOpwithNumberOp1(dSymbolTable[str2],something,((BinaryOperationExpression)aSimple.Value).Operator));
            }
            else if (aSimple.Value is BinaryOperationExpression && (((BinaryOperationExpression)aSimple.Value).Operand2 is NumericExpression))
           {
                 int something = 0;
                String str = ((BinaryOperationExpression)aSimple.Value).Operand1.ToString();
                //str = str.Substring(1,str.IndexOf(' ')-1);
                
                String str2 =((BinaryOperationExpression)aSimple.Value).Operand2.ToString();
               // str2 = str2.Substring(str2.IndexOf(' ')+1);
                //str2 = str.Substring(str2.IndexOf(' ')+1,str2.Length-2-str2.IndexOf(' '));
                Int32.TryParse(str2, out  something );
                if(!dSymbolTable.ContainsKey(str))
                 throw new SyntaxErrorException("This name not exists", new Identifier( str,0,0));
                lAssembly.AddRange(CopytoOpwithNumberOp2(dSymbolTable[str],something,((BinaryOperationExpression)aSimple.Value).Operator));
          }
           else if(aSimple.Value is BinaryOperationExpression && (((BinaryOperationExpression)aSimple.Value).Operand1 is Number) && (((BinaryOperationExpression)aSimple.Value).Operand1 is Number))
             {
                 int something = 0;
                int something2 = 0;
                String str = ((BinaryOperationExpression)aSimple.Value).Operand1.ToString();
                str = str.Substring(1,str.IndexOf(' ')-1);
                Int32.TryParse(str, out  something );
                String str2 =((BinaryOperationExpression)aSimple.Value).Operand1.ToString();
                str2 = str2.Substring(str2.IndexOf(' ')+1);
                str2 = str.Substring(str2.IndexOf(' ')+1,str2.Length-2-str2.IndexOf(' '));
                Int32.TryParse(str2, out  something2 );
                lAssembly.AddRange(CopytoOpwith2Numbers(something,something2,((BinaryOperationExpression)aSimple.Value).Operator));
            }
            else if (aSimple.Value is NumericExpression)
              {

                String Line1 = "@"+ aSimple.Value.ToString();
                String Line2 = "D=A";
                String Line3 = "@RESULT";
                String Line4 = "M=D";
                lAssembly.Add(Line1);
                lAssembly.Add(Line2);
                lAssembly.Add(Line3);
                lAssembly.Add(Line4);
              }
            else
            {
                if(!dSymbolTable.ContainsKey(aSimple.Variable.ToString()))
                  throw new SyntaxErrorException("This name not exists", new Identifier( aSimple.Value.ToString(),0,0));
                String line1 = "@LCL";
                String line2 = "D=M";
                String line3 = "@"+ dSymbolTable[aSimple.Value.ToString()];
                String line4 = "A=D+A";
                String Line5 = "D=M";
                String Line6 = "@RESULT";
                String Line7 = "M=D";
                lAssembly.Add(line1);
                lAssembly.Add(line2);
                lAssembly.Add(line3);
                lAssembly.Add(line4);
                lAssembly.Add(Line5);
                lAssembly.Add(Line6);
                lAssembly.Add(Line7);
            }
                String sline1 = "@LCL";
                String sline2 = "D=M";
                String sline3 = "@"+ dSymbolTable[aSimple.Variable];
                String sLine4 = "D=D+A";
                String sLine5 = "@ADDRESS";
                String sLine6 = "M=D";
                String sLine7 = "@RESULT";
                String sLine8 = "D=M";
                String sLine9 = "@ADDRESS";
                String sLine10 = "A=M";
                String sLine11 = "M=D";
                lAssembly.Add(sline1);
                lAssembly.Add(sline2);
                lAssembly.Add(sline3);
                lAssembly.Add(sLine4);
                lAssembly.Add(sLine5);
                lAssembly.Add(sLine6);
                lAssembly.Add(sLine7);
                lAssembly.Add(sLine8);
                lAssembly.Add(sLine9);
                lAssembly.Add(sLine10);
                lAssembly.Add(sLine11);
                
            //add here code for computing a single let statement containing only a simple expression
            return lAssembly;
        }


        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            List<string> artificialvars = new List<string>();
            int index=0;
            for (int i=0;i<lDeclerations.Count;i++)
            {
              if(lDeclerations[i].Name.Contains("_"))
                    artificialvars.Add(lDeclerations[i].Name);
              else
              {
            //     if (dTable.ContainsKey(lDeclerations[i].Name))
            //            throw new SyntaxErrorException("This name already exists", new Identifier( lDeclerations[i].Name,0,0));
                 dTable[lDeclerations[i].Name]=index;
                 index++;
              }
            }
            for(int j=0;j<artificialvars.Count;j++)
            {
                dTable[artificialvars[j]]=index;
                index++;
            }

            //add here code to comptue a symbol table for the given var declarations
            //real vars should come before (lower indexes) than artificial vars (starting with _), and their indexes must be by order of appearance.
            //for example, given the declarations:
            //var int x;
            //var int _1;
            //var int y;
            //the resulting table should be x=0,y=1,_1=2
            //throw an exception if a var with the same name is defined more than once
            return dTable;
        }


        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }

        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
             List<LetStatement> let = new List<LetStatement>();
            if(s.Value.ToString().Length==1)
            {
                let.Add(s);
                VarDeclaration v = new VarDeclaration("int",s.Variable);
                lVars.Add(v);
            }

             List<Token> lTokens = new List<Token>();
             List<Token> lTokens2 = new List<Token>();
            int indexvar =1;
            int numLine=0;
            int count=0;
            int place=0;
            String str =s.Value.ToString();
            String str2="";
            String str1 = "let _"+indexvar+"=";
            str=str.Replace(" ","");
            lTokens = Tokenize(s.Value.ToString(),numLine);
            place=lTokens.Count;
            int i=0;
            while (i<str.Length-1)
            {
                if (lTokens.Count==5)
                    str1="let "+s.Variable+"=";
              
               if ((lTokens[i] is Parentheses) && (((Parentheses)lTokens[i]).Name=='(') && (lTokens[i+4] is Parentheses) && (((Parentheses)lTokens[i+4]).Name==')'))
               {
               for (int k =i ;k<i+5;k++)
               {
                  str1 = str1 +lTokens[k].ToString();
                  str2 = str2 +lTokens[k].ToString();

               }
                str1=str1+";";   
                lTokens2 = Tokenize(str1,numLine);
                if (lTokens.Count!=5)
                {
                VarDeclaration v = new VarDeclaration("int","_"+indexvar);
                lVars.Add(v);
                }
                LetStatement stat=ParseStatement(lTokens2);
                let.Add(stat);
                if (lTokens.Count==5)
                        break;
            //    int w=i;
            //    if(place==lTokens.Count);
             //   for (;w<str.Length;w++)
             //           if (str[w]=='(' && str[w+1]!='(' )
             //               break;
               // str = str.Substring(0,w)+"_"+indexvar+str.Substring(str.IndexOf(")")+1,str.Length-1-str.IndexOf(")"));
                str=str.Replace(str2,"_"+indexvar);
                lTokens = Tokenize(str,numLine);
                indexvar++;
                numLine++;
                str1 = "let _"+indexvar+"=";
                str2="";
                i=0;
                continue;
               }
               i++;
              }
            return let;


               /*while(str.Length!=5)
                {
                    str1 = str1.Substring(str.IndexOf("("),str.Length-1-str.IndexOf("("))
                    String left = str1.Substring(0,str.Length-1-str.IndexOf("("))
                   // String right = str1.Substring(0,str.Length-1-str.IndexOf(")"))

                }
                lTokens2 = Tokenize(str1,numLine);
                VarDeclaration v = new VarDeclaration("int","_"+indexvar);
                lVars.Add(v);
                LetStatement stat=ParseStatement(lTokens2);
                let.Add(stat);
                indexvar++;
                numLine++;
                


                while(str1[0]=="(")
                {
                  count++
                  str1 = str1.Substring(1,str1.IndexOf(")"))

                }
                String str3 = str1.Substring(str.IndexOf("("),str.Length-1-str.IndexOf("("));
            }
            
            
            
            
            for (int j=0;j<lTokens.Count;j++)
            {
                if ((lTokens[j] is Parentheses) && (((Parentheses)lTokens[j]).Name=='('))
                    count++;
                if ((lTokens[j] is Parentheses) && (((Parentheses)lTokens[j]).Name==')'))
                {
                    count--;
                    String line = "let _"+indexvar+"=(";
                    place = j;
                    while (!(lTokens[place] is Parentheses) && ((Parentheses)lTokens[place]).Name!='(')
                    {
                        line = line + lTokens[place].ToString();
                        place--;
                    }
                    line = line +")";
                    lTokens2 = Tokenize(line,numLine);
                    VarDeclaration v = new VarDeclaration("int","_"+indexvar);
                    lVars.Add(v);
                    LetStatement stat=ParseStatement(lTokens2);
                    let.Add(stat);
                    indexvar++;
                    numLine++;
                   
                }
            }
            return let;*/
        }
            
            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.
            
        
        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }

 
        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }
        private List<string> CopytoOp(int index1,int index2,string op)
        {  
            List<string> lExpanded = new List<string>();
            String Line1 = "@LCL";
            String Line2 = "D=M";
            String Line3 = "@" + index1;
            String Line4 = "A=D+A";
            String Line5 = "D=M";
            String Line6 = "@OP1";
            String Line7 = "M=D";
            String Line8 = "@LCL";
            String Line9 = "D=M";
            String Line10 = "@" + index2;
            String Line11 = "A=D+A";
            String Line12 = "D=M";
            String Line13 = "@OP2";
            String Line14 = "M=D";
            String Line15 = "D=M";
            String Line16 = "@OP1";
            String Line17 = "D=M"+op+"D";
            String Line18 = "@RESULT";
            String Line19 = "M=D";
            lExpanded.Add(Line1);
            lExpanded.Add(Line2);
            lExpanded.Add(Line3);
            lExpanded.Add(Line4);
            lExpanded.Add(Line5);
            lExpanded.Add(Line6);
            lExpanded.Add(Line7);
            lExpanded.Add(Line8);
            lExpanded.Add(Line9);
            lExpanded.Add(Line10);
            lExpanded.Add(Line11);
            lExpanded.Add(Line12);
            lExpanded.Add(Line13);
            lExpanded.Add(Line14);
                lExpanded.Add(Line15);
            lExpanded.Add(Line16);
            lExpanded.Add(Line17);
            lExpanded.Add(Line18);
            lExpanded.Add(Line19);

            return lExpanded;   
        }

        private List<string> CopytoOpwithNumberOp2(int index1,int number,string op)
        {  
            List<string> lExpanded = new List<string>();
            
            String Line1 = "@LCL";
            String Line2 = "D=M";
            String Line3 = "@" + index1;
            String Line4 = "A=D+A";
            String Line5 = "D=M";
            String Line6 = "@OP1";
            String Line7 = "M=D";
            String Line8 = "@"+number;
            String Line9 = "D=A";
            String Line10 = "@OP2";
            String Line11 = "M=D";
             String Line12 = "D=M";
            String Line13 = "@OP1";
            String Line14 = "D=M"+op+"D";
            String Line15 = "@RESULT";
            String Line16 = "M=D";
            lExpanded.Add(Line1);
            lExpanded.Add(Line2);
            lExpanded.Add(Line3);
            lExpanded.Add(Line4);
            lExpanded.Add(Line5);
            lExpanded.Add(Line6);
            lExpanded.Add(Line7);
            lExpanded.Add(Line8);
            lExpanded.Add(Line9);
            lExpanded.Add(Line10);
            lExpanded.Add(Line11);
                lExpanded.Add(Line12);
            lExpanded.Add(Line13);
            lExpanded.Add(Line14);
            lExpanded.Add(Line15);
            lExpanded.Add(Line16);
          
            return lExpanded;   
        }

        private List<string> CopytoOpwithNumberOp1(int index1,int number,string op)
        {  
            List<string> lExpanded = new List<string>();
           
            String Line1 = "@LCL";
            String Line2 = "D=M";
            String Line3 = "@" + index1;
            String Line4 = "A=D+A";
            String Line5 = "D=M";
            String Line6 = "@OP2";
            String Line7 = "M=D";
            String Line8 = "@"+number;
            String Line9 = "D=A";
            String Line10 = "@OP1";
            String Line11 = "M=D";
            String Line12 = "D=M";
            String Line13 = "@OP2";
            String Line14 = "D=D"+op+"M";
            String Line15 = "@RESULT";
            String Line16 = "M=D";
            lExpanded.Add(Line1);
            lExpanded.Add(Line2);
            lExpanded.Add(Line3);
            lExpanded.Add(Line4);
            lExpanded.Add(Line5);
            lExpanded.Add(Line6);
            lExpanded.Add(Line7);
            lExpanded.Add(Line8);
            lExpanded.Add(Line9);
            lExpanded.Add(Line10);
            lExpanded.Add(Line11);
                lExpanded.Add(Line12);
            lExpanded.Add(Line13);
            lExpanded.Add(Line14);
            lExpanded.Add(Line15);
            lExpanded.Add(Line16);
         
            return lExpanded;   
        }
             private List<string> CopytoOpwith2Numbers(int number1,int number2,string op)
        {  
            List<string> lExpanded = new List<string>();
           
            String Line1 = "@"+number1;
            String Line2 = "D=A";
            String Line3 = "@OP1";
            String Line4 = "M=D";
            String Line5 = "@"+number2;
            String Line6 = "D=A";
            String Line7 = "@OP2";
            String Line8 = "M=D";
                 String Line9 = "D=M";
            String Line10 = "@OP1";
            String Line11 = "D=D"+op+"M";
            String Line12 = "@RESULT";
            String Line13 = "M=D";
            lExpanded.Add(Line1);
            lExpanded.Add(Line2);
            lExpanded.Add(Line3);
            lExpanded.Add(Line4);
            lExpanded.Add(Line5);
            lExpanded.Add(Line6);
            lExpanded.Add(Line7);
            lExpanded.Add(Line8);
                lExpanded.Add(Line9);
            lExpanded.Add(Line10);
            lExpanded.Add(Line11);
            lExpanded.Add(Line12);
            lExpanded.Add(Line13);
      

            return lExpanded;   
        }
    

        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (aDelimiters.Contains(s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }
 
        public List<Token> Tokenize(string sLine, int iLine)
         {
           // Token token ;
            List<string> Tokens = new List<string>();
            List<Token> lTokens = new List<Token>();
            char [] Delimiters = new char[] { ';' , ',' , '\t','(', ')', '[', ']', '{', '}' ,' ','*', '+', '-', '/', '<', '>', '&', '=', '|', '!' };
                int indexInLine=0;
                int i = iLine;
                int number=0;
                String line = sLine;
               Tokens =  Split(line,Delimiters);
                for (int j=0;j < Tokens.Count;j++)
                {
                    String firstCahr = Tokens[j].Substring(0,1);
                      if (Tokens[j].CompareTo("/")==0 && Tokens[j+1].CompareTo("/")==0)
                        break;
                    if (Tokens[j].CompareTo(" ")==0)
                        {
                        indexInLine++;
                        continue;
                        }

                    if (Tokens[j].CompareTo("\t")==0)
                        {
                        indexInLine++;
                        continue;
                        
                        }
                    if(Tokens[j].CompareTo("function")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine=indexInLine+8;
                        continue;
                    }
                     if(Tokens[j].CompareTo("var")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine=indexInLine+3;
                        continue;
                    }
                 if(Tokens[j].CompareTo("if")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+2;
                        continue;
                    }
                 if(Tokens[j].CompareTo("let")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+3;
                        continue;
                    }
                 if(Tokens[j].CompareTo("return")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+6;
                        continue;
                    }
                 if(Tokens[j].CompareTo("while")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+5;
                        continue;
                    }
                 if(Tokens[j].CompareTo("else")==0)
                    {
                        Statement stat = new Statement(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+4;
                        continue;
                    }
                 if(Tokens[j].CompareTo("int")==0)
                    {
                        VarType stat = new VarType(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+3;
                        continue;
                    }
                  if(Tokens[j].CompareTo("bool")==0)
                    {
                        VarType stat = new VarType(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+4;
                        continue;
                    }
                 if(Tokens[j].CompareTo("array")==0)
                    {
                        VarType stat = new VarType(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+5;
                        continue;
                    }
                 if(Tokens[j].CompareTo("char")==0)
                    {
                        VarType stat = new VarType(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+4;
                        continue;
                    }
                 if(Tokens[j].CompareTo("false")==0)
                    {
                        Constant stat =new Constant (Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+5;
                        continue;
                    }
                 if(Tokens[j].CompareTo("true")==0)
                    {
                        Constant stat =new Constant (Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+4;
                        continue;
                    }
                 if(Tokens[j].CompareTo("null")==0)
                    {
                        Constant stat =new Constant (Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                         indexInLine=indexInLine+4;
                        continue;
                    }
                 if((Int32.TryParse(firstCahr, out  number ) ))
                    {
                        if ((Int32.TryParse(Tokens[j], out  number )))
                        {
                        Number stat = new Number(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine=indexInLine+Tokens[j].Length;
                        continue;
                        }
                        else
                        {
                            Identifier ID = new Identifier(Tokens[j],i,indexInLine);
                           throw new SyntaxErrorException("ID can't start with number",ID);
                 
                           
                         }
                    }
                 
                 if(Tokens[j].CompareTo("(")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].CompareTo(")")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].CompareTo("{")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].CompareTo("}")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].CompareTo("[")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].CompareTo("]")==0)
                    {
                        Parentheses stat = new Parentheses(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("="))
                    {
                        int index = Tokens[j].IndexOf("=");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("<"))
                    {
                        int index = Tokens[j].IndexOf("<");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains(">"))
                    {
                        int index = Tokens[j].IndexOf(">");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                   if(Tokens[j].Contains("+"))
                    {
                        int index = Tokens[j].IndexOf("+");
                        Operator stat = new Operator(Tokens[j][0],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("-"))
                    {
                        int index = Tokens[j].IndexOf("-");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("*"))
                    {
                        int index = Tokens[j].IndexOf("*");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("/"))
                    {
                        int index = Tokens[j].IndexOf("/");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("%"))
                    {
                        int index = Tokens[j].IndexOf("%");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                 if(Tokens[j].Contains("!"))
                    {
                        int index = Tokens[j].IndexOf("!");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                  if(Tokens[j].Contains("&"))
                    {
                        int index = Tokens[j].IndexOf("&");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                   if(Tokens[j].Contains("|"))
                    {
                        int index = Tokens[j].IndexOf("|");
                        Operator stat = new Operator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                   if(Tokens[j].Contains(","))
                    {
                        int index = Tokens[j].IndexOf(",");
                        Separator stat = new Separator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                   if(Tokens[j].Contains(";"))
                    {
                        int index = Tokens[j].IndexOf(";");
                        Separator stat = new Separator(Tokens[j][index],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine++;
                        continue;
                    }
                    if(!(Int32.TryParse(firstCahr, out  number )))
                    {
                        char firstCahr1 = firstCahr[0];
                        if (( firstCahr1 < 'A' || firstCahr1 > 'Z') && ( firstCahr1 < 'a' || firstCahr1 > 'z' ) && firstCahr1 != '_')
                            {
                            Identifier ID = new Identifier(Tokens[j],i,indexInLine);
                           throw new SyntaxErrorException("ID start with ilegal char",ID);
                            }
                        else
                        {
                        Identifier stat = new Identifier(Tokens[j],i,indexInLine);
                        lTokens.Add(stat);
                        indexInLine=indexInLine+Tokens[j].Length;
                        continue;
                         }

                    }
                 }
            
            return lTokens;
        

        }
        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }
            return lTokens;
        }

    }
}
