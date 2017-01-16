using ExpParser.Exceptions;
using System;
using System.Collections.Generic;

namespace ExpParser
{
    /// <summary>
    /// Evaluate a 'token' by setting a 'TokenEvaluator' function so that TokenEvaluator(token) 
    /// represent the evaluation of Token(k)"
    /// </summary>

    public class Token: IEvaluableExpression
    {
        public string token;
        public delegate object TokenEvaluatorFunction(object obj, string keyword);
        public virtual TokenEvaluatorFunction TokenEvaluator { get; set; }

        public Token(string token) 
        {
            this.token = token.Trim();
        }
        public Token(string token, TokenEvaluatorFunction tokenEvaluator) : this(token)
        {
            this.TokenEvaluator = tokenEvaluator;
        }
        public virtual object Evaluate(object obj) 
        {
            if (this.TokenEvaluator == null)
            {
                throw new EvaluationException("No evaluation function exists for '" + token + "'");
            }
            return this.TokenEvaluator(obj, this.token);            
        }
        public override string ToString()
        {
            return this.token;
        }
    }
}
