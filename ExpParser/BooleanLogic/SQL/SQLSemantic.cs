using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic.SQL
{
    public class SQLSemantic: BooleanLogicSemantic
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
