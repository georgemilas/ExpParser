using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords
{
    public abstract class KeywordsSemantic: IKeywordsSemantic
    {
        public IOperator AND { get; set; }
        public IOperator OR { get; set; }
        public IOperator NOT { get; set; }

        public Token.TokenEvaluatorFunction TokenEvaluator { get; set; }
    }
}
