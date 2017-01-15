using ExtParser.Extensions;
using System;
using System.Collections.Generic;

namespace ExpParser.ObjectQuery
{
    public class ObjectComparerOperator : IOperator
    {
        public string op;
        //private Func<object, object, bool> comparer;

        public ObjectComparerOperator(string op) //, Func<object, object, bool> comparer)
        {
            this.op = op;
            //this.comparer = comparer;
        }
        public object Evaluate(object obj, IEvaluableExpression exp) { throw new Exception("Can't eq on one parameter, need 2"); }  //obj is a string text 
        public object Evaluate(object obj, List<IEvaluableExpression> exps)
        {
            throw new NotImplementedException("Evaluation should go to OperatorExpression and should not come down to IOperator");
            //var left = (bool)exps.First().Evaluate(obj);
            //var right = (bool)exps.Skip(1).First().Evaluate(obj);
            //return comparer(left, right);
        }
        public string ToString(IEvaluableExpression exp) { return exp.ToString(); }
        public string ToString(List<IEvaluableExpression> exps) { return exps.Join(" " + this.op + " "); }
    }
}