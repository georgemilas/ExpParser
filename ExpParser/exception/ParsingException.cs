using System;
using System.Collections.Generic;

namespace ExpParser.Exceptions
{
    public class ParsingException : Exception, IComparable
    {
        public ParsingException(string msg)
            : base(msg)
        { }
        public ParsingException(string msg, Exception e)
            : base(msg, e)
        { }

        public virtual int CompareTo(object other)
        {
            return Object.ReferenceEquals(this, other) ? 0 : 1;
        }
    }
}