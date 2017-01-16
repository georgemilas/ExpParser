using System;
using ExtParser.Extensions;
using System.Text.RegularExpressions;

namespace ExpParser.BooleanLogic
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
    public class KeywordsExpressionParser : BooleanLogicExpressionParser
    {
        public KeywordsExpressionParser(string keywordsExpressionString) : base(keywordsExpressionString, new TextSearch.TextSearchSemantic()) { }

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
            keys = QuotedStrings(keys);

            //3. align tokensContainer and expressions one next to the other 
            //since whitespace has a meaning in this language
            Regex r = new Regex(@"\s+");
            keys = r.Replace(keys, " ");    //reduce multiple spaces to 1 space

            return keys;
        }



        /// <summary>
        /// Tokenize expressions between quotes 
        /// </summary>
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
                    tokensContainer.AddToken("token#" + hs, token.Slice(1, -1).Replace(quoteEscape.ToString() + quote.ToString(), quote.ToString()));    //strip out enclosing quote symbol and un-escape quotes
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



    }

}
