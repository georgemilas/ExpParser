using System;
using ExpParser.Exceptions;

namespace ExpParser.Arithmetic
{
    public class ArithmenticSemantic : IArithmeticSemantic
    {
        public IOperator PLUS { get; set; }     // +
        public IOperator MINUS { get; set; }    // -
        public IOperator MUL { get; set; }      // *
        public IOperator DIV { get; set; }      // /
        public IOperator POW { get; set; }      // ^     

        public Token.TokenEvaluatorFunction TokenEvaluator { get; set; }

        public ArithmenticSemantic()
        {
            this.PLUS = new Plus();
            this.MINUS = new Minus();
            this.MUL = new Multiply();
            this.DIV = new Division();
            this.POW = new Pow();

            TokenEvaluator = (object obj, string token) =>
            {
                //obj should always be null ;
                int i;
                if (!int.TryParse(token, out i))
                {
                    long l;
                    if (!long.TryParse(token, out l))
                    {
                        float f;
                        if (!float.TryParse(token, out f))
                        {
                            double d;
                            if (!double.TryParse(token, out d))
                            {
                                decimal m;
                                if (!decimal.TryParse(token, out m))
                                {
                                    throw new EvaluationException(token + " is not one of int, long, float, double, decimal");
                                }
                                return m;
                            }
                            return d;
                        }
                        return f;
                    }
                    return l;
                }
                return i;
            };

        }


    }
}
