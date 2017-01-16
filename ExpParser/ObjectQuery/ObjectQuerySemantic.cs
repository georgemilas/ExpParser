using System;
using ExpParser.BooleanLogic;

namespace ExpParser.ObjectQuery
{
    public abstract class ObjectQuerySemantic : BooleanLogicSemantic, IObjectQuerySemantic
    {
        public IOperator EQ { get; set; }
        public IOperator NE { get; set; }
        public IOperator LT { get; set; }
        public IOperator GT { get; set; }
        public IOperator LE { get; set; }
        public IOperator GE { get; set; }
    }
}