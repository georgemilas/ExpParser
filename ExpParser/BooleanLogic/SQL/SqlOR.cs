using ExpParser.Exceptions;
using ExtParser.Extensions;
using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic.SQL
{
    public class SqlOR : SQLOperator, IOperator
    {
        //obj is always null, as we are not really evaluating anything but transforming the keywords text into a SQL WHERE clause as string
        public object Evaluate(object obj, IEvaluableExpression exp) { throw new EvaluationException("IOperator._AND -> cannot evaluate one element, needs a list of elements"); }
        public object Evaluate(object obj, List<IEvaluableExpression> exps)
        {
            return EvaluateAndJoin(" OR ", exps);
        }
        public string ToString(IEvaluableExpression exp)
        {
            return exp.ToString();
        }
        public string ToString(List<IEvaluableExpression> exps)
        {
            return "(" + exps.Join(" OR ") + ")";
        }
    }
}
