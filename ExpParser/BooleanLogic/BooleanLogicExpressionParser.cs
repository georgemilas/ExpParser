using ExpParser.Exceptions;
using ExtParser.Extensions;
using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic
{

    /// <summary>
    ///   Provide a keywords expression parser with boolean logic so the following rules apply:
    /// 
    ///   - space(s) is an OR, & is an AND, ! is a NOT
    ///   - "k1 k2" same as "k1 or k2" 
    ///   - "k1 k2 & !k3" same as "k1 or (k2 and not k3)"
    ///   - precedence table in descending order: not and or
    /// 
    /// <param name="keywordsExpressionString">Evaluable expression string containing keywords and logical operators</param>
    /// <param name="semantic">
    ///    IBooleanLogicSemantic to evaluate against custom contexts 
    ///      e.g. - SQL generation SQLSemantic, 
    ///           - real time evaluation against text TextSearchSemantic
    ///           - etc
    /// </param>
    /// 
    /// </summary>
    public class BooleanLogicExpressionParser: ExpBaseParser
    {
        public new IBooleanLogicSemantic Semantic
        {
            get
            {
                return (IBooleanLogicSemantic)base.Semantic;
            }
        }
        
        
        public BooleanLogicExpressionParser(string keywordsExpressionString, IBooleanLogicSemantic semantic) : base(keywordsExpressionString, semantic) { }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////
        
        public override string PrepareParsing(string keys)
        {
            return keys;
        }
        
        public override IEvaluableExpression Parse(string kexp)
        {
            kexp = kexp.Trim();
            kexp = Parentheses(kexp, ParenthesesTokenHandler);
            //all expressions between parentheses are tokenized and so are Regex and literals
            //so reduce "and, or, not" with the rest of tokens
            kexp = kexp.Replace(" not ", " |not|");
            kexp = kexp.Replace(" ! ", " |not|");
            kexp = kexp.Replace(" !", " |not|");
            kexp = kexp.Replace(" and ", "|and|");
            kexp = kexp.Replace(" & ", "|and|");
            kexp = kexp.ToLower().Replace(" or ", "|or|");
            kexp = kexp.Replace(" ", "|or|");

            //OR has lower precedence then AND, 
            //so split OR's, split AND's then do AND's and then do OR's
            IEvaluableExpression exp = GetOrExpression(kexp);
            return exp;
        }

        /// <summary>
        /// Handle OR operator
        /// </summary>
        /// <param name="kexp"></param>
        /// <returns></returns>
        protected virtual IEvaluableExpression GetOrExpression(string kexp)
        {
            if (String.IsNullOrEmpty(kexp))
            {
                throw new ParsingException("Expression to parse must not be null or empty");
            }
            List<string> orList = kexp.SplitClean("|or|");
            List<IEvaluableExpression> ors = new List<IEvaluableExpression>();
            IEvaluableExpression exp = null;
            if (orList.Count > 1)
            {
                foreach (string k in orList)
                {
                    //will reduce "not" before reducing "and parts":
                    ors.Add(GetAndExpression(k));
                }
                exp = new ExpressionTree(this.Semantic.OR, ors);
            }
            else
            {
                exp = GetAndExpression(orList[0]);
            }
            return exp;
        }

        /// <summary>
        /// Handle AND and NOT operators 
        /// </summary>
        protected virtual IEvaluableExpression GetAndExpression(string k)
        {
            return GetAndExpression(k, GetToken);   
        }

        /// <summary>
        /// Handle AND and NOT operators and continue (next function) with handling the operator attribute 
        /// By default operator attribute is a Token (aka next=GetToken) but pass a different handler to extend the parser 
        /// </summary>
        protected virtual IEvaluableExpression GetAndExpression(string k, Func<string, IEvaluableExpression> next)
        {
            List<string> andList= k.SplitClean("|and|"); 
            if ( andList.Count > 1 )
            {
                List<IEvaluableExpression> ands = new List<IEvaluableExpression>();
                foreach (string a in andList)
                {
                    if (a.Contains("|not|"))
                    {
                        string ka = a.Replace("|not|", "");
                        ands.Add(new ExpressionTree(this.Semantic.NOT, next(ka)));
                    }
                    else
                    {
                        ands.Add(next(a));
                    }
                }
                return new ExpressionTree(this.Semantic.AND, ands);
            }
            else 
            {

                if (tokensContainer.ContainsKey(k))
                {
                    if (tokensContainer.IsValueAnEvaluableExpression(k))
                    {
                        return tokensContainer.GetExpression(k);
                    }
                    else
                    {
                        string stoken = tokensContainer.GetToken(k);
                        if (stoken.Contains("|not|"))
                        {
                            string ka = stoken.Replace("|not|", "");
                            return new ExpressionTree(this.Semantic.NOT, next(ka));
                        }
                        else
                        {
                            return next(stoken);
                        }
                    }
                }
                else
                {
                    if (k.Contains("|not|"))
                    {
                        string ka = k.Replace("|not|", "");
                        return new ExpressionTree(this.Semantic.NOT, next(ka));
                    }
                    else
                    {
                        return next(k);
                    }
                    
                }             
                
            }
        }        

        
    }


    

}
