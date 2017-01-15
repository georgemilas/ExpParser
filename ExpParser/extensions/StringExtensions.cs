using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ExtParser.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// see ListExtensions Slice (python like slice)
        /// </summary>
        public static string Slice(this string txt, int idxFrom)
        {
            List<char> lst = new List<char>(txt.ToCharArray());
            List<char> res = lst.Slice(idxFrom);
            return res.Join("");
        }

        /// <summary>
        /// see ListExtensions Slice (python like slice)
        /// </summary>
        public static string Slice(this string txt, int? idxFrom, int? idxTo)
        {
            List<char> lst = new List<char>(txt.ToCharArray());
            List<char> res = lst.Slice(idxFrom, idxTo);
            return res.Join("");
        }

        /// <summary>
        /// txt.Split and remove empty entries
        /// </summary>
        public static List<string> SplitClean(this string txt, string splitter)
        {
            return new List<string>(txt.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries));
        }

    }
}
