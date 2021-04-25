using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyPaintTest
{
    [TestClass]
    public class UnitTestExample1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string s1 = "test";
            string s2 = "test";
            Assert.AreEqual(s1, s2);
        }
    }
}
