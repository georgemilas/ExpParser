using ExpParser.ObjectQuery;
using ExtParser.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ExpParser.Tests.Extensions
{
    [TestFixture]
    class CSVTester
    {
        
        [Test]
        public void TestToCsvLine()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var actual = CSV.ToCsvLine(lst);
            Assert.AreEqual("1,2,3,4,5,6,7", actual);

            actual = CSV.ToCsvLine(lst, true);
            Assert.AreEqual("1,2,3,4,5,6,7", actual);

            var lst2 = new List<string>() { "a", "b", "c" };
            actual = CSV.ToCsvLine(lst2, true);
            Assert.AreEqual("\"a\",\"b\",\"c\"", actual);
        }

        [Test]
        public void TestToCsv()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var csv = new List<List<int>> {lst, lst};
            var actual = CSV.ToCsv(csv);
            Assert.AreEqual("1,2,3,4,5,6,7\r\n1,2,3,4,5,6,7\r\n", actual);

            actual = CSV.ToCsv(csv, true);
            Assert.AreEqual("1,2,3,4,5,6,7\r\n1,2,3,4,5,6,7\r\n", actual);

            var lst2 = new List<string>() { "a", "b", "c" };
            var csv2 = new List<List<string>> { lst2, lst2 };
            actual = CSV.ToCsv(csv2, true);
            Assert.AreEqual("\"a\",\"b\",\"c\"\r\n\"a\",\"b\",\"c\"\r\n", actual);
        }

        [Test]
        public void TestFromCsvLine()
        {
            var lst = new List<string>() { "a", "b", "c" };
            var actual = CSV.FromCsvLine("\"a\",\"b\",\"c\"");
            CollectionAssert.AreEqual(lst, actual);
        }


       
    }
}