using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords.SQL
{
    public class SQLSemantic: KeywordsSemantic
    {
        public SQLSemantic(SQLTokenEvaluator tokenEvaluator)
        {
            this.AND = new SqlAND();
            this.OR = new SqlOR();
            this.NOT = new SqlNOT();
            this.TokenEvaluator = tokenEvaluator.Evaluator;
        }
    }
}
