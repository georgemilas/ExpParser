using System;
using System.Collections.Generic;
using System.Text;

namespace ExpParser.keywords
{
    public interface IKeywordsSemantic: ISemantic
    {
        IOperator AND {get; set;}
        IOperator OR { get; set;}
        IOperator NOT { get; set;}        
    }
}
