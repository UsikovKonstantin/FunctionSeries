using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FunctionSeriesClassLibrary;
using System.Collections.Generic;
using System.Diagnostics;

namespace FunctionSeriesTest
{
    [TestClass]
    public class InterpreterTest
    {
        public string test = "";
        public double x = 0;

        [TestMethod]
        public void GetPolishExpressionTest()
        {
            test = "(1 + 2) + (sin(20) * 12 - 11) / 20";
            Assert.AreEqual(Interpreter.GetPolishExpression(test), "1 2 + 20 sin 12 * 11 - 20 / + ");
        }
        [TestMethod]
        public void GetPolishExpressionTest2()
        {
            test = "312 * (15 / cos(130) + 12 * tan(158)) - 841 + 11 * 10 - ln(115) / sqrt(4)";
            Assert.AreEqual(Interpreter.GetPolishExpression(test), "312 15 130 cos / 12 158 tan * + * 841 - 11 10 * + 115 ln 4 sqrt / - ");
        }
        [TestMethod]
        public void SolvePolishExpressionTest()
        {
            x = 2;
            test = "12*x^2 + 13*x + 5";
            Assert.AreEqual(Interpreter.SolvePolishExpression(Interpreter.GetPolishExpression(test), new Dictionary<char, double> { { 'x', x } }), 79);
        }
        [TestMethod]
        public void SolvePolishExpressionTest2()
        {
            x = 1;
            test = "(18*sin(2*x)) + 11*x^3 / (11 * cos(3*x)) + 13*x";
            Assert.AreEqual(Interpreter.SolvePolishExpression(Interpreter.GetPolishExpression(test), new Dictionary<char, double> { { 'x', x } }), 2,798102101837749);
        }
    }

    [TestClass]
    public class FourierApproxTest
    {
        [TestMethod]
        public void FourierAppTest()
        {
            
        }
    }

    [TestClass]
    public class AproxximationTest
    {
        [TestMethod]
        public void TestFastVsSlow()
        {
            List<(double x, double y)> point_cloud = new();
            Random rand = new Random();
            for (int i = 0; i < 1024; i++)
            {
                point_cloud.Add(((rand.NextDouble() - 0.5) * 2000, (rand.NextDouble() - 0.5) * 2000));
            }
            Stopwatch sw = Stopwatch.StartNew();
            var FSlow = new FourierApprox(point_cloud, FourierApprox.transform_type.slow);
            Console.WriteLine($"slow:{sw.ElapsedTicks}");
            sw.Restart();
            var FFast = new FourierApprox(point_cloud, FourierApprox.transform_type.fast);
            Console.WriteLine($"fast:{sw.ElapsedTicks}");
        }
    }
}