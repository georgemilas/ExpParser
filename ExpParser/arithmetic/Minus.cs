using System;
using System.Collections.Generic;

namespace ExpParser.arithmetic
{
    public class Minus : ArithmeticOperator
    {
        public Minus() : base("-", 0) { }
        public override object Evaluate(object obj, List<IEvaluableExpression> exp) //obj is a string text 
        {
            return Evaluator(obj, exp, (e, a) => a - e,  //int
                                       (e, a) => a - e,  //long
                                       (e, a) => a - e,  //double
                                       (e, a) => a - e,  //float
                                       (e, a) => a - e); //decimal
        }
    }
}
