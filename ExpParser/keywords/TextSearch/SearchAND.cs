using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords.TextSearch
{
    public class SearchAND : IOperator
    {
        public object Evaluate(object obj, IEvaluableExpression exp) { return true; }  //obj is a string text 
        public object Evaluate(object obj, List<IEvaluableExpression> exps)
        {
            foreach (IEvaluableExpression e in exps)
            {
                bool res = (bool)e.Evaluate(obj);
                if (!res)
                {
                    return false;
                }
            }
            return true;
        }
        public string ToString(IEvaluableExpression exp)
        {
            return exp.ToString();
        }
        public string ToString(List<IEvaluableExpression> exps)
        {
            return "(" + exps.Join(" AND ") + ")";
        }
    }
}
