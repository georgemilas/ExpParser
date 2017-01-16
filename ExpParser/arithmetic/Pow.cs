using System;
using System.Collections.Generic;

namespace ExpParser.arithmetic
{
    public class Pow : ArithmeticOperator
    {
        public Pow() : base("^", 1) { }
        public override object Evaluate(object obj, List<IEvaluableExpression> exp) //obj is a string text 
        {
            //accumulator first just like with division (in 2 ^ 3  ^ 3 => 2^3 => 8^3
            return Evaluator(obj, exp, (e, a) => Convert.ToInt32(Math.Pow(a, e)),   //int
                                       (e, a) => Convert.ToInt64(Math.Pow(a, e)),   //long
                                       (e, a) => Convert.ToDouble(Math.Pow(a, e)),  //double
                                       (e, a) => Convert.ToSingle(Math.Pow(a, e)),  //float
                                       (e, a) => Convert.ToDecimal(Math.Pow((double)a, (double)e))); //decimal
        }
    }
}
