using System;

namespace ExpParser
{
    public interface ISemantic: ITokenEvaluator
    {
        ITokenEvaluator TokenEvaluatorInstance { get; set;}         
    }
}
