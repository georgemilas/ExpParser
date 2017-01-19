using System;
using System.Collections.Generic;

namespace ExpParser.Arithmetic
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
                                       //for double and float .NET returns Infinity while for int, long, decimal is DivideByZeroException
                                       (e, a) => { if (e == 0) { throw new DivideByZeroException(); } else { return a / e; } },  //double
                                       (e, a) => { if (e == 0) { throw new DivideByZeroException(); } else { return a / e; } },  //float
                                       (e, a) => a / e); //decimal
        }
    }
}
