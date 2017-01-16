using ExtParser.Extensions;
using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic.SQL
{
    public class SQLTokenEvaluator
    {
        public enum OPERATOR_TYPE { EQUAL, LIKE , LIKE_AND_NULL_TEST };
        public enum FIELD_TYPE { STRING, NUMBER };
        protected List<string> fields;
        protected OPERATOR_TYPE operatorType;
        protected FIELD_TYPE fieldType;

        public SQLTokenEvaluator(string field, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType):
            this(new List<string>(new string[] { field }), operatorType, fieldType)
        {
            
        }
        public SQLTokenEvaluator(List<string> fields, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType)
        {
            this.fields = fields;
            this.operatorType = operatorType;
            this.fieldType = fieldType;
        }

        //obj is always null, as we are not really evaluating anything but transforming the keywords text into a SQL WHERE clause as string
        public object Evaluator(object obj, string token)
        {
            List<string> res = new List<string>();
            foreach (string field in this.fields)
            {
                if (this.operatorType == OPERATOR_TYPE.LIKE || 
                    this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST
                   )
                {
                    if (this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST)
                    {
                        res.Add(string.Format("({0} IS NOT NULL AND {0} LIKE '%{1}%')", field, token));
                    }
                    else
                    {
                        res.Add(string.Format("{0} LIKE '%{1}%'", field, token));
                    }
                }
                else
                {
                    string str = "";
                    if (this.fieldType == FIELD_TYPE.STRING) { str = "'"; }
                    res.Add(string.Format("{0}={2}{1}{2}", field, token, str));
                }
            }
            return "(" + res.Join(" OR ") + ")";
        }
    }

}
