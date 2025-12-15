using ExpParser.ObjectQuery;
using ExtParser.Extensions;
using Xunit;
using System;
using System.Collections.Generic;

namespace ExpParser.Tests.Extensions
{
    public class CSVTester
    {
        
        [Fact]
        public void TestToCsvLine()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var actual = CSV.ToCsvLine(lst);
            Assert.Equal("1,2,3,4,5,6,7,8", actual);

            actual = CSV.ToCsvLine(lst, true);
            Assert.Equal("1,2,3,4,5,6,7,8", actual);

            var lst2 = new List<string>() { "a", "b", "c" };
            actual = CSV.ToCsvLine(lst2, true);
            Assert.Equal("\"a\",\"b\",\"c\"", actual);
        }

        [Fact]
        public void TestToCsv()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var csv = new List<List<int>> {lst, lst};
            var actual = CSV.ToCsv(csv);
            Assert.Equal("1,2,3,4,5,6,7\r\n1,2,3,4,5,6,7\r\n", actual);

            actual = CSV.ToCsv(csv, true);
            Assert.Equal("1,2,3,4,5,6,7\r\n1,2,3,4,5,6,7\r\n", actual);

            var lst2 = new List<string>() { "a", "b", "c" };
            var csv2 = new List<List<string>> { lst2, lst2 };
            actual = CSV.ToCsv(csv2, true);
            Assert.Equal("\"a\",\"b\",\"c\"\r\n\"a\",\"b\",\"c\"\r\n", actual);
        }

        [Fact]
        public void TestFromCsvLine()
        {
            var lst = new List<string>() { "a", "b", "c" };
            var actual = CSV.FromCsvLine("\"a\",\"b\",\"c\"");
            Assert.Equal(lst, actual);
        }


       
    }
}