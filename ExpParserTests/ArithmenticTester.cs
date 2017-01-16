using ExpParser.Arithmetic;
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
    }
}