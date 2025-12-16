using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic.SQL
{
    public class SQLSemantic: BooleanLogicSemantic
    {
        public SQLSemantic(SQLTokenEvaluator tokenEvaluator)
        {
            this.AND = new SqlAND(this);
            this.OR = new SqlOR(this);
            this.NOT = new SqlNOT(this);
            this.TokenEvaluatorInstance = tokenEvaluator;            
        }
    }
}
