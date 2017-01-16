using System;

namespace ExpParser.ObjectQuery
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



    class ObjectQueryTester
    {
        public ObjectQueryTester() { }
        
        public bool Evaluate(string exp, object obj)
        {
            var kexp = new ObjectQueryExpressionParser(exp, new ObjectEvaluatorSemantic());
            return (bool)kexp.Evaluate(obj);
        }

        public void test()
        {
            bool res;

            //string f3 = "Feet * 12 + Inches <= HeightLimit and (Sedation = null or Sedation = true)";
            //res = Evaluate(f3, new Provider2() { HeightLimit = 100, Feet = 5, Inches = 9 });                   //false


            string f1 = "(Age >= 12 and Gender = Male) or Gender != Male";
            res = Evaluate(f1, new Provider() { Age = 25, Gender = "Male" });  //true
            res = Evaluate(f1, new Provider2() { Age = 25, Gender = "Female" });    //true
            res = Evaluate(f1, new Provider() { Age = 10, Gender = "Male" });      //false

            string f2 = "Weight <= 300 and (Sedation = null or Sedation = true)";
            res = Evaluate(f2, new Provider() { Weight = 400 });                   //false
            res = Evaluate(f2, new Provider() { Weight = 300 });                   //true
            res = Evaluate(f2, new Provider() { Weight = 200, Sedation = false }); //false
            res = Evaluate(f2, new Provider() { Weight = 200, Sedation = true });  //true

            string kw = "(Rate ne 0 and Type eq H) or Type ne H";
            var evRes = Evaluate(kw, new TT() { Rate = 5, Type = "H" });     //true
            evRes = Evaluate(kw, new TT() { Rate = 0, Type = "H" });         //false
            evRes = Evaluate(kw, new TT() { Rate = 0, Type = "$" });         //true
                             
        }
    }
}