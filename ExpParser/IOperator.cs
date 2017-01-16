using System;
using System.Collections.Generic;

namespace ExpParser
{
    public interface IOperator
    {
        object Evaluate(object obj, IEvaluableExpression exp);
        object Evaluate(object obj, List<IEvaluableExpression> exp);
        string ToString(IEvaluableExpression exp);
        string ToString(List<IEvaluableExpression> exp);
    }
}
