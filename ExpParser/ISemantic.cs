using System;

namespace ExpParser
{
    public interface ISemantic
    {
        Token.TokenEvaluatorFunction TokenEvaluator { get; set;}
    }
}
