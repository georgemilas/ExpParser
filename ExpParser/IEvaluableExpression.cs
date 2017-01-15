using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser
{
    public interface IEvaluableExpression
    {
        /// <summary>
        /// Evaluate the current Expression tree against the given text
        /// </summary>
        object Evaluate(object obj);   
    }
}
