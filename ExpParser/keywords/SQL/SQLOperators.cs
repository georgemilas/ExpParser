using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords.SQL
{
    public class SQLOperator
    {
        protected string EvaluateAndJoin(string opr, List<IEvaluableExpression> exps)
        {
            List<string> res = new List<string>();
            foreach (IEvaluableExpression a in exps)
            {
                res.Add((string)a.Evaluate(null));                
            }
            if (res.Count > 1)
            {
                return "(" + res.Join(opr) + ")";
            }
            return res[0];
        }
    }
   
}
    

