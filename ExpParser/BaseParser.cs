using System;
using System.Collections.Generic;
using ExtParser.Extensions;
using ExpParser.Exceptions;

namespace ExpParser
{
    public abstract class BaseParser : IEvaluableExpression
    {
        public string StringToBeParsed { get; set; }

        protected TokensContainer tokensContainer;  //state helper
        protected List<Token> _tokens;              //list of all tokens that are not Expression trees themselves
        protected delegate string TokenHandler(string token, string tokenHash);
        private IEvaluableExpression _expression;
        
        public BaseParser(string expressionToBeParsed, ISemantic semantic)
        {
            this.StringToBeParsed = expressionToBeParsed;
            this._expression = null;
            this.Semantic = semantic;
        }

        /// <summary>
        /// TokenEvaluator if the current Keywords Expression matches the given text
        /// </summary>
        public virtual object Evaluate(object obj)
        {
            return this.Expression.Evaluate(obj);
        }

        /// <summary>
        /// list of all tokens that are not Expression trees themselves
        /// </summary>
        protected List<Token> tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }

        /// <summary>
        /// Provides abstraction over how tokens are evaluated
        /// </summary>
        public ISemantic Semantic { get; set; }

        /// <summary>
        /// an evaluable Expression tree 
        /// </summary>
        public IEvaluableExpression Expression
        {
            get
            {
                if (this._expression == null)    //build an evaluable Expression tree
                {
                    this.tokensContainer = new TokensContainer();
                    this.tokens = new List<Token>();
                    string strToParse = this.StringToBeParsed;

                    strToParse = this.PrepareParsing(strToParse);
                    IEvaluableExpression exp = Parse(strToParse);
                    this._expression = this.FinalizeParsing(exp);
                }
                return _expression;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////
        /// ENGINE
        ////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// prepare state machine and expressionToBeParsed into a clean Parse-able expressionToBeParsed
        /// </summary>
        /// <param name="expressionToBeParsed"></param>
        /// <returns></returns>
        public abstract string PrepareParsing(string expressionToBeParsed);
        /// <summary>
        /// method that builds an evaluable Expression tree from 
        /// </summary>
        public abstract IEvaluableExpression Parse(string expressionToBeParsed);
        public virtual IEvaluableExpression FinalizeParsing(IEvaluableExpression exp)
        {
            if (this.Semantic != null && this.tokens != null)
            {
                foreach (Token k in this.tokens)
                {
                    k.TokenEvaluator = this.Semantic.TokenEvaluator;
                }
            }
            return exp;
        }




        /// <summary>
        /// Tokenize expressions between quotes 
        /// </summary>
        /// <returns></returns>
        protected string QuotedStrings(string expressionToBeParsed)
        {
            return QuotedStrings(expressionToBeParsed, '"', '\\');
        }
        protected string QuotedStrings(string expressionToBeParsed, char quote, char quoteEscape)
        {
            string keys = expressionToBeParsed;

            //reduce expressions between quotes to some unique token that we can more easily work with
            int idx = keys.IndexOf(quote);
            while (idx >= 0)
            {
                int seekFrom = idx + 1;
                int idx2 = keys.Slice(seekFrom).IndexOf(quote);

                while (idx2 >= 0 && keys[seekFrom + idx2 - 1] == quoteEscape && !(idx2 == keys.Length - 1))
                {
                    //escape for " is \" unless is the last char
                    seekFrom = seekFrom + idx2 + 1 + 1;
                    idx2 = keys.Slice(seekFrom).IndexOf(quote);
                }
                if (idx2 >= 0)
                {
                    string token = keys.Slice(idx, seekFrom + idx2 + 1);
                    string hs = tokensContainer.GetTokenHash(token);
                    tokensContainer.AddToken("token#" + hs, token.Slice(1, -1).Replace(quoteEscape.ToString()+quote.ToString(), quote.ToString()));    //strip out enclosing quote symbol and un-escape quotes
                    keys = keys.Replace(token, "token#" + hs);
                    idx = keys.IndexOf(quote);
                }
                else
                {
                    break;
                }
            }
            
            return keys;
        }


        /// <summary>
        /// tokenize a statement and set in the TokensContainer 
        /// as  container[token#statement_hash] = IEvaluableExpression
        /// </summary>
        protected virtual string ParanthesesTokenHandler(string token, string tokenHash)
        {
            //strip out '(' and ')' and recursively Parse sub Expression (sub parentheses)
            IEvaluableExpression tval = Parse(token.Slice(1, -1));
            tokensContainer.AddEvaluableExpression("token#" + tokenHash, tval);
            return "token#" + tokenHash.ToString();
        }

        /// <summary>
        /// tokenize a statement and set in the TokensContainer 
        /// as container[token#statement_hash] = IEvaluableExpression Parentheses
        /// </summary>
        protected virtual string Parentheses(string kexp) 
        {
            return Parentheses(kexp, ParanthesesTokenHandler);
        }
        protected virtual string Parentheses(string kexp, TokenHandler tokenHandler)
        {
            return Parentheses(kexp, tokenHandler, '(', ')');
        }
        protected virtual string Parentheses(string kexp, TokenHandler tokenHandler, char open, char close)
        {
            int openCnt = 0;
            List<int> ixopen = new List<int>();
            //first reduce expressions in between parentheses to an Expression instance:
            //uses something like a state machine (with a stack)
            for (int ix = 0; ix < kexp.Length; ix++)
            {
                if (kexp[ix] == open)
                {
                    openCnt += 1;  //add to stack
                    ixopen.Add(ix); //remember 
                }
                else if (kexp[ix] == close)
                {
                    openCnt -= 1;      //pop stack
                    if (openCnt == 0)  //we have the Expression
                    {
                        string token = kexp.Slice(ixopen[0], ix + 1);
                        string hs = tokensContainer.GetTokenHash(token);
                        //parentheses inside token must be handled by this (may call parentheses 
                        //against token or whatever) and store the token value in the tokensContainer dictionary
                        string th = tokenHandler(token, hs);
                        kexp = kexp.Replace(token, th);
                        //restart to reduce more parentheses expressions (whether has more parentheses or no)
                        return Parentheses(kexp, tokenHandler, open, close);
                    }
                    else
                    {
                        try { ixopen.Pop(); }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ParsingException("parsing error - closing parentheses not matching open ones");
                        }
                    }
                }
            }
            if (openCnt != 0)
            {
                throw new ParsingException("parsing error - closing parentheses not matching open ones");
            }
            return kexp;
        }

        /// <summary>
        /// instantiate and return Token's, 
        ///  -this token does not yet have an Evaluator function, one must be set at some point later 
        ///   maybe in finalizeParser method or something
        /// </summary>
        protected virtual IEvaluableExpression GetToken(string r)
        {
            Token kw = null;

            if (tokensContainer.ContainsKey(r))
            {
                if (tokensContainer.IsValueAnEvaluableExpression(r))
                {
                    return tokensContainer.GetExpression(r);
                }
                kw = new Token(tokensContainer.GetToken(r));
                this.tokens.Add(kw);
                return kw;
            }
            kw = new Token(r);
            this.tokens.Add(kw);
            return kw;
        }


    }

}