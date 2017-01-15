using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ExpParser.keywords.SQL;
using ExpParser.Exceptions;

namespace ExpParser.keywords
{
    class KeywordsTester
    {
        public KeywordsTester()
        {
        }

       
        public void test()
        {
            string kw = "(george and ((maria and andrew) or \"paul milas\"))   mona eugen \"milas family\"";
            Console.WriteLine(kw);
            
            KeywordsExpressionParser kexp = new KeywordsExpressionParser(kw);
            
            if ( (bool)kexp.Evaluate("paul milas and andrew are going to see a movie") != false ) 
                throw new ParsingException("bad TRUE: paul milas and andrew are going to see a movie");
    
            if ( (bool)kexp.Evaluate("\"paul milas\" and george are going to see a movie") != true )
                throw new ParsingException("FALSE: paul milas and george are going to see a movie");
    
            if ( (bool)kexp.Evaluate("paul and george milas are going to see a movie") != false )
                throw new ParsingException("TRUE: paul and george milas are going to see a movie");
    
            if ( (bool)kexp.Evaluate("george, maria, andrew and mona are going to see a movie") != true )
                throw new ParsingException("FALSE george, maria, andrew and mona are going to see a movie");

            SQLTokenEvaluator te = new SQLTokenEvaluator("name", SQLTokenEvaluator.OPERATOR_TYPE.EQUAL, SQLTokenEvaluator.FIELD_TYPE.STRING);
            kexp = new KeywordsExpressionParser(kw, new SQLSemantic(te));
            string res = (string)kexp.Evaluate(null);
            string mustBe = "(((name='george') AND (((name='maria') AND (name='andrew')) OR (name='paul milas'))) OR (name='mona') OR (name='eugen') OR (name='milas family'))";

            if ( res != mustBe ) 
            {
                throw new ParsingException(string.Format("got {0}, {2}expected: {1}", res, mustBe, Environment.NewLine));
            }

            kw = "(george {\"paul milas\"}) & !{\\d}";
            Console.WriteLine(kw);
            kexp = new KeywordsExpressionParser(kw);
            if ((bool)kexp.Evaluate("gheorghe") != false) throw new ParsingException("TRUE: gheorghe");
            if ((bool)kexp.Evaluate("george") != true) throw new ParsingException("FALSE: george");
            if ((bool)kexp.Evaluate("george 123") != false) throw new ParsingException("TRUE: george 123");


            kexp = new KeywordsExpressionParser(kw, new SQLSemantic(te));
            res = (string)kexp.Evaluate(null);
            mustBe = "(((name='george') OR (name='{\"paul milas\"}')) AND NOT ((name='{\\d}')))";

            if (res != mustBe)
            {
                throw new ParsingException(string.Format("got {0}, {2}expected: {1}", res, mustBe, Environment.NewLine));
            }


            te = new SQLTokenEvaluator("fld_name", SQLTokenEvaluator.OPERATOR_TYPE.LIKE, SQLTokenEvaluator.FIELD_TYPE.STRING);
            KeywordsExpressionParser parser = new KeywordsExpressionParser("(maria gheorghe) and not (andrew anthony)", new SQLSemantic(te));
            string where = (string)parser.Evaluate(null);
            mustBe = "(((fld_name LIKE '%maria%') OR (fld_name LIKE '%gheorghe%')) AND NOT (((fld_name LIKE '%andrew%') OR (fld_name LIKE '%anthony%'))))";

            if (where != mustBe)
            {
                throw new ParsingException(string.Format("got {0}, {2}expected: {1}", res, mustBe, Environment.NewLine));
            }
	

        }

    }






}
