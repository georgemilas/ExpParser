using ExpParser.keywords;
using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExpParser.arithmetic
{
    public class ArithmeticExpressionParser : BaseParser
    {
        public new IArithmeticSemantic Semantic
        {
            get
            {
                return (IArithmeticSemantic)base.Semantic;
            }
        }

        public ArithmeticExpressionParser(string arithmeticExpression, ArithmenticSemantic semantic): base(arithmeticExpression, semantic) { }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////
        public override string PrepareParsing(string keys)
        {
            Regex r = new Regex(@"\s+");
            keys = r.Replace(keys, " ");    //reduce multiple spaces to 1 space
            return keys;
        }

        public string ParseReplace(string kexp, string what, string replacement)
        {
            Regex rex = new Regex(what, RegexOptions.IgnoreCase);
            return rex.Replace(kexp, replacement);
        }

        public override IEvaluableExpression Parse(string kexp)
        {
            kexp = kexp.Trim();
            kexp = Parentheses(kexp);
            //all expressions between parentheses are tokenized and so are Regex and literals
            //so reduce "and, or, not" with the rest of tokens
            
            kexp = ParseReplace(kexp, @" *\+ *", "|+|");
            kexp = ParseReplace(kexp, " *- *", "|-|");    

            kexp = ParseReplace(kexp, " */ *", "|/|");
            kexp = ParseReplace(kexp, @" *\* *", "|*|");

            kexp = ParseReplace(kexp, @" *\^ *", "|^|");

            //+- has lower precedence than */ which has lower precedence than ^  
            //so split +-, split */, split ^ then evaluate ^, evaluate */, evaluate +-
            var exp = GetPlusExpression(kexp);
            return exp;
        }

        protected IEvaluableExpression GetPlusExpression(string kexp)
        {
            return GetOperatorExpression(kexp, "|+|", this.Semantic.PLUS, GetMinusExpression);
        }
        protected IEvaluableExpression GetMinusExpression(string kexp)
        {
            return GetOperatorExpression(kexp, "|-|", this.Semantic.MINUS, GetMultiplyExpression);            
        }
        protected IEvaluableExpression GetMultiplyExpression(string kexp)
        {
            return GetOperatorExpression(kexp, "|*|", this.Semantic.MUL, GetDivideExpression);
        }
        protected IEvaluableExpression GetDivideExpression(string kexp)
        {
            return GetOperatorExpression(kexp, "|/|", this.Semantic.DIV, GetPowExpression);
        }
        protected IEvaluableExpression GetPowExpression(string kexp)
        {
            return GetOperatorExpression(kexp, "|^|", this.Semantic.POW, GetToken);  //GetTokenExpression
        }


        protected IEvaluableExpression GetOperatorExpression(string kexp, string tokenStr, IOperator op, Func<string, IEvaluableExpression> next)
        {
            List<string> opList = kexp.SplitClean(tokenStr);  // |-|, |+|, |*| ... 
            List<IEvaluableExpression> opExpressions = new List<IEvaluableExpression>();
            IEvaluableExpression exp = null;
            if (opList.Count > 1)
            {
                foreach (string k in opList)
                {
                    opExpressions.Add(next(k));
                }
                exp = new ExpressionTree(op, opExpressions);
            }
            else
            {
                exp = next(opList[0]);
            }
            return exp;
        }


    }

}