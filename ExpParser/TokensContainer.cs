using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser
{
    /// <summary>
    /// a helper class for the parser
    /// </summary>
    public class TokensContainer
    {
        Dictionary<string, string> tokens;
        Dictionary<string, IEvaluableExpression> expressions;   //keys may be of type token#statement_hash    (ex: token#-352765342)

        public TokensContainer()
        {
            tokens = new Dictionary<string, string>();
            expressions = new Dictionary<string, IEvaluableExpression>();
        }

        public bool ContainsKey(string k)
        {
            return (tokens.ContainsKey(k) || expressions.ContainsKey(k));
        }

        public bool IsValueAnEvaluableExpression(string key)
        {
            return expressions.ContainsKey(key);
        }

        public IEvaluableExpression GetExpression(string key)
        {
            return this.expressions[key];
        }

        public string GetToken(string key)
        {
            return this.tokens[key];
        }
        public void AddToken(string key, string v)
        {
            this.tokens[key] = v;
        }
        public void AddEvaluableExpression(string k, IEvaluableExpression v)
        {
            this.expressions[k] = v;
        }

        public string GetTokenHash(string token)
        {
            return token.GetHashCode().ToString().Replace("-", "N");
        }

    }
}
