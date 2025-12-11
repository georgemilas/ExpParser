using ExpParser.Arithmetic;
using ExpParser.Exceptions;
using Xunit;
using System;

namespace ExpParser.Tests.Arithmetic
{
    public class ArithmeticTester
    {
        public object Evaluate(string exp, object obj)
        {
            var kexp = new ArithmeticExpressionParser(exp, new ArithmenticSemantic());
            return kexp.Evaluate(obj);
        }

        [Fact]
        public void TestArithmetic()
        {
            Assert.Equal(3, Evaluate("3", null));
            Assert.Equal(4.0, (double)Evaluate("3m+1", null));
            Assert.Equal(4.0, (double)Evaluate("2 * (2)", null));
            Assert.Equal(-1.0, (double)Evaluate("2 - 4 + 1", null));
            Assert.Equal(60.0, (double)Evaluate("2 * 10 + 4 * (5.0 + 5)", null));   
            Assert.Equal(45.0, (double)Evaluate("2 * 10 + 4 * 5 + 5", null));       
            Assert.Equal(47.5, (double)Evaluate("2 * 10 + 4.5 * 5 + 5.0", null)); 
            Assert.Equal(27.0, (double)Evaluate("2 * 10 + 10 / 5 + 5", null));      
            Assert.Equal(22.5, (double)Evaluate("2 * 10 + 10 / 2 / 2", null));    
            Assert.Equal(42.25, (double)Evaluate("(2 * 2 + 10 / 2 / 2)^2", null));
            Assert.Equal(36.0, (double)Evaluate("(2 * 2 + 12 / 3 / 2)^2", null));   
            Assert.Equal(144.0, (double)Evaluate("(2 * 2 + 12 / (3 / 2))^2", null)); 
            Assert.Equal(12.0, (double)Evaluate("2 * 2 + 12 / (3 / 2)", null));
            
        }

        [Fact]
        public void TestArithmetic_Errors()
        {
            Assert.Throws<DivideByZeroException>(() => Evaluate("2 / 0 + 1", null));
            Assert.Throws<EvaluationException>(() => Evaluate("2 / '2'", null));
            Assert.Throws<ParsingException>(() => Evaluate("() + ()", null));
            Assert.Throws<EvaluationException>(() => Evaluate("bla and bla", null));
        }


    }
}