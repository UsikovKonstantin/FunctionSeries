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
            x = 3;
            test = "18*sin(2*x) + 11*x^3 / 11 * cos(3*x) + 13*x";
            Assert.AreEqual(Math.Round(Interpreter.SolvePolishExpression(Interpreter.GetPolishExpression(test), new Dictionary<char, double> { { 'x', x } }), 3), 9.37);
        }
    }
    [TestClass]
    public class FourierSeriesTest
    {
        [TestMethod]
        public void FourierSerTest()
        {
            var test = new FourierSeries();
            Assert.AreEqual(test.ToString(), "cos(x) + sin(x)");
        }

        [TestMethod]
        public void FourierSer2Test()
        {
            var test = new FourierSeries(4, 3, "x + 1", FourierSeriesType.Sin);
            double[] n = new double[5] { 0, 2.228, -0.477, 0.743, -0.239 };
            for (int i = 0; i <= test.N; i++)
            {
                Assert.AreEqual(Math.Round(test.A[i], 3), 0);
                Assert.AreEqual(Math.Round(test.B[i], 3), n[i]);
            }
        }

        [TestMethod]
        public void FourierSer2Test2()
        {
            var test = new FourierSeries(2, 10, "x * 2", FourierSeriesType.Cos);
            double[] n = new double[3] { 10, -4.053, 0 };
            for (int i = 0; i <= test.N; i++)
            {
                Assert.AreEqual(Math.Round(test.A[i], 3), n[i]);
                Assert.AreEqual(Math.Round(test.B[i], 3), 0);
            }
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
            Assert.AreEqual(test.ToString(), "cos(x) + sin(x)");
        }

        [TestMethod]
        public void FourierSer3Test3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.ToString(), "5 + 15*cos(2*x) + 20*sin(2*x)");
        }

        [TestMethod]
        public void ComputeTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(Math.Round(test.Compute(0.1), 3), 1.095);
        }

        [TestMethod]
        public void ComputeTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(Math.Round(test.Compute(15), 3), -0.109);
        }

        [TestMethod]
        public void ComputeTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(Math.Round(test.Compute(1), 3), 16.944);
        }

        [TestMethod]
        public void IntegralTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(Math.Round(test.Integral("e x ^", -3.14, 3.14, 50000), 3), 23.061);
        }

        [TestMethod]
        public void IntegralTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(Math.Round(test.Integral("e", -3.14, 3.14, 50000), 3), 17.071);
        }

        [TestMethod]
        public void IntegralTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(Math.Round(test.Integral("e 2 ^", -3.14, 3.14, 100000), 3), 46.403);
        }

        [TestMethod]
        public void GetDerivativeTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            Assert.AreEqual(test.GetDerivative().ToString(), "cos(x) - sin(x)");
        }

        [TestMethod]
        public void GetDerivativeTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.GetDerivative().ToString(), "cos(x) - sin(x)");
        }

        [TestMethod]
        public void GetDerivativeTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.GetDerivative().ToString(), "40*cos(2*x) + -30*sin(2*x)");
        }

        [TestMethod]
        public void GetIntegralTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            Assert.AreEqual(test.GetIntegral().ToString(), "-cos(x) + sin(x)");
        }

        [TestMethod]
        public void GetIntegralTest2()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.CosSin);
            Assert.AreEqual(test.GetIntegral().ToString(), "-cos(x) + sin(x)");
        }

        [TestMethod]
        public void GetIntegralTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            Assert.AreEqual(test.GetIntegral().ToString(), "-10*cos(2*x) + 7,5*sin(2*x)");
        }

        [TestMethod]
        public void AddTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "5 + 16*cos(x) + 21*sin(x)");
        }

        [TestMethod]
        public void AddTest2()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "11 + 23*cos(x) + 28*sin(x)");
        }

        [TestMethod]
        public void AddTest3()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "10 + 30*cos(2*x) + 40*sin(2*x)");
        }

        [TestMethod]
        public void SubTest()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test - test2;
            Assert.AreEqual(test3.ToString(), "-5 - 14*cos(x) - 19*sin(x)");
        }

        [TestMethod]
        public void SubTest2()
        {
            var test = new FourierSeries(new double[] { 12, 8 }, new double[] { 3, 8 }, 2 * Math.PI, FourierSeriesType.CosSin);
            var test2 = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test3 = test - test2;
            Assert.AreEqual(test3.ToString(), "1 - 7*cos(x) - 12*sin(x)");
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
            Assert.AreEqual(test2.ToString(), "10*cos(x) + 10*sin(x)");
        }

        [TestMethod]
        public void Mul1Test2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = 3 * test;
            Assert.AreEqual(test2.ToString(), "15 + 45*cos(2*x) + 60*sin(2*x)");
        }

        [TestMethod]
        public void Mul2Test()
        {
            var test = new FourierSeries(new double[] { 0.0, 1.0 }, new double[] { 0.0, 1.0 }, 2 * Math.PI, FourierSeriesType.Sin);
            var test2 = test * 10;
            Assert.AreEqual(test2.ToString(), "10*cos(x) + 10*sin(x)");
        }

        [TestMethod]
        public void Mul2Test2()
        {
            var test = new FourierSeries(new double[] { 10, 15 }, new double[] { 15, 20 }, Math.PI, FourierSeriesType.Cos);
            var test2 = test * 3;
            Assert.AreEqual(test2.ToString(), "15 + 45*cos(2*x) + 60*sin(2*x)");
        }
    }
    [TestClass]
    public class TaylorSeriesTest
    {
        [TestMethod]
        public void TaylorSerTest()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            Assert.AreEqual(test.ToString(), "1 + x + 0,5*x^2");
        }

        [TestMethod]
        public void TaylorSer2Test()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 2);
            Assert.AreEqual(test.ToString(), "1 + x - 0,5*x^2");
        }

        [TestMethod]
        public void TaylorSer3Test()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            Assert.AreEqual(test.ToString(), "1 + x");
        }

        [TestMethod]
        public void AddTest()
        {
            var test = new TaylorSeries("e^x", 0, 3);
            var test2 = new TaylorSeries("sin(x)", 0, 3);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "1 + 2*x + 0,5*x^2");
        }

        [TestMethod]
        public void AddTest2()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 2);
            var test2 = new TaylorSeries("sin(x)", 0, 3);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "1 + 2*x - 0,5*x^2 - 0,16666666666666666*x^3");
        }

        [TestMethod]
        public void AddTest3()
        {
            var test = new TaylorSeries("cos(x)", 0, 2);
            var test2 = new TaylorSeries("sin(x)", 0, 2);
            var test3 = test + test2;
            Assert.AreEqual(test3.ToString(), "1 + x - 0,5*x^2");
        }

        [TestMethod]
        public void ComputeTest()
        {
            var test = new TaylorSeries("e^x", 1, 100);
            Assert.AreEqual(Math.Round(test.Compute(3), 3), 20.086);
        }

        [TestMethod]
        public void ComputeTest2()
        {
            var test = new TaylorSeries("cos(x)*sin(x)", 1, 100);
            Assert.AreEqual(Math.Round(test.Compute(5), 3), -0.272);
        }

        [TestMethod]
        public void ComputeTest3()
        {
            var test = new TaylorSeries("cos(x)", 1, 100);
            Assert.AreEqual(Math.Round(test.Compute(1), 3), 0.54);
        }

        [TestMethod]
        public void UnaryMinusTest()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            var test2 = -test;
            Assert.AreEqual(test2.ToString(), "-1 - x - 0,5*x^2");
        }

        [TestMethod]
        public void UnaryMinus2Test()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 2);
            var test2 = -test;
            Assert.AreEqual(test2.ToString(), "-1 - x + 0,5*x^2");
        }

        [TestMethod]
        public void UnaryMinus3Test()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            var test2 = -test;
            Assert.AreEqual(test2.ToString(), "-1 - x");
        }

        [TestMethod]
        public void CompTest()
        {
            var test = new TaylorSeries("e^x", 0, 3);
            var test2 = new TaylorSeries("sin(x)", 0, 3);
            bool test3 = test == test2;
            Assert.AreEqual(test3, false);
        }

        [TestMethod]
        public void CompTest2()
        {
            var test = new TaylorSeries("sin(x)", 0, 2);
            var test2 = new TaylorSeries("cos(x) * sin(x)", 0, 2);
            bool test3 = test == test2;
            Assert.AreEqual(test3, true);
        }

        [TestMethod]
        public void CompTest3()
        {
            var test = new TaylorSeries("cos(x)", 0, 2);
            var test2 = new TaylorSeries("sin(x)", 0, 2);
            bool test3 = test == test2;
            Assert.AreEqual(test3, false);
        }

        [TestMethod]
        public void UnCompTest()
        {
            var test = new TaylorSeries("e^x", 0, 3);
            var test2 = new TaylorSeries("sin(x)", 0, 3);
            bool test3 = test != test2;
            Assert.AreEqual(test3, true);
        }

        [TestMethod]
        public void UnCompTest2()
        {
            var test = new TaylorSeries("sin(x)", 0, 2);
            var test2 = new TaylorSeries("cos(x) * sin(x)", 0, 2);
            bool test3 = test != test2;
            Assert.AreEqual(test3, false);
        }

        [TestMethod]
        public void UnCompTest3()
        {
            var test = new TaylorSeries("cos(x)", 0, 2);
            var test2 = new TaylorSeries("sin(x)", 0, 2);
            bool test3 = test != test2;
            Assert.AreEqual(test3, true);
        }

        [TestMethod]
        public void MulTest()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            var test2 = test * 5;
            Assert.AreEqual(test2.ToString(), "5 + 5*x + 2,5*x^2");
        }

        [TestMethod]
        public void MulTest2()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 2);
            var test2 = test * 10;
            Assert.AreEqual(test2.ToString(), "10 + 10*x - 5*x^2");
        }

        [TestMethod]
        public void MulTest3()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            var test2 = test * 3;
            Assert.AreEqual(test2.ToString(), "3 + 3*x");
        }

        [TestMethod]
        public void Mul2Test()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            var test2 = 5 * test;
            Assert.AreEqual(test2.ToString(), "5 + 5*x + 2,5*x^2");
        }

        [TestMethod]
        public void Mul2Test2()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 2);
            var test2 = 10 * test;
            Assert.AreEqual(test2.ToString(), "10 + 10*x - 5*x^2");
        }

        [TestMethod]
        public void Mul2Test3()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            var test2 = 3 * test;
            Assert.AreEqual(test2.ToString(), "3 + 3*x");
        }

        [TestMethod]
        public void GetDerivativeTest()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            Assert.AreEqual(test.GetDerivative().ToString(), "1 + x");
        }

        [TestMethod]
        public void GetDerivativeTest2()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 3);
            Assert.AreEqual(test.GetDerivative().ToString(), "1 - x - 0,5*x^2");
        }

        [TestMethod]
        public void GetDerivativeTest3()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            Assert.AreEqual(test.GetDerivative().ToString(), "1");
        }

        [TestMethod]
        public void GetIntegralTest()
        {
            var test = new TaylorSeries("e^x", 0, 2);
            Assert.AreEqual(test.GetIntegral().ToString(), "x + 0,5*x^2");
        }

        [TestMethod]
        public void GetIntegralTest2()
        {
            var test = new TaylorSeries("cos(x) + sin(x)", 0, 3);
            Assert.AreEqual(test.GetIntegral().ToString(), "x + 0,5*x^2 - 0,16666666666666666*x^3");
        }

        [TestMethod]
        public void GetIntegralTest3()
        {
            var test = new TaylorSeries("cos(x) * e^x", 0, 2);
            Assert.AreEqual(test.GetIntegral().ToString(), "x + 0,5*x^2");
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