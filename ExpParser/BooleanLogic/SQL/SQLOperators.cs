using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpParser.BooleanLogic.SQL
{
    public class SQLOperator
    {

        public SQLOperator()
        {
            
        }
        public SQLOperator(ISemantic semantic)
        {
            this._semantic = semantic;            
        }

        protected ISemantic _semantic;
        public ISemantic Semantic
        {
            get { return _semantic; }
            set { _semantic = value; }
        }

        

        protected string EvaluateAndJoin(string opr, List<IEvaluableExpression> exps)
        {

            var evalInstance = (SQLTokenEvaluator)Semantic.TokenEvaluatorInstance;
            bool allConstants = exps.All(e => e.IsConstant);
            if (opr == " OR " && evalInstance.operatorType == SQLTokenEvaluator.OPERATOR_TYPE.ILIKE_ANY_ARRAY && allConstants)
            {
                // Handle LIKE_ANY_ARRAY operator type
                var arr = exps.Map(a => $"'%{(string)a.Evaluate(opr)}%'"); 
                if (arr.Count() > 0)
                {
                    string token = string.Join(",", arr);
                    return $"({evalInstance.fields[0]} ILIKE ANY(ARRAY[{token}]))";
                }   
            }

            List<string> res = new List<string>();
            foreach (IEvaluableExpression a in exps)
            {
                res.Add((string)a.Evaluate(opr));                
            }
            if (res.Count > 1)
            {
                return "(" + res.Join(opr) + ")";
            }
            return res[0];
        }
    }
   
}
    

