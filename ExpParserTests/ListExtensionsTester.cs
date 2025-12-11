using ExpParser.ObjectQuery;
using ExtParser.Extensions;
using Xunit;
using System;
using System.Collections.Generic;

namespace ExpParser.Tests.Extensions
{
    public class ListExtensionsTester
    {
        
        [Fact]
        public void TestSlice()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            Assert.Equal(new List<int>() { 4, 5, 6, 7 }, lst.SliceEx(3, null)); // => [4,5,6,7]
            Assert.Equal(new List<int>() { 5, 6, 7 }, lst.SliceEx(-3, null)); // => [5,6,7]
            Assert.Equal(new List<int>() { }, lst.SliceEx(20, null));// => []
            Assert.Equal(lst, lst.SliceEx(-20, null));// => [1,2,3,4,5,6,7]
            Assert.Equal(new List<int>() { 1,2,3 }, lst.SliceEx(null, 3));// => [1,2,3]
            Assert.Equal(new List<int>() { 1,2,3,4 }, lst.SliceEx(null, -3));// => [1,2,3,4]
            Assert.Equal(lst, lst.SliceEx(null, 20));// => [1,2,3,4,5,6,7]
            Assert.Equal(new List<int>() { }, lst.SliceEx(null, -20));// => []
            Assert.Equal(new List<int>() { 4 }, lst.SliceEx(3, 4));// => [4]
            Assert.Equal(new List<int>() { 5, 6 }, lst.SliceEx(-3, -1));// => [5,6]
            Assert.Equal(new List<int>() { }, lst.SliceEx(-3, -4));// => []
            Assert.Equal(new List<int>() { 4, 5, 6, 7 }, lst.SliceEx(3, 20));// => [4,5,6,7]                             
        }

        [Fact]
        public void TestJoin()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5 };
            Assert.Equal("1 | 2 | 3 | 4 | 5", lst.Join(" | "));
        }


        [Fact]
        public void TestUnzip()
        {
             //[1,2,3,4,5,6,7,8,9,10].Split(3) -> [(1,2,3),(4,5,6),(7,8,9),(10)]
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 , 10 };
            var actual = lst.Unzip(3);
            Assert.Equal(new List<List<int>>()
            {
                new List<int>() {1,2,3 },
                new List<int>() {4,5,6 },
                new List<int>() {7,8,9 },
                new List<int>() { 10 },
            }, actual);
        }

        [Fact]
        public void TestSplit()
        {
            //[1,2,3,4,5,6,7,8,9,10].Split(3) -> [(1,2,3),(4,5,6),(7,8,9,10)]
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var actual = lst.Split(3);
            Assert.Equal(new List<List<int>>()
            {
                new List<int>() {1,2,3 },
                new List<int>() {4,5,6 },
                new List<int>() {7,8,9,10 },                
            }, actual);
        }

        [Fact]
        public void TestUnion()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Union(lst2);
            Assert.Equal(new List<int>() { 1, 1, 2, 3, 4, 5, 0, 6, 7 }, actual);
        }

        [Fact]
        public void TestLeftDiference()
        {
            var lst = new List<int>() {  1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() {0, 1, 2, 3, 6, 7 };
            var actual = lst.Left_Difference(lst2);
            Assert.Equal(new List<int>() { 4, 5 }, actual);
        }

        [Fact]
        public void TestDiference()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Difference(lst2);
            Assert.Equal(new List<int>() { 4, 5, 0, 6, 7 }, actual);
        }

        [Fact]
        public void TestIntersection()
        {
            var lst = new List<int>() { 1, 1, 2, 3, 4, 5 };
            var lst2 = new List<int>() { 0, 1, 2, 3, 6, 7 };
            var actual = lst.Intersection(lst2);
            Assert.Equal(new List<int>() { 1, 2, 3 }, actual);
        }
    }
}