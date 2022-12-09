using Microsoft.VisualStudio.TestTools.UnitTesting;
using FunctionSeriesClassLibrary;
using System.Collections.Generic;

namespace FunctionSeriesTest
{
    [TestClass]
    public class InterpreterTest
    {
        public string test = "";
        public string output = "";
        public List<char> testlist = new List<char>();
        public List<char> testlist2;

        [TestMethod]
        public void GetPolishExpressionTest()
        {
            test = "(1 + 2) + (sin(20) * 12 - 11) / 20";
            output = Interpreter.GetPolishExpression(test);
            Assert.AreEqual(output, "1 2 + 20 sin 12 * 11 - 20 / + ");
        }
        [TestMethod]
        public void GetPolishExpressionTest2()
        {
            test = "312 * (15 / cos(130) + 12 * tan(158)) - 841 + 11 * 10 - ln(115) / sqrt(4)";
            output = Interpreter.GetPolishExpression(test);
            Assert.AreEqual(output, "312 15 130 cos / 12 158 tan * + * 841 - 11 10 * 115 ln 4 sqrt / - + ");
        }
        [TestMethod]
        public void IsVarInPolExpressionTest()
        {
            test = "x + y + z";
            testlist = Interpreter.GetVarsFromExpression(test);
            testlist2 = new List<char>() { 'x', 'y', 'z' };
            CollectionAssert.AreEqual(testlist, testlist2);
        }
        [TestMethod]
        public void IsVarInPolExpressionTest2()
        {
            test = "6 * x + 17 * y - 18 * z / 32 * a";
            testlist = Interpreter.GetVarsFromExpression(test);
            testlist2 = new List<char>() { 'x', 'y', 'z', 'a' };
            CollectionAssert.AreEqual(testlist, testlist2);
        }
        [TestMethod]
        public void GetVarsFromExpressionTest()
        {
            test = "x + y + z";
            testlist = Interpreter.GetVarsFromExpression(test);
            testlist2 = new List<char>() { 'x', 'y', 'z'};
            CollectionAssert.AreEqual(testlist, testlist2);
        }
        [TestMethod]
        public void GetVarsFromExpressionTest2()
        {
            test = "6 * x + 17 * y - 18 * z / 32 * a";
            testlist = Interpreter.GetVarsFromExpression(test);
            testlist2 = new List<char>() { 'x', 'y', 'z', 'a' };
            CollectionAssert.AreEqual(testlist, testlist2);
        }
    }

    [TestClass]
    public class FunctionSeriesTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}