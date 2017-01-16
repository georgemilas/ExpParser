using System;

namespace ExpParser.Arithmetic
{
    public interface IArithmeticSemantic : ISemantic
    {
        IOperator PLUS { get; set; }     // +
        IOperator MINUS { get; set;}   // -
        IOperator MUL { get; set;}     // *
        IOperator DIV { get; set; }    // /
        IOperator POW { get; set; }    // ^     
    }

}



