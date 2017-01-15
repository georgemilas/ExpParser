using ExpParser.Exceptions;
using System;
using System.Collections.Generic;

namespace ExpParser.ObjectQuery
{
    public class PropertyToken : Token
    {
        public PropertyToken(string token): base(token) { }
        //private object evaled = null;
        
        public override object Evaluate(object obj)
        {
            var p = obj.GetType().GetProperty(token);
            if (p == null)
            {
                throw new EvaluationException(String.Format("Property {0} was not found", token));                    
            }
            var evaled = p.GetValue(obj, null);
            Verify(evaled, p.PropertyType);
            
            return evaled;                
        }

        public void Verify(object value, Type propType)
        {
            Type tp = value == null ? propType : value.GetType();
            List<Type> types = new List<Type>() { typeof(int), typeof(int?),
                                                  typeof(double), typeof(double?),
                                                  typeof(decimal), typeof(decimal?),
                                                  typeof(string),
                                                  typeof(bool), typeof(bool?),
                                                  typeof(DateTime), typeof(DateTime?),
                                                  typeof(DateTimeOffset), typeof(DateTimeOffset?) };
            if (!types.Contains(tp))
            {
                throw new EvaluationException(String.Format("Property {0} is of type {1}. Supported types are int, double, decimal, string, DateTime, DateTimeOffset and nullable versions of them", token, tp));
            }
        }        
        
    }
}