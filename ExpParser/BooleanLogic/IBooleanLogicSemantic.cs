using System;
using System.Collections.Generic;

namespace ExpParser.BooleanLogic
{
    public interface IBooleanLogicSemantic: ISemantic
    {
        IOperator AND {get; set;}
        IOperator OR { get; set;}
        IOperator NOT { get; set;}        
    }
}
