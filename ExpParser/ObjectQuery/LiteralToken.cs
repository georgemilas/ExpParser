using ExpParser.Exceptions;
using System;

namespace ExpParser.ObjectQuery
{
    public class LiteralToken : Token
    {
        private PropertyToken prop;
        public LiteralToken(PropertyToken prop, string token): base(token)
        {
            this.prop = prop;
        }

        public override object Evaluate(object obj)
        {
            var propValue = prop.Evaluate(obj);
            if (propValue == null)
            {
                return null;
            }
            Type tp = propValue.GetType();
            try
            {
                if (tp == typeof (int) || tp == typeof(int?))
                {
                    return token == "null" ? (int?)null : int.Parse(token);
                }
                if (tp == typeof (double) || tp == typeof(double?))
                {
                    return token == "null" ? (double?)null : double.Parse(token);
                }
                if (tp == typeof (decimal) || tp == typeof(decimal?))
                {
                    return token == "null" ? (decimal?)null : decimal.Parse(token);
                }
                if (tp == typeof (string))
                {
                    return token;
                }
                if (tp == typeof (bool) || tp == typeof(bool?))
                {
                    return token ==  "null" ? (bool?)null : bool.Parse(token);
                }
                if (tp == typeof (DateTime) || tp == typeof(DateTime?))
                {
                    return token == "null" ? (DateTime?)null : DateTime.Parse(token);
                }
                if (tp == typeof (DateTimeOffset) || tp == typeof(DateTimeOffset?))
                {
                    return token == "null" ? (DateTimeOffset?)null : DateTimeOffset.Parse(token);
                }
            }
            catch (FormatException)
            {
                throw new EvaluationException(string.Format("Input string was not in a correct format, expected {0} but found {1}", tp.Name, token));
            }
            throw new Exception("Impossible to get here in evaluate");
        }

        public bool Compare(object obj, ObjectComparerOperator op)
        {
            var propValue = prop.Evaluate(obj);
            var litValue = Evaluate(obj);
            int res = 0;
            if (propValue is IComparable)
            {
                IComparable comparable = (IComparable)propValue;
                res = comparable.CompareTo(litValue);
            }
            else if ( propValue != litValue )
            {
                throw new EvaluationException(String.Format("Values to compare are not IComparable {0}", token));
            }
            if (op.op == "eq")
            {
                return res == 0;
            }
            if (op.op == "ne")
            {
                return res != 0;
            }
            if (op.op == "lt")
            {
                return res < 0;
            }
            if (op.op == "gt")
            {
                return res > 0;
            }
            if (op.op == "le")
            {
                return res <= 0;
            }
            if (op.op == "ge")
            {
                return res >= 0;
            }
            //throw new Exception("Impossible to get here in Compare");            
            throw new EvaluationException(String.Format("Values to compare are not IComparable {0}", token));                
        }
    }
}