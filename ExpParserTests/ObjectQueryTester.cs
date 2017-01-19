using ExpParser.Exceptions;
using ExpParser.ObjectQuery;
using NUnit.Framework;
using System;

namespace ExpParser.Tests.ObjectQuery
{
    class TT
    {
        public int Rate { get; set; }
        public string Type { get; set; }
    }

    class Provider
    {
        public double? Weight { get; set; }
        public int? Age { get; set; }
        public bool? Sedation{ get; set; }
        public string Gender { get; set; }
    }

    class Provider2
    {
        public int? Age { get; set; }
        public string Gender { get; set; }
        public int Feet { get; set; }
        public int Inches { get; set; }
        public int HeightLimit { get; set; }   //in inches
    }


    [TestFixture]
    class ObjectQueryTester
    {
        
        public bool Evaluate(string exp, object obj)
        {
            var kexp = new ObjectQueryExpressionParser(exp, new ObjectEvaluatorSemantic());
            return (bool)kexp.Evaluate(obj);
        }

        [Test]
        public void TestObjectEvaluation()
        {
            bool res;

            //string f3 = "Feet * 12 + Inches <= HeightLimit and (Sedation = null or Sedation = true)";
            //res = Evaluate(f3, new Provider2() { HeightLimit = 100, Feet = 5, Inches = 9 });                   //false

            string f1 = "(Age >= 12 and Gender = Male) or Gender != Male";
            Assert.AreEqual(true, Evaluate(f1, new Provider() { Age = 25, Gender = "Male" }));  //true
            Assert.AreEqual(true, res = Evaluate(f1, new Provider2() { Age = 25, Gender = "Female" }));    //true
            Assert.AreEqual(false, Evaluate(f1, new Provider() { Age = 10, Gender = "Male" }));      //false

            string f2 = "Weight <= 300 and (Sedation = null or Sedation = true)";
            Assert.AreEqual(false, Evaluate(f2, new Provider() { Weight = 400 }));                   //false
            Assert.AreEqual(true, Evaluate(f2, new Provider() { Weight = 300 }));                   //true
            Assert.AreEqual(false, Evaluate(f2, new Provider() { Weight = 200, Sedation = false })); //false
            Assert.AreEqual(true, Evaluate(f2, new Provider() { Weight = 200, Sedation = true }));  //true

            string kw = "(Rate ne 0 and Type eq H) or Type ne H";
            Assert.AreEqual(true, Evaluate(kw, new TT() { Rate = 5, Type = "H" }));     //true
            Assert.AreEqual(false, Evaluate(kw, new TT() { Rate = 0, Type = "H" }));         //false
            Assert.AreEqual(true, Evaluate(kw, new TT() { Rate = 0, Type = "$" }));         //true
                             
        }

        [Test]
        public void TestObjectEvaluation_EvaluationErrors()
        {
            Assert.Throws<EvaluationException>(() => Evaluate("Bogus >= 12", new Provider()), "Property Bogus was not found");  
            Assert.Throws<EvaluationException>(() => Evaluate("Age >= blablabla", new Provider() { Age = 25}), "Input string was not in a correct format, expected Int32 but found blablabla");

        }

        [Test]
        public void TestObjectEvaluation_ParsingErrors()
        {
            Assert.Throws<ParsingException>(() => Evaluate("Age >= new Provider()", new Provider() { Age = 25 }), "Empty parentheses expression () is not supported");
        }


    }
}