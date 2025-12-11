using System;
using ExpParser.BooleanLogic.SQL;
using ExpParser.BooleanLogic;
using Xunit;

namespace ExpParser.Tests.BooleanLogic
{
    public class KeywordsTester
    {
        /// <summary>
        /// (george and ((maria and andrew) or "paul milas"))   mona eugen "milas family"
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_eval1()
        {
            string kw = "(george and ((maria and andrew) or \"paul milas\"))   mona eugen \"milas family\"";

            ExpBaseParser parser = new KeywordsExpressionParser(kw);

            Assert.False((bool)parser.Evaluate("paul milas and andrew are going to see a movie"));
            Assert.True((bool)parser.Evaluate("\"paul milas\" and george are going to see a movie"));
            Assert.False((bool)parser.Evaluate("paul and george milas are going to see a movie"));
            Assert.True((bool)parser.Evaluate("george, maria, andrew and mona are going to see a movie"));
        }

        /// <summary>
        /// (george and ((maria and andrew) or "paul milas"))   mona eugen "milas family"
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_SQL1()
        {
            string kw = "(george and ((maria and andrew) or \"paul milas\"))   mona eugen \"milas family\"";
            SQLTokenEvaluator te = new SQLTokenEvaluator("name", SQLTokenEvaluator.OPERATOR_TYPE.EQUAL, SQLTokenEvaluator.FIELD_TYPE.STRING);
            var parser = new KeywordsExpressionParser(kw, new SQLSemantic(te));
            string res = (string)parser.Evaluate(null);
            string mustBe = "(((name='george') AND (((name='maria') AND (name='andrew')) OR (name='paul milas'))) OR (name='mona') OR (name='eugen') OR (name='milas family'))";

            Assert.Equal(mustBe, res);
        }


        

        /// <summary>
        /// (george {"paul milas"}) & !{\d}
        /// </summary>
        [Fact]        
        public void Test_KeywordsExpressionParser_eval2()
        {
            string kw = "(george {\"paul milas\"}) & !{\\d}";

            var parser = new KeywordsExpressionParser(kw);
            Assert.False((bool)parser.Evaluate("gheorghe"));
            Assert.True((bool)parser.Evaluate("george"));
            Assert.False((bool)parser.Evaluate("george 123"));            

        }

        /// <summary>
        /// (george {"paul milas"}) & !{\d}
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_SQL2()
        {
            string kw = "(george {\"paul milas\"}) & !{\\d}";
            SQLTokenEvaluator te = new SQLTokenEvaluator("name", SQLTokenEvaluator.OPERATOR_TYPE.EQUAL, SQLTokenEvaluator.FIELD_TYPE.STRING);
            var parser = new KeywordsExpressionParser(kw, new SQLSemantic(te));
            var res = (string)parser.Evaluate(null);
            var mustBe = "(((name='george') OR (name='{\"paul milas\"}')) AND NOT ((name='{\\d}')))";
            
            Assert.Equal(mustBe, res);
        }








        /// <summary>
        /// (maria gheorghe) and not (andrew anthony)
        /// </summary>
        [Fact]
        public void Test_BooleanLogicExpressionParser_SQL_like()
        {
            var te = new SQLTokenEvaluator("fld_name", SQLTokenEvaluator.OPERATOR_TYPE.LIKE, SQLTokenEvaluator.FIELD_TYPE.STRING);
            //using BooleanLogicExpressionParser instead of KeywordsExpressionParser is OK as we are not using neither RegEx nor quoted strings
            var parser = new BooleanLogicExpressionParser("(maria gheorghe) and not (andrew anthony)", new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            var mustBe = "(((fld_name LIKE '%maria%') OR (fld_name LIKE '%gheorghe%')) AND NOT (((fld_name LIKE '%andrew%') OR (fld_name LIKE '%anthony%'))))";

            Assert.Equal(mustBe, where);
        }


    }






}
