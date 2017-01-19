using ExpParser.Arithmetic;
using ExpParser.Exceptions;
using NUnit.Framework;
using System;

namespace ExpParser.Tests.Arithmetic
{
    [TestFixture]
    class ArithmeticTester
    {
        public object Evaluate(string exp, object obj)
        {
            var kexp = new ArithmeticExpressionParser(exp, new ArithmenticSemantic());
            return kexp.Evaluate(obj);
        }

        [Test]
        public void TestArithmetic()
        {
            Assert.AreEqual(3, Evaluate("3", null));
            Assert.AreEqual(4, Evaluate("3m+1", null));
            Assert.AreEqual(4, Evaluate("2 * (2)", null));
            Assert.AreEqual(-1, Evaluate("2 - 4 + 1", null));
            Assert.AreEqual(60, Evaluate("2 * 10 + 4 * (5.0 + 5)", null));   
            Assert.AreEqual(45, Evaluate("2 * 10 + 4 * 5 + 5", null));       
            Assert.AreEqual(47.5, Evaluate("2 * 10 + 4.5 * 5 + 5.0", null)); 
            Assert.AreEqual(27, Evaluate("2 * 10 + 10 / 5 + 5", null));      
            Assert.AreEqual(22.5, Evaluate("2 * 10 + 10 / 2 / 2", null));    
            Assert.AreEqual(42.25, Evaluate("(2 * 2 + 10 / 2 / 2)^2", null));
            Assert.AreEqual(36, Evaluate("(2 * 2 + 12 / 3 / 2)^2", null));   
            Assert.AreEqual(144, Evaluate("(2 * 2 + 12 / (3 / 2))^2", null)); 
            Assert.AreEqual(12, Evaluate("2 * 2 + 12 / (3 / 2)", null));
            
        }

        [Test]
        public void TestArithmetic_Errors()
        {
            Assert.Throws<DivideByZeroException>(() => Evaluate("2 / 0 + 1", null));
            Assert.Throws<EvaluationException>(() => Evaluate("2 / '2'", null), "'2' is not one of int, long, float, double, decimal");
            Assert.Throws<ParsingException>(() => Evaluate("() + ()", null));
            Assert.Throws<EvaluationException>(() => Evaluate("bla and bla", null), "bla and bla is not one of int, long, float, double, decimal");
        }


    }
}