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
            Assert.True((bool)parser.Evaluate("paul milas and george are going to see a movie"));
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
        public void Test_KeywordsExpressionParser_regex_SQL()
        {
            string kw = "(george {paul milas}) & !{\\d*}";
            SQLTokenEvaluator te = new SQLTokenEvaluator("name", SQLTokenEvaluator.OPERATOR_TYPE.EQUAL, SQLTokenEvaluator.FIELD_TYPE.STRING);
            var parser = new KeywordsExpressionParser(kw, new SQLSemantic(te));
            var res = (string)parser.Evaluate(null);
            var mustBe = "(((name='george') OR ((name ~* 'paul milas'))) AND NOT (((name ~* '\\d*'))))";
            
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

        /// <summary>
        /// paris / paris and not louvre  LIKE ANY ARRAY
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_PostgreSQL_ilike_array_basic()
        {
            var te = new SQLTokenEvaluator("image_path", SQLTokenEvaluator.OPERATOR_TYPE.ILIKE_ANY_ARRAY, SQLTokenEvaluator.FIELD_TYPE.STRING);
            var parser = new KeywordsExpressionParser("paris", new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            var mustBe = "(image_path ILIKE '%paris%')";

            Assert.Equal(mustBe, where);

            parser = new KeywordsExpressionParser("paris and not louvre", new SQLSemantic(te));
            where = (string)parser.Evaluate(null);
            mustBe = "((image_path ILIKE '%paris%') AND NOT ((image_path ILIKE '%louvre%')))";

            Assert.Equal(mustBe, where);

            parser = new KeywordsExpressionParser($"\"paris france\"", new SQLSemantic(te));
            where = (string)parser.Evaluate(null);
            mustBe = "(image_path ILIKE '%paris france%')";

            Assert.Equal(mustBe, where);

        }


        /// <summary>
        /// (maria gheorghe) and not (andrew anthony)   LIKE ANY ARRAY
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_PostgreSQL_ilike_array()
        {
            var te = new SQLTokenEvaluator("fld_name", SQLTokenEvaluator.OPERATOR_TYPE.ILIKE_ANY_ARRAY, SQLTokenEvaluator.FIELD_TYPE.STRING);
            var parser = new KeywordsExpressionParser("(maria gheorghe) and not (andrew anthony)", new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            var mustBe = "((fld_name ILIKE ANY(ARRAY['%maria%','%gheorghe%'])) AND NOT ((fld_name ILIKE ANY(ARRAY['%andrew%','%anthony%']))))";

            Assert.Equal(mustBe, where);
        }

        
        /// <summary>
        /// (barcelona and (8024 8004 981)) or (barcelona and phone and (3932 448 5453))  LIKE ANY ARRAY
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_PostgreSQL_ilike_array_complex()
        {
            var te = new SQLTokenEvaluator("image_path", SQLTokenEvaluator.OPERATOR_TYPE.ILIKE_ANY_ARRAY, SQLTokenEvaluator.FIELD_TYPE.STRING);
            string expr = @"(barcelona and (8024 8004 981)) or (barcelona and phone and (3932 448 5453))";
            var parser = new KeywordsExpressionParser(expr, new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            var mustBe = "(((image_path ILIKE '%barcelona%') AND (image_path ILIKE ANY(ARRAY['%8024%','%8004%','%981%']))) OR ((image_path ILIKE '%barcelona%') AND (image_path ILIKE '%phone%') AND (image_path ILIKE ANY(ARRAY['%3932%','%448%','%5453%']))))";
            Assert.Equal(mustBe, where);
        }

        /// <summary>
        /// (barcelona and (8024 8004 981)) or (barcelona and phone and (3932 448 5453))  LIKE ANY ARRAY
        /// </summary>
        [Fact]
        public void Test_KeywordsExpressionParser_PostgreSQL_ilike_array_complex2()
        {
            var te = new SQLTokenEvaluator("image_path", SQLTokenEvaluator.OPERATOR_TYPE.ILIKE_ANY_ARRAY, SQLTokenEvaluator.FIELD_TYPE.STRING);
            string expr = @"(mihai and not moni) liliana";
            var parser = new KeywordsExpressionParser(expr, new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            var mustBe = "(((image_path ILIKE '%mihai%') AND NOT ((image_path ILIKE '%moni%'))) OR (image_path ILIKE '%liliana%'))";
            Assert.Equal(mustBe, where);

            expr = "\"mihai's birthday\"";
            parser = new KeywordsExpressionParser(expr, new SQLSemantic(te));
            where = (string)parser.Evaluate(null);
            mustBe = "(image_path ILIKE '%mihai''s birthday%')";
            Assert.Equal(mustBe, where);
        }
    }






}
