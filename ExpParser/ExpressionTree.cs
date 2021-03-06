using ExpParser.Exceptions;
using System;
using System.Collections.Generic;

namespace ExpParser
{
    /// <summary>
    /// an Expression tree is like a LISP Expression:
    /// 
    /// ex: (+ a (- b c)) == 
    ///     ExpressionTree(func_add, 
    ///                    ListOf ( Token(a), 
    ///                             ExpressionTree(func_substract, 
    ///                                            ListOf ( Token(b), 
    ///                                                     Token(c)
    ///                                                    ) 
    ///                                            )
    ///                           ) 
    ///                   )
    ///   - the trick of evaluating is intended to be implemented in the operator functions 
    ///     and in the Token.
    /// </summary>
    public class ExpressionTree: IEvaluableExpression
    {
        public IOperator op;
        public List<IEvaluableExpression> args;
        public IEvaluableExpression exp;

        public ExpressionTree(IOperator op, List<IEvaluableExpression> args) 
        {
            this.op = op;
            this.args = args;
        }
        public ExpressionTree(IOperator op, IEvaluableExpression exp)
        {
            this.op = op;
            this.exp = exp;
        }
        public virtual object Evaluate(object obj) 
        {
            if (args != null && exp != null) 
            {
                throw new EvaluationException("An expression must contain an operator and a list of IEvaluableExpression expression or just one IEvaluableExpression but not both");
            }

            if (this.args!= null && this.args.Count > 0) 
            {
                var res = this.op.Evaluate(obj, this.args);
                return res;
            }
            else 
            {
                var res = this.op.Evaluate(obj, this.exp);
                return res;
            }
        }
        
        public override string ToString()
        {
            if (args != null && exp != null)
            {
                throw new EvaluationException("An expression must contain an operator and a list of IEvaluableExpression expression or just one IEvaluableExpression but not both");
            }

            if (this.args != null && this.args.Count > 0)
            {
                return this.op.ToString(this.args);
            }
            else
            {
                return this.op.ToString(this.exp);
            }
        }
    }
}
