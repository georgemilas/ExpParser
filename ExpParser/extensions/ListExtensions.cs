using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtParser.Extensions
{
    
    public static class ListExtensions
    {
        
        /// <summary>
        /// - returns a representation something like: (itm1, itm2, itm3)  
        ///     with the parentheses and comma and space after comma
        /// - use ToString(false) to return without parentheses: itm1, itm2, itm3
        /// - for more flexibility use CSV.ToCsvLine(this)
        /// </summary>
        public static string ToString<T>(this IEnumerable<T> lst, bool paranteses, string separator = ",")
        {
            string res = CSV.ToCsvLine(lst, separator);

            if (paranteses)
            {
                res = "(" + res + ")";
            }
            return res;
        }

        public static List<T> Intersection<T>(this IEnumerable<T> lst, IEnumerable<T> lst2)
        {
            List<T> res = new List<T>();
            
            foreach (T itm in lst2)
            {
                if (lst.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

        /// <summary>
        /// - return what in this and not in that AND ALSO what in that and not in this
        /// - this.Union(other).Left_Diference(this.Intersection(other))
        /// </summary>
        public static List<T> Difference<T>(this IEnumerable<T> lst, IEnumerable<T> lst2)
        {
            List<T> inters = lst.Intersection(lst2);
            List<T> uni = lst.Union(lst2);
            return uni.Left_Difference(inters);
        }
        /// <summary>
        /// return what in this and not in that    (but not what in that and not in this)
        /// </summary>
        public static List<T> Left_Difference<T>(this IEnumerable<T> lst, List<T> lst2)
        {
            List<T> res = new List<T>();
            foreach (T itm in lst)
            {
                if (!lst2.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

        public static List<T> Union<T>(this IEnumerable<T> lst, IEnumerable<T> lst2)
        {
        
            List<T> res = new List<T>(lst);

            foreach (T itm in lst2)
            {
                if (!res.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
            
        }

        public delegate T ReduceAction<T>(T t1, T acum);
        public static T Reduce<T>(this IEnumerable<T> lst, ReduceAction<T> reducer)
        {
            return Reduce(lst, reducer, default(T));
        }
        public static T Reduce<T>(this IEnumerable<T> lst, ReduceAction<T> reducer, T acum)
        {
            return lst.Aggregate(acum, (a, el) => 
            {
                var res = reducer(el, a);
                return res;
            });                        
        }

        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> lst, Func<T, U> mapper)
        {
            return lst.Select(mapper);
        }
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> lst, Func<T, bool> filter)
        {
            return lst.Where(filter);
        }


        /// <summary>
        /// - Split the elements of the list in lists of #nr elements
        /// - [1,2,3,4,5,6,7,8,9,10].Unzip(3) -> [(1,2,3),(4,5,6),(7,8,9), (10,)]
        /// </summary>
        public static List<List<T>> Unzip<T>(this IEnumerable<T> lst, int nr)
        {
            List<T> res = new List<T>(); 
            List<List<T>> mainlst = new List<List<T>>();
            foreach(T itm in lst)
            {
                if (res.Count < nr) 
                {
                    res.Add(itm);
                }
                else 
                {
                    mainlst.Add(res);
                    res = new List<T>();
                    res.Add(itm);
                }
            }
            if (res.Count > 0)
            {
                mainlst.Add(res);
            }
            return mainlst;
        }

        /// <summary>
        /// Evenly split the elements in #nr lists
        /// - [1,2,3,4,5,6,7,8,9,10].Split(3) -> [(1,2,3),(4,5,6),(7,8,9,10)]
        /// </summary>
        public static List<List<T>> Split<T>(this IEnumerable<T> lst, int nr)
        {
            int lists = lst.Count() / nr;
            List<T> res = new List<T>();
            List<List<T>> mainlst = new List<List<T>>();
            foreach (T itm in lst)
            {
                if ( res.Count < lists || (res.Count >= lists && (mainlst.Count >= lists-1)))
                {
                    res.Add(itm);
                }
                else
                {
                    mainlst.Add(res);
                    res = new List<T>();
                    res.Add(itm);
                }
            }
            if (res.Count > 0)
            {
                mainlst.Add(res);
            }
            return mainlst;
        }

        /// <summary>
        /// [1,2,3,4,5].join(" | ") -> "1 | 2 | 3 | 4 | 5"
        /// </summary>
        public static string Join<T>(this IList<T> lst, string separator)
        {
            StringBuilder res = new StringBuilder();
            for (int i=0; i<lst.Count;i++)
            {
                if (i != lst.Count - 1) res.Append(lst[i].ToString() + separator);
                else res.Append(lst[i].ToString());
            }
            return res.ToString();
        }

        /// <summary>
        /// use the list like a stack this.Add & this.pop
        /// </summary>
        public static T Pop<T>(this IList<T> lst)
        {
            if (lst.Count() > 0)
            {
                T itm = lst[lst.Count - 1];
                lst.Remove(itm);
                return itm;
            }
            else
            {
                throw new IndexOutOfRangeException("The list is empty");
            }
        }


        /// <summary>
        /// - see slice(ixFrom, null)
        /// </summary>
        public static List<T> SliceEx<T>(this IEnumerable<T> lst, int idxFrom) 
        {
            return SliceEx(lst, idxFrom, null);
        }
        
        /// <summary>
        /// - like the slice operator in python
        /// - lst = [1,2,3,4,5,6,7]
        ///     lst.Slice(3, null) => [4,5,6,7]
        ///     lst.Slice(-3, null) => [5,6,7]
        ///     lst.Slice(20, null) => []
        ///     lst.Slice(-20, null) => [1,2,3,4,5,6,7]
        ///  
        ///     lst.Slice(null, 3) => [1,2,3]
        ///     lst.Slice(null, -3) => [1,2,3,4]
        ///     lst.Slice(null, 20) => [1,2,3,4,5,6,7]
        ///     lst.Slice(null, -20) => []
        /// 
        ///     lst.Slice(3, 4) => [4]
        ///     lst.Slice(-3, -1) => [5,6]
        ///     lst.Slice(-3, -4) => []
        ///     lst.Slice(3, 20) => [4,5,6,7]
        /// </summary>
        public static List<T> SliceEx<T>(this IEnumerable<T> lst, int? idxFrom, int? idxTo)
        {
            var lcnt = lst.Count();

            if (idxFrom == null) { idxFrom = 0; }
            if (idxTo == null) { idxTo = lcnt; }
            
            if (idxFrom < 0)
            {
                idxFrom = lcnt + idxFrom;
                if (idxFrom < 0) idxFrom = 0;
            }
            if (idxTo < 0)
            {
                idxTo = lcnt + idxTo;
                if (idxTo < 0) idxTo = 0;
            }

            int cnt = (int)idxTo;
            if (cnt > lcnt) cnt = lcnt;
            cnt = cnt - (int)idxFrom;
            if (cnt < 0) cnt = 0;                
            
            if ((int)idxFrom >= lcnt) return new List<T>();

            return lst.ToList().GetRange((int)idxFrom, cnt);    //all to the end
        }
        

    }

}
