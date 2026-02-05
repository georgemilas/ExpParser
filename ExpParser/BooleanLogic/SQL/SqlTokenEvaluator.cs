using ExtParser.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ExpParser.BooleanLogic.SQL
{
    public record SqlFields 
    {
        public string DefaultField { get; set; } = string.Empty;
        //public string FaceSQL { get; set; } = string.Empty;
        public string FaceField { get; set; } = string.Empty;
        public string LocationField { get; set; } = string.Empty;
        public string DateField { get; set; } = string.Empty;

        /// <summary>
        /// Shortcut for the default field 
        /// </summary>
        public string Field {get { return this.DefaultField; }  }
    }

    public class SQLTokenEvaluator : ITokenEvaluator
    {
        public enum OPERATOR_TYPE { EQUAL, LIKE , LIKE_AND_NULL_TEST, ILIKE_ANY_ARRAY };
        public enum FIELD_TYPE { STRING, NUMBER };
        public SqlFields fields;
        public OPERATOR_TYPE operatorType;
        public FIELD_TYPE fieldType;
        
        public SQLTokenEvaluator(string field, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType):
            this(new SqlFields { DefaultField = field }, operatorType, fieldType)
        {
            
        }
        public SQLTokenEvaluator(SqlFields fields, OPERATOR_TYPE operatorType, FIELD_TYPE fieldType)
        {
            this.fields = fields;
            this.operatorType = operatorType;
            this.fieldType = fieldType;
        }
        
        public Func<object, string, object> TokenEvaluator { get => this.Evaluator; }

        public static string EscapeArrayLike(string term)
        {
            return term
                .Replace("\\", "\\\\")  // must be first
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("'", "''");
        }

        //obj is the actual operator context (AND, OR NOT), as we are not really evaluating anything but transforming the keywords text into a SQL WHERE clause as string
        public object Evaluator(object obj, string token)
        {

            var field = this.fields.Field;
                
            //a special token such as a regular expression keyword is between {}
            //regardless of the operatorType we do this as a special evaluation independend from operatorType 
            if (token.StartsWith("{") && token.EndsWith("}"))
            {
                var txt = EvaluateSpecialToken(token);
                return txt;
            }

            if (this.operatorType == OPERATOR_TYPE.LIKE || 
                this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST || 
                this.operatorType == OPERATOR_TYPE.ILIKE_ANY_ARRAY
                )
            {
                if (this.operatorType == OPERATOR_TYPE.LIKE_AND_NULL_TEST)
                {
                    return string.Format($"({field} IS NOT NULL AND {field} LIKE '%{EscapeArrayLike(token)}%')");
                }
                else if (this.operatorType == OPERATOR_TYPE.ILIKE_ANY_ARRAY)
                {
                    return string.Format($"{field} ILIKE '%{EscapeArrayLike(token)}%'");
                }
                else
                {
                    return string.Format($"{field} LIKE '%{EscapeArrayLike(token)}%'");
                }
            }
            else  // it is OPERATOR_TYPE.EQUAL
            {
                string str = "";
                if (this.fieldType == FIELD_TYPE.STRING) { str = "'"; }
                return $"{field}={str}{token}{str}";
            }                        
        }

        private string EvaluateSpecialToken(string token)
        {
            // {r:} {regex:} {} regex search using Postgres regex operator ~ or ~* (case insensitive)
            
            // {s:} {search:} search using vector embeding similarity of the token to search and the stored vectors based on the following: file name, folder names, image tags, people tags, image description, image location
            
            // {l:} {loc:} location search using geospatial search {l:lat,lon,radiusInMeters}  e.g. {l:37.7749,-122.4194,5000} -> within 5km of San Francisco
            // 1. Geocoding API (Reverse Geocoding) - Get location name from coordinates:
            //     GET https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY 
            //     Returns city, country, address components for that exact location.
            // 2. Places API (Nearby Search) - Find places within radius:
            //     GET https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=40.714224,-73.961452&radius=5000&type=local
            //     radius is in meters (5000 = ~3.1 miles). Use type=locality for cities.
            
            // {d:} or {date:} -> {d=:} {d!=:} {d>:} {d>=:} {d<:} {d<=:} created date  {d=:2025/05/01} {d>:2025-05-01} {d<=:2025.05.01}  YYYY/MM/DD or YYYY-MM-DD or YYYY.MM.DD 
            
            // {ai:} {face:} face recognition search 

            token = token.Slice(1, -1);
            var parts = token.Split(":");            
            if (parts.Length == 2)
            {
                var (op, value) = (parts[0].ToLowerInvariant(), parts[1]);                
                if (op == "regex" || op == "r")
                {
                    return EvaluateRegex(value);
                }
                if (op == "search" || op == "s")
                {
                    return EvaluateSearch(value);
                }
                if (op == "loc" || op == "l")
                {
                    return EvaluateLocation(value);
                }
                if (op == "ai" || op == "face")
                {
                    return EvaluateFace(value);
                }
                if (op.StartsWith("d"))
                {
                    return EvaluateDate(op, value);
                }                
                //unknown special token, treat as regex
                return EvaluateRegex(value);                
            }
            return EvaluateRegex(token); //default to regex evaluation
        }

        private string EvaluateRegex(string pattern)
        {
            //Postgres case insesitive regex matching                 
            return string.Format($"({this.fields.Field} ~* '{pattern}')");   // image_path ~* 'regex'
        }
        private string EvaluateSearch(string searchTerm)
        {
            //TODO: placeholder for vector embedding search
            return ""; //string.Format($"(vector_search({field}, '{searchTerm}') = TRUE)");   
        }
        private string EvaluateLocation(string locationToken)
        {
            //TODO: placeholder for geospatial search
            // expected format: lat,lon,radiusInMeters
            var parts = locationToken.Split(",");
            if (parts.Length == 3)
            {
                var lat = parts[0];
                var lon = parts[1];
                var radius = parts[2];
                return string.Format($"(geo_within_radius({this.fields.LocationField}, {lat}, {lon}, {radius}) = TRUE)");   
            }
            //return "(1=0)"; //invalid location token
            throw new FormatException($"Invalid location token: {locationToken}");
        }
        private string EvaluateFace(string faceToken)
        {
            var faceField = this.fields.FaceField;
            return string.Format($"({faceField} @> Array['{EscapeArrayLike(faceToken.ToLower())}'])");   
        }
        private string EvaluateDate(string op, string dateToken)
        {
            var operators = new List<string> { "=", "!=", ">", ">=", "<", "<=" };
            op = op.StartsWith("date") ? op.Substring(4) : op.Substring(1);
            if (!operators.Contains(op))
            {
                //op = "="; //default to equal
                //return "(1=0)"; //invalid operator
                throw new FormatException($"Invalid operator: {op}, expected one of {string.Join(", ", operators)}");                
            }

            DateTime dt;
            if (DateTime.TryParse(dateToken, out dt))
            {
                //string dtField = "coalesce(exif.date_taken, vm.date_taken, ai.image_timestamp_utc)";
                var dtField = this.fields.DateField;
                return string.Format($"({dtField} {op} '{dt:yyyy-MM-dd}')");   
            } 

            //return "(1=0)"; //invalid date format
            throw new FormatException($"Invalid date format in date token: {dateToken}");
        }

    }

}
