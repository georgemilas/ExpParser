using System;
using System.Collections.Generic;

namespace ExpParser.ObjectQuery
{
    public class ComparerOperatorExpression : ExpressionTree
    {
        public ComparerOperatorExpression(IOperator op, PropertyToken p, LiteralToken l): base(op, new List<IEvaluableExpression>() { p, l }) { }
        public override object Evaluate(object obj)
        {
            ObjectComparerOperator co = (ObjectComparerOperator)op;
            LiteralToken lt = (LiteralToken) this.args[1];
            return lt.Compare(obj, co);
        }
    }
}