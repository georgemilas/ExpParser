using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpParser.keywords
{

    /// <summary>
    ///   - space(s) is an OR, & is an AND, ! is a NOT
    ///   - "k1 k2" same as "k1 or k2" 
    ///   - "k1 k2 & !k3" same as "k1 or (k2 and not k3)"
    ///   - 'k1 "some phrase"' same as 'k1 or "some phrase"' 
    ///   - NOTE quotes " inside "some phrase" must be escaped with \ so "some\" phrase" and 
    ///     if \" is needed then "some \\"phrase" because (\") will become (") after parsing so no need to escape escaping
    ///                          "some \\\"phrase"  -> some \\"phrase
    ///                          "some \\\\"phrase"  -> some \\\"phrase
    ///   - k1 (k2 & {k3}) == k1 or (k2 and {k3}) where k3 may be a regular Expression
    ///   - precedence table in descending order: {exp} "exp" ! and or
    /// </summary>
    public class KeywordsExpressionParser: BaseParser
    {
        public new IKeywordsSemantic Semantic
        {
            get
            {
                return (IKeywordsSemantic)base.Semantic;
            }
        }
        
        public KeywordsExpressionParser(string keywordsExpressionString): this(keywordsExpressionString, new TextSearch.TextSearchSemantic()) { }
        public KeywordsExpressionParser(string keywordsExpressionString, IKeywordsSemantic semantic) : base(keywordsExpressionString, semantic) { }

        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////
        private string CurlyBracesTokenHandler(string token, string tokenHash)
        {
            //just store the token as is (curlyBraces inside are part of the regular Expression)
            //when matching, it will check for token[0]=={ & token[-1]==} and if yes it will use 
            //regexp matching if available or literal matching of the whole thing including {} otherwise
            this.tokensContainer.AddToken("token#" + tokenHash, token);
            return "token#" + tokenHash;
        }

        public override string PrepareParsing(string keys)
        {
            //1. handle Regexp first
            //expressions inside {} are regular expressions, tokenize them
            keys = this.Parentheses(keys, this.CurlyBracesTokenHandler, '{', '}');

            //2. handle "exact match strings" second
            //reduce expressions between quotes to some unique token that we can more easily work with
            keys = QuotedStrings(keys);
            
            //3. align tokensContainer and expressions one next to the other 
            //since whitespace has a meaning in this language
            Regex r = new Regex(@"\s+");
            keys = r.Replace(keys, " ");    //reduce multiple spaces to 1 space

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
