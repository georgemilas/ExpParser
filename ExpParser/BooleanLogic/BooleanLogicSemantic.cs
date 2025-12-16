using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic
{
    public abstract class BooleanLogicSemantic: IBooleanLogicSemantic
    {
        public IOperator AND { get; set; }
        public IOperator OR { get; set; }
        public IOperator NOT { get; set; }

        public ITokenEvaluator TokenEvaluatorInstance { get; set; }
        public virtual Func<object, string, object> TokenEvaluator 
        { 
            get { return TokenEvaluatorInstance.TokenEvaluator; }             
        }
    }
}
