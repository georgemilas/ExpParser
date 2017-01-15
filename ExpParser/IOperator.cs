using System;
using System.Collections.Generic;

namespace ExpParser
{
    public interface IOperator
    {
        //static bool _AND(string text, List<IEvaluableExpression> args);
        //static bool _NOT(string text, IEvaluableExpression arg);
        //static bool _OR(string text, List<IEvaluableExpression> args);

        object Evaluate(object obj, IEvaluableExpression exp);
        object Evaluate(object obj, List<IEvaluableExpression> exp);
        string ToString(IEvaluableExpression exp);
        string ToString(List<IEvaluableExpression> exp);
    }
}
