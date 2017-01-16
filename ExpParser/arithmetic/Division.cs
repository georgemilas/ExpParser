using System;
using System.Collections.Generic;

namespace ExpParser.arithmetic
{
    public class Division : ArithmeticOperator
    {
        public Division() : base("/", 1) { }
        public override object Evaluate(object obj, List<IEvaluableExpression> exp) //obj is a string text 
        {
            //left to right semantic so
            //accumulator is first for division 
            //10 /2 / 2 => 10/2 => 5/2
            return Evaluator(obj, exp, (e, a) => a / e,  //int   
                                       (e, a) => a / e,  //long
                                       (e, a) => a / e,  //double
                                       (e, a) => a / e,  //float
                                       (e, a) => a / e); //decimal
        }
    }
}
