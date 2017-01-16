using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExtParser.Extensions
{
    public static class CSV
    {
        /*
        public static string ToCsvLine(FileInfo[] arr) { return ToCsvLine(arr, ", "); }
        public static string ToCsvLine(FileInfo[] arr, string separator)
        {
            return ToCsvLine(arr, separator, delegate(Object fo)
                { return ((FileInfo)fo).FullName; });  
        }
        */

        public delegate string GetStringDelegate(Object obj);

        /// <summary>
        /// returns a string in a CSV format for the list (columns) given 
        /// </summary>
        public static string ToCsvLine(IEnumerable e) { return ToCsvLine(e, ",", null, false); }
        public static string ToCsvLine(IEnumerable e, bool quoteString) { return ToCsvLine(e, ",", null, quoteString); }

        public static string ToCsvLine(IEnumerable e, string separator) { return ToCsvLine(e, separator, null, false); }
        public static string ToCsvLine(IEnumerable e, string separator, bool quoteStrings) { return ToCsvLine(e, separator, null, quoteStrings); }

        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string ToCsvLine(IEnumerable e, string separator, GetStringDelegate getStrDel) { return ToCsvLine(e, separator, getStrDel, false); }
        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string ToCsvLine(IEnumerable e, string separator, GetStringDelegate getStrDel, bool quoteStrings)
        {
            if (separator == null) { separator = ","; }
            StringBuilder bres = new StringBuilder("");
            foreach (Object s in e)
            {
                string ss;
                if (getStrDel != null) 
                { 
                    ss = getStrDel(s); 
                }
                else 
                {
                    if (s is string && quoteStrings) 
                    {
                        string mys = (string)s;
                        if (mys.Contains("\"" + separator))
                        {
                            mys = mys.Replace("\"" + separator, "\" " + separator);
                        }
                        ss = "\"" + mys + "\"";
                    }
                    else 
                    {
                        ss =  s != null ? s.ToString() : ""; 
                    }
                }
                bres.Append(ss + separator);
            }
            string res = bres.ToString();
            if (res.EndsWith(separator))  //remove trailing separator
            {
                res = res.Substring(0, res.Length - separator.Length);
            }
            return res;
        }

        public static string ToCsvLine(Object e, GetStringDelegate getStrDel)
        {
            return getStrDel(e);
        }

        
        
        /// <summary>
        /// returns a string in a CSV format for the list (rows) of list (columns)
        /// </summary>
        public static string ToCsv(IEnumerable<IEnumerable> e) { return ToCsv(e, ",", null, false); }
        public static string ToCsv(IEnumerable<IEnumerable> e, bool quoteString) { return ToCsv(e, ",", null, quoteString); }

        public static string ToCsv(IEnumerable<IEnumerable> e, string separator) { return ToCsv(e, separator, null, false); }
        public static string ToCsv(IEnumerable<IEnumerable> e, string separator, bool quoteStrings) { return ToCsv(e, separator, null, quoteStrings); }

        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list of lists otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string ToCsv(IEnumerable<IEnumerable> e, string separator, GetStringDelegate getStrDel) { return ToCsv(e, separator, getStrDel, false); }
        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list of lists otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string ToCsv(IEnumerable<IEnumerable> e, string separator, GetStringDelegate getStrDel, bool quoteStrings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IEnumerable le in e)
            {
                sb.AppendLine(CSV.ToCsvLine(le, separator, getStrDel, quoteStrings));
            }
            return sb.ToString();

            
        }

        public static string ToCsv(IEnumerable e, GetStringDelegate getStrDel)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Object le in e)
            {
                sb.AppendLine(CSV.ToCsvLine(le, getStrDel));
            }
            return sb.ToString();
        }

        public static string CalendarAppointmentstoCsv(IEnumerable e, GetStringDelegate getStrDel)
        {
            StringBuilder sb = new StringBuilder();
            //add the standard calender header line
            sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\"","Subject","Start Date","Start Time","End Date","End Time","All day event","Reminder on/off","Reminder Date","Reminder Time","Meeting Organizer","Required Attendees","Optional Attendees","Meeting Resources","Billing Information","Categories","Description","Location","Mileage","Priority","Private","Sensitivity","Show time as"));
            foreach (Object le in e)
            {
                sb.AppendLine(CSV.ToCsvLine(le, getStrDel));
            }
            return sb.ToString();
        }


        public static List<string> FromCsvLine(string line)
        {
            return FromCsvLine(line, new List<char> { ',', '\t' }, '"');
        }
        public static List<string> FromCsvLine(string line, List<char> separators, char? quote)
        {
            var sep = new Dictionary<char, int>();
            foreach (var s in separators) { sep[s] = 0; }

            //string[] lline = line.Trim().Split(new char[] { ',' });
            List<string> res = new List<string>();

            string itm = null;
            bool inItemValue = false;   //consider quoted items
            int cnt = -1;
            foreach(char c in line)
            {
                cnt += 1;
                if (sep.ContainsKey(c))  //a separator (',' '\t')
                {
                    if (!inItemValue)
                    {
                        res.Add(itm);
                        itm = null;
                    }
                    else
                    {
                        itm = itm == null ? c.ToString() : itm + c.ToString();    //separator (comma, tab) is part of itm
                    }
                }
                else if (quote != null && c == quote)  // '"'
                {
                    if (inItemValue)
                    {
                        //only finish itm if next char is a separator (comma, tab) or EOL otherwise " is part of itm
                        if (cnt == line.Length - 1 || sep.ContainsKey(line[cnt + 1]))
                        {
                            inItemValue = false;
                        }
                        else
                        {
                            itm = itm == null ? c.ToString() : itm + c.ToString();
                        }
                    }
                    else
                    {
                        inItemValue = true;  //not already in a value so start a value
                    }
                }
                else if (c == '\0')
                {
                    //ignore
                }
                else
                {
                    itm = itm == null ? c.ToString() : itm + c.ToString();  
                }                
            }
            if (itm != null)
            {
                res.Add(itm);
            }
            return res;
        }

        /// <summary>
        /// then return a collection indexable using integers indexes
        /// - see also ParseWithHeader
        /// </summary>
        public static List<List<string>> Parse(string filePath) { return Parse(File.OpenText(filePath)); }
        public static List<List<string>> Parse(StreamReader sr) { return Parse(sr, line => line, new List<char> { ',', '\t' }, '"'); }

        /// <summary>
        /// then return a collection indexable using integers indexes
        /// - see also ParseWithHeader
        /// </summary>
        public static List<T> Parse<T>(string filePath, Func<List<string>, T> objMaker) { return Parse<T>(File.OpenText(filePath), objMaker); }
        public static List<T> Parse<T>(StreamReader sr, Func<List<string>, T> objMaker) { return Parse(sr, objMaker, new List<char> { ',', '\t' }, '"'); }

        public static List<T> Parse<T>(string filePath, Func<List<string>, T> objMaker, List<char> separators, char? quote) { return Parse<T>(File.OpenText(filePath), objMaker, separators, quote); }
        public static List<T> Parse<T>(StreamReader sr, Func<List<string>, T> objMaker, List<char> separators, char? quote)
        {
            List<T> res = new List<T>();
            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() != "")
                    {
                        res.Add(objMaker(CSV.FromCsvLine(line, separators, quote)));  //let errors propagate
                    }
                }
            }
            finally
            {
                sr.Close();
            }
            return res;
        }

        public static List<Dictionary<string, string>> ParseWithHeader(List<List<string>> parsedCSV)
        {
            return ParseWithHeader<Dictionary<string, string>>(parsedCSV, (line) =>
            {
                var r = new Dictionary<string, string>();
                for (int i = 0; i < line.Count; i++)
                {
                    r[parsedCSV[0][i]] = line[i];
                }
                return r;
            });
        }
        /// <summary>
        /// using a parsed collection and considering the first line in the file to be a header line
        /// then return a collection indexable using name instead on just integers indexes
        ///   ex:    csv[1]["invoice"], csv[2]["invoice"] instead of
        ///          csv[1][0], csv[2][0]    
        /// </summary>
        public static List<T> ParseWithHeader<T>(List<List<string>> parsedCSV, Func<List<string>, T> objMaker) 
        {
            List<T> res = new List<T>();

            if (parsedCSV.Count < 1)
            {
                return new List<T>();
            }

            if (parsedCSV.Count == 1)
            {
                List<string> nulls = new List<string>();
                for (int i = 0; i < parsedCSV[0].Count; i++)
                {
                    nulls.Add(null);
                }
                T d = objMaker(nulls);  //return an object with all values as null
                res.Add(d);
                
            }
            else
            {
                for (int i = 1; i < parsedCSV.Count; i++)
                {
                    T d = objMaker(parsedCSV[i]);
                    res.Add(d);
                }
            }

            return res;
        }


    }

}
