using ExpParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords.TextSearch
{
    public class SearchNOT : IOperator
    {
        public object Evaluate(object obj, List<IEvaluableExpression> exp) //obj is a string text 
        {
            throw new EvaluationException("IOperator._NOT -> cannot negate a list");
        }
        public object Evaluate(object obj, IEvaluableExpression exp)        //obj is a string text 
        {
            return !(bool)exp.Evaluate(obj);
        }
        public string ToString(IEvaluableExpression exp)
        {
            return " NOT " + exp.ToString();
        }
        public string ToString(List<IEvaluableExpression> exps)
        {
            throw new EvaluationException("IOperator._NOT -> cannot negate a list");
        }
    }
}
