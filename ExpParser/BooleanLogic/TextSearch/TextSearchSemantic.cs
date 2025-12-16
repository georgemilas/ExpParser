using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExpParser.BooleanLogic.TextSearch
{
    public class TextSearchSemantic: BooleanLogicSemantic, ITokenEvaluator
    {
        public TextSearchSemantic()
        {
            //this are short-circuit operators evaluators so maybe not everything will be evaluated
            this.AND = new SearchAND();
            this.OR = new SearchOR();
            this.NOT = new SearchNOT();
            this.TokenEvaluatorInstance = this;            
        }

        public override Func<object, string, object> TokenEvaluator { get => this.KeywordFinder; }

        //obj is a string text 
        public object KeywordFinder(object obj, string token)
        {
            string txt = (string) obj;
            //a regular expression keyword is between {} 
            if (token.StartsWith("{") && token.EndsWith("}"))
            {
                Regex r = new Regex(token.Slice(1, -1), RegexOptions.IgnoreCase);
                Match res = r.Match(txt);
                return res.Success;
            }
            //otherwise just TokenEvaluator for the text keyword
            return txt.IndexOf(token) >= 0;
        }

    }

}
