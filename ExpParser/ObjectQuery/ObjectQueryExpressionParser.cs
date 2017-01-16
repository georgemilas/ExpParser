using ExpParser.keywords;
using ExtParser.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExpParser.ObjectQuery
{
    public class ObjectQueryExpressionParser : KeywordsExpressionParser
    {
        public ObjectQueryExpressionParser(string apiQueryExpression, ObjectQuerySemantic semantic): base(apiQueryExpression, semantic) { }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////

        public string ParseReplace(string kexp, string what, string replacement)
        {
            Regex rex = new Regex(what, RegexOptions.IgnoreCase);
            return rex.Replace(kexp, replacement);
        }

        public override IEvaluableExpression Parse(string kexp)
        {
            kexp = kexp.Trim();
            kexp = Parentheses(kexp, ParenthesesTokenHandler);
            //all expressions between parentheses are tokenized and so are Regex and literals
            //so reduce "and, or, not" with the rest of tokens
            
            kexp = ParseReplace(kexp, " le ", "|le|");
            kexp = ParseReplace(kexp, " *<= *", "|le|");    //must handle '<=' before any of '=' or '<'  

            kexp = ParseReplace(kexp, " ge ", "|ge|");
            kexp = ParseReplace(kexp, " *>= *", "|ge|");

            kexp = ParseReplace(kexp, " ne ", "|ne|");
            kexp = ParseReplace(kexp, " *!= *", "|ne|");

            kexp = ParseReplace(kexp, " eq ", "|eq|");
            kexp = ParseReplace(kexp, " *= *", "|eq|");
            
            kexp = ParseReplace(kexp, " lt ", "|lt|");
            kexp = ParseReplace(kexp, " *< *", "|lt|");
            
            kexp = ParseReplace(kexp, " gt ", "|gt|");
            kexp = ParseReplace(kexp, " *> *", "|gt|");                        

            kexp = ParseReplace(kexp, " not ", " |not|");
            kexp = ParseReplace(kexp, " and ", "|and|");
            kexp = ParseReplace(kexp, " or ", "|or|");
            //kexp = kexp.Replace(" ", "|or|");   // we are not supporting space " " as an OR for the API language

            //OR has lower precedence then AND, 
            //so split OR's, split AND's handle NOT and comparer operators then do AND's and then do OR's
            //NOT takes precedence over comparer operators so "not a = b" is the same as "(not a) = b"
            IEvaluableExpression exp = GetOrExpression(kexp);
            return exp;
        }
        protected override IEvaluableExpression GetAndExpression(string k)
        {
            return GetAndExpression(k, GetComparerOperatorExpression);
        }
        
        /// <summary>
        /// opToken -> |eq|, |ne|, etc.
        /// </summary>
        protected IEvaluableExpression GetComparerOperatorExpression(string r)
        {
            ObjectQuerySemantic s = (ObjectQuerySemantic)this.Semantic;
            var cmp = GetComparerOperatorExpression(r, "|eq|", s.EQ);
            if (cmp == null) cmp = GetComparerOperatorExpression(r, "|ne|", s.NE);
            if (cmp == null) cmp = GetComparerOperatorExpression(r, "|lt|", s.LT);
            if (cmp == null) cmp = GetComparerOperatorExpression(r, "|gt|", s.GT);
            if (cmp == null) cmp = GetComparerOperatorExpression(r, "|le|", s.LE);
            if (cmp == null) cmp = GetComparerOperatorExpression(r, "|ge|", s.GE);
            if (cmp == null)
            {
                return GetToken(r);    
            }
            return cmp;
        }

        /// <summary>
        /// opToken -> |eq|, |ne|, etc.
        /// </summary>
        protected IEvaluableExpression GetComparerOperatorExpression(string r, string opToken, IOperator op)
        {
            //We have Support for Property to Literal comparison 
            //There is no support for Property to Property comparison 
            //There is no support for Literal to Literal comparison 

            List<string> opList = r.SplitClean(opToken);
            if (opList.Count > 1)
            {
                var propToken = new PropertyToken(opList[0]);          //left side -> property must be on the left  
                this.tokens.Add(propToken);

                string literal = tokensContainer.ContainsKey(opList[1]) ? tokensContainer.GetToken(opList[1]) : opList[1];
                var litToken = new LiteralToken(propToken, literal);  //right side -> literal must be on the right                             
                this.tokens.Add(litToken);

                return new ComparerOperatorExpression(op, propToken, litToken);                                                
            }
            return null;
        }

    }
}