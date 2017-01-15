using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser
{
    public interface ISemantic
    {
        Token.TokenEvaluatorFunction TokenEvaluator { get; set;}
    }
}
