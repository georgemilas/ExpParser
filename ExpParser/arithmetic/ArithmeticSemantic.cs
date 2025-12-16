using System;
using ExpParser.Exceptions;
using System.Text.RegularExpressions;
using ExtParser.Extensions;
using System.Reflection.Metadata.Ecma335;

namespace ExpParser.Arithmetic
{
    public class ArithmenticSemantic : IArithmeticSemantic
    {
        public IOperator PLUS { get; set; }     // +
        public IOperator MINUS { get; set; }    // -
        public IOperator MUL { get; set; }      // *
        public IOperator DIV { get; set; }      // /
        public IOperator POW { get; set; }      // ^     

        private Func<object, string, object> _arithmeticTokenEvaluator;
        public ArithmenticSemantic()
        {
            this.PLUS = new Plus();
            this.MINUS = new Minus();
            this.MUL = new Multiply();
            this.DIV = new Division();
            this.POW = new Pow();
            
            _arithmeticTokenEvaluator = (object obj, string token) =>
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
                                var decr = new Regex(@"\d+(\.\d+)?m", RegexOptions.IgnoreCase);   //3m, 3M, 3.3m
                                if (decr.IsMatch(token))
                                {
                                    token = token.Slice(0,-1);
                                }
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

        public virtual Func<object, string, object> TokenEvaluator { get => this._arithmeticTokenEvaluator; }

        public ITokenEvaluator TokenEvaluatorInstance { get { return this; } set { } }
        

    }
}
