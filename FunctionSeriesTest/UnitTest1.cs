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
            Assert.AreEqual(Interpreter.SolvePolishExpression(Interpreter.GetPolishExpression(test), new Dictionary<char, double> { { 'x', x } }), 2, 798102101837749);
        }
    }

    [TestClass]
    public class FourierSeriesTest
    {
        [TestMethod]
        public void FourierSerTest()
        {
            var test = new FourierSeries();
            Assert.AreEqual(test.ToString(), "Cos(1x) + Sin(1x)");
        }

        [TestMethod]
        public void FourierSer2Test()
        {
            var test = new FourierSeries(1, 1, "x + 1", FourierSeriesType.Sin);
            Assert.AreEqual(test.ToString(), "1,5914185290717897*Sin(6,283185307179586x)");
        }

        [TestMethod]
        public void FourierSer2Test2()
        {
            var test = new FourierSeries(2, 10, "x * 2", FourierSeriesType.Cos);
            Assert.AreEqual(test.ToString(), "5 - 4,05318069547683*Cos(0,6283185307179586x) - 6,394884621840902E-16*Cos(1,2566370614359172x)");
        }

        [TestMethod]
        public void FourierSer3Test()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.ToString(), new FourierSeries().ToString());
        }

        [TestMethod]
        public void FourierSer3Test2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.ToString(), "Cos(1x) + Sin(1x)");
        }

        [TestMethod]
        public void FourierSer3Test3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.ToString(), "5 + 15*Cos(2x) + 20*Sin(2x)");
        }

        [TestMethod]
        public void ComputeTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.Compute(0.1).ToString(), "1,094837581924854");
        }

        [TestMethod]
        public void ComputeTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.Compute(15).ToString(), "-0,10940007270170449");
        }

        [TestMethod]
        public void ComputeTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.Compute(1).ToString(), "16,9437459883065");
        }

        [TestMethod]
        public void IntegralTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.Integral("e x ^", -3.14, 3.14, 50000).ToString(), "23,06058409113614");
        }

        [TestMethod]
        public void IntegralTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.Integral("e", -3.14, 3.14, 50000).ToString(), "17,070809882730966");
        }

        [TestMethod]
        public void IntegralTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.Integral("e 2 ^", -3.14, 3.14, 100000).ToString(), "46,40327230121512");
        }

        [TestMethod]
        public void GetDerivativeTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            Assert.AreEqual(test.GetDerivative().ToString(), "Cos(1x) - Sin(1x)");
        }

        [TestMethod]
        public void GetDerivativeTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.GetDerivative().ToString(), "Cos(1x) - Sin(1x)");
        }

        [TestMethod]
        public void GetDerivativeTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.GetDerivative().ToString(), "40*Cos(2x) + -30*Sin(2x)");
        }

        [TestMethod]
        public void GetIntegralTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            Assert.AreEqual(test.GetIntegral().ToString(), "-Cos(1x) + Sin(1x)");
        }

        [TestMethod]
        public void GetIntegralTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.GetIntegral().ToString(), "-Cos(1x) + Sin(1x)");
        }

        [TestMethod]
        public void GetIntegralTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.GetIntegral().ToString(), "-10*Cos(2x) + 7,5*Sin(2x)");
        }

        [TestMethod]
        public void AddTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "5 + 16*Cos(1x) + 21*Sin(1x)");
        }

        [TestMethod]
        public void AddTest2()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "11 + 23*Cos(1x) + 28*Sin(1x)");
        }

        [TestMethod]
        public void AddTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "10 + 30*Cos(2x) + 40*Sin(2x)");
        }

        [TestMethod]
        public void SubTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test - test2;
            Assert.AreEqual(test3.ToString(), "-5 - 14*Cos(1x) - 19*Sin(1x)");
        }

        [TestMethod]
        public void SubTest2()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test - test2;
            Assert.AreEqual(test3.ToString(), "1 - 7*Cos(1x) - 12*Sin(1x)");
        }

        [TestMethod]
        public void SubTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test - test2;
            Assert.AreEqual(test3.ToString(), "");
        }

        [TestMethod]
        public void EqualTest()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            bool test3 = test == test2;
            Assert.AreEqual(test3, false);
        }

        [TestMethod]
        public void EqualTest2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            bool test3 = test == test2;
            Assert.AreEqual(test3, true);
        }

        [TestMethod]
        public void NonEqualTest()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            bool test3 = test != test2;
            Assert.AreEqual(test3, true);
        }

        [TestMethod]
        public void NonEqualTest2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            bool test3 = test != test2;
            Assert.AreEqual(test3, false);
        }

        [TestMethod]
        public void Mul1Test()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = 10 * test;
            Assert.AreEqual(test2.ToString(), "10*Cos(1x) + 10*Sin(1x)");
        }

        [TestMethod]
        public void Mul1Test2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = 3 * test;
            Assert.AreEqual(test2.ToString(), "15 + 45*Cos(2x) + 60*Sin(2x)");
        }

        [TestMethod]
        public void Mul2Test()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = test * 10;
            Assert.AreEqual(test2.ToString(), "10*Cos(1x) + 10*Sin(1x)");
        }

        [TestMethod]
        public void Mul2Test2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = test * 3;
            Assert.AreEqual(test2.ToString(), "15 + 45*Cos(2x) + 60*Sin(2x)");
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
            var TestPoint = (rand.NextDouble() - 0.5) * 2000;
            Assert.AreEqual(FSlow.Compute(TestPoint, 100), FFast.Compute(TestPoint, 100), 1E-6);
        }
    }
}