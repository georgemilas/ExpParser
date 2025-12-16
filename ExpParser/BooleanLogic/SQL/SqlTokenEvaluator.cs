using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ExpParser.BooleanLogic.SQL
{
    public class SQLTokenEvaluator : ITokenEvaluator
    {
        public enum OPERATOR_TYPE { EQUAL, LIKE , LIKE_AND_NULL_TEST, ILIKE_ANY_ARRAY };
        public enum FIELD_TYPE { STRING, NUMBER };
        public List<string> fields;
        public OPERATOR_TYPE operatorType;
        public FIELD_TYPE fieldType;
        
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
        
        public Func<object, string, object> TokenEvaluator { get => this.Evaluator; }

        private string EscapeArrayLike(string term)
        {
            return term
                .Replace("\\", "\\\\")  // must be first
                .Replace("%", "\\%")
                .Replace("_", "\\_");
        }

        //obj is the actual operator context (AND, OR NOT), as we are not really evaluating anything but transforming the keywords text into a SQL WHERE clause as string
        public object Evaluator(object obj, string token)
        {
            if ((string)obj == " OR " && operatorType == OPERATOR_TYPE.ILIKE_ANY_ARRAY)
            {
                return EscapeArrayLike(token);        
            }
        
            List<string> res = new List<string>();
            foreach (string field in this.fields)
            {
                if (this.operatorType == OPERATOR_TYPE.LIKE || 
                    this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST || 
                    this.operatorType == OPERATOR_TYPE.ILIKE_ANY_ARRAY
                   )
                {
                    if (this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST)
                    {
                        res.Add(string.Format("({0} IS NOT NULL AND {0} LIKE '%{1}%')", field, EscapeArrayLike(token)));
                    }
                    else if (this.operatorType == OPERATOR_TYPE.ILIKE_ANY_ARRAY)
                    {
                        res.Add(string.Format("{0} ILIKE '%{1}%'", field, EscapeArrayLike(token)));
                    }
                    else
                    {
                        res.Add(string.Format("{0} LIKE '%{1}%'", field, EscapeArrayLike(token)));
                    }
                }
                else
                {
                    string str = "";
                    if (this.fieldType == FIELD_TYPE.STRING) { str = "'"; }
                    res.Add($"{field}={str}{token}{str}");
                }
            }
            return "(" + res.Join(" OR ") + ")";
        }
    }

}
