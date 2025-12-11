using ExpParser.Exceptions;
using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpParser.Arithmetic
{
    public abstract class ArithmeticOperator : IOperator
    {
        protected string OP { get; set; }
        protected int Acum { get; set; }

        protected ArithmeticOperator(string op, int acum = 0)
        {
            this.OP = op;
            this.Acum = acum;
        }

        public abstract object Evaluate(object obj, List<IEvaluableExpression> exp);

        public object Evaluator(object obj, List<IEvaluableExpression> exp , Func<int, int, int> intEval
                                                                            , Func<long, long, long> longEval
                                                                            , Func<double, double, double> doubleEval
                                                                            , Func<float, float, float> floatEval
                                                                            , Func<decimal, decimal, decimal> decimalEval) 
        {
            var evaled = exp.Map(e => { var res = e.Evaluate(obj); return res; });
            double startAcum = this.Acum;
            if (OP == "/" || OP == "^" || OP == "-")
            {
                startAcum = Convert.ToDouble(evaled.First());  //acumulator is double so every result will be double
                evaled = evaled.SliceEx(1);
            }
            return evaled.Reduce((el, acum) =>
            {
                if (el.GetType() == typeof(float) || el.GetType() == typeof(double) || acum.GetType() == typeof(float) || acum.GetType() == typeof(double))
                {
                    var res = doubleEval(Convert.ToDouble(el), Convert.ToDouble(acum));
                    return res;
                }

                if (el.GetType() == typeof(int) || el.GetType() == typeof(long) && (acum.GetType() == typeof(int) || acum.GetType() == typeof(long)))
                {
                    if (OP == "/" || OP == "^")
                    {
                        var res = doubleEval(Convert.ToDouble(el), Convert.ToDouble(acum));
                        return res;
                    }
                    var lres = longEval(Convert.ToInt64(el), Convert.ToInt64(acum));
                    return lres;
                }
                
                if (el.GetType() == typeof(decimal) || acum.GetType() == typeof(decimal))
                {
                    var res = decimalEval(Convert.ToDecimal(el), Convert.ToDecimal(acum));
                    return res;
                }

                throw new EvaluationException("Operator " + OP + " is not supported for " + el.GetType());
            }, startAcum);
        }
        public object Evaluate(object obj, IEvaluableExpression exp)        //obj is a string text 
        {
            throw new EvaluationException("Operator " + OP + " is not supported for one element");
        }
        public string ToString(IEvaluableExpression exp)
        {
            return exp.ToString();
        }
        public string ToString(List<IEvaluableExpression> exps)
        {
            return "(" + exps.Join(" " + OP + " ") + ")";
        }
    }

}
