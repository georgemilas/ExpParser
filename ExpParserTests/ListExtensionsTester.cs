using ExpParser.ObjectQuery;
using ExtParser.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ExpParser.Tests.Extensions
{
    [TestFixture]
    class ListExtensionsTester
    {
        
        [Test]
        public void TestSlice()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            CollectionAssert.AreEqual(new List<int>() { 4, 5, 6, 7 }, lst.Slice(3, null)); // => [4,5,6,7]
            CollectionAssert.AreEqual(new List<int>() { 5, 6, 7 }, lst.Slice(-3, null)); // => [5,6,7]
            CollectionAssert.AreEqual(new List<int>() { }, lst.Slice(20, null));// => []
            CollectionAssert.AreEqual(lst, lst.Slice(-20, null));// => [1,2,3,4,5,6,7]
            CollectionAssert.AreEqual(new List<int>() { 1,2,3 }, lst.Slice(null, 3));// => [1,2,3]
            CollectionAssert.AreEqual(new List<int>() { 1,2,3,4 }, lst.Slice(null, -3));// => [1,2,3,4]
            CollectionAssert.AreEqual(lst, lst.Slice(null, 20));// => [1,2,3,4,5,6,7]
            CollectionAssert.AreEqual(new List<int>() { }, lst.Slice(null, -20));// => []
            CollectionAssert.AreEqual(new List<int>() { 4 }, lst.Slice(3, 4));// => [4]
            CollectionAssert.AreEqual(new List<int>() { 5, 6 }, lst.Slice(-3, -1));// => [5,6]
            CollectionAssert.AreEqual(new List<int>() { }, lst.Slice(-3, -4));// => []
            CollectionAssert.AreEqual(new List<int>() { 4, 5, 6, 7 }, lst.Slice(3, 20));// => [4,5,6,7]                             
        }

        [Test]
        public void TestJoin()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5 };
            Assert.AreEqual("1 | 2 | 3 | 4 | 5", lst.Join(" | "));
        }


        [Test]
        public void TestUnzip()
        {
             //[1,2,3,4,5,6,7,8,9,10].Split(3) -> [(1,2,3),(4,5,6),(7,8,9),(10)]
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 , 10 };
            var actual = lst.Unzip(3);
            CollectionAssert.AreEqual(new List<List<int>>()
            {
                new List<int>() {1,2,3 },
                new List<int>() {4,5,6 },
                new List<int>() {7,8,9 },
                new List<int>() { 10 },
            }, actual);
        }

        [Test]
        public void TestSplit()
        {
            //[1,2,3,4,5,6,7,8,9,10].Split(3) -> [(1,2,3),(4,5,6),(7,8,9,10)]
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var actual = lst.Split(3);
            CollectionAssert.AreEqual(new List<List<int>>()
            {
                new List<int>() {1,2,3 },
                new List<int>() {4,5,6 },
                new List<int>() {7,8,9,10 },                
            }, actual);
        }

        [Test]
        public void TestUnion()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Union(lst2);
            CollectionAssert.AreEqual(new List<int>() { 1, 1, 2, 3, 4, 5, 0, 6, 7 }, actual);
        }

        [Test]
        public void TestLeftDiference()
        {
            var lst = new List<int>() {  1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() {0, 1, 2, 3, 6, 7 };
            var actual = lst.Left_Difference(lst2);
            CollectionAssert.AreEqual(new List<int>() { 4, 5 }, actual);
        }

        [Test]
        public void TestDiference()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Difference(lst2);
            CollectionAssert.AreEqual(new List<int>() { 4, 5, 0, 6, 7 }, actual);
        }

        [Test]
        public void TestIntersection()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Intersection(lst2);
            CollectionAssert.AreEqual(new List<int>() { 1, 2, 3 }, actual);
        }
    }
}