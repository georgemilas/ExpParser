using ExpParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords.SQL
{
    public class SqlNOT : SQLOperator, IOperator
    {
        //obj is always null, as we are not really evaluating anything but transforming the keywords text into a SQL WHERE clause as string
        public object Evaluate(object obj, IEvaluableExpression exp)
        {
            return string.Format("NOT ({0})", exp.Evaluate(null));
        }
        public object Evaluate(object obj, List<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> cannot negate a list");
        }
        public string ToString(IEvaluableExpression exp)
        {
            return "NOT " + exp.ToString();
        }
        public string ToString(List<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> cannot negate a list");
        }
    }
}
