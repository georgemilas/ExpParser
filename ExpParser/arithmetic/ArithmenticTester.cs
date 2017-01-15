using System;

namespace ExpParser.arithmetic
{
    class TT
    {
        public int Weight { get; set; }
        public string Feet { get; set; }
        public string Inches { get; set; }
        public int WeightLimit { get; set; }
        public string HeightLimit { get; set; }
    }

    class ArithmeticTester
    {
        public ArithmeticTester() { }
        
        public object Evaluate(string exp, object obj)
        {
            var kexp = new ArithmeticExpressionParser(exp, new ArithmenticSemantic());
            return kexp.Evaluate(obj);
        }

        public void test()
        {
            object res;

            res = Evaluate("2 * 10 + 4 * (5.0 + 5)", null);   //60
            res = Evaluate("2 * 10 + 4 * 5 + 5", null);       //45
            res = Evaluate("2 * 10 + 4.5 * 5 + 5.0", null);   //47.5
            res = Evaluate("2 * 10 + 10 / 5 + 5", null);      //27
            res = Evaluate("2 * 10 + 10 / 2 / 2", null);       //22.5
            res = Evaluate("(2 * 2 + 10 / 2 / 2)^2", null);   //42.25
            res = Evaluate("(2 * 2 + 12 / 3 / 2)^2", null);   //36
            res = Evaluate("(2 * 2 + 12 / (3 / 2))^2", null); //144
            res = Evaluate("2 * 2 + 12 / (3 / 2)", null);     //12

        }
    }
}