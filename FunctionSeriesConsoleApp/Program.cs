﻿using FunctionSeriesClassLibrary;
using System.Diagnostics;
using System.Numerics;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

//FourierSeries fs = new FourierSeries(10, 6.28, "x", FourierSeriesType.Sin);
//Console.WriteLine(fs);

//Stopwatch sw = Stopwatch.StartNew();
//Console.WriteLine(fs.Integral("e x ^", -3.14, 3.14, 5000000));
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);

//string test = "3";
//bool test2 = Interpreter.IsVarInPolExpression(test);
//Console.WriteLine(test2);

//Console.WriteLine(new FourierApprox(new List<(double x, double y)>() { (12, 11), (8, 11) }, FourierApprox.transform_type.fast));


//Expr x = Expr.Variable("x");
//Expr func = new Expr(Infix.Parse("(-x-1)^2+cos((-x-1))").ResultValue);
//Console.WriteLine("f(x)= " + func.ToString());

//Expr derivative = func.Differentiate(x);
//Console.WriteLine("f'(x)= "+derivative.ToString());

//string TaylorSeries(string fun, double x0) {
//    string res = "";
//    string newFun = fun;
//    if (x0 != 0) newFun.Replace("x", $"(x{(x0 > 0 ? "-" : "+")}{x0})");

//    Expr x = Expr.Variable("x");
//    Expr func = new Expr(Infix.Parse(newFun).ResultValue);
//    Expr derivative1 = func.Differentiate(x);
//    Expr derivative2 = derivative1.Differentiate(x);
//    Expr derivative3 = derivative2.Differentiate(x);
//    res += $"({derivative1})/1!*(x-{x0}) ";
//    res += $"+ ({derivative2})/2!*(x-{x0})^2 ";
//    res += $"+ ({derivative3})/6!*(x-{x0})^3 ";
//    res += $"... +({func})^(n)({x0})/n!*(x-{x0})^n";
//    res += $"+ ...";
//    return res;
//}

//Console.WriteLine(TaylorSeries("x^2+2", 2));


//TaylorSeries test = new TaylorSeries("cos(x)", 0, 5);
//TaylorSeries test2 = new TaylorSeries("cos(x)", 0, 5);
//Console.WriteLine(test);
//Console.WriteLine(test.GetIntegral().GetDerivative());
//var bruh = test.GetIntegral().GetDerivative();
//Console.WriteLine(bruh == test2);
//Console.WriteLine(test2);
//Console.WriteLine(-test2);
//Console.WriteLine(test.GetIntegral());
//Console.WriteLine(test.GetIntegral().GetIntegral());
//Console.WriteLine(test.GetIntegral().GetIntegral().Compute(2));
//Console.WriteLine(test.GetDerivative());
//Console.WriteLine(test.GetDerivative().GetDerivative());
//Console.WriteLine(test.GetDerivative().GetDerivative().GetDerivative());
//Console.WriteLine(test.GetDerivative().GetDerivative().GetDerivative().GetDerivative());
//Console.WriteLine(test.GetDerivative().GetDerivative().GetDerivative().GetDerivative().GetDerivative());

//Console.WriteLine(FunctionHelper.GetType("x"));
//Console.WriteLine(FunctionHelper.GetType("x^3"));
//Console.WriteLine(FunctionHelper.GetType("sin(x)"));
//Console.WriteLine(FunctionHelper.GetType("cos(x)"));
//Console.WriteLine(FunctionHelper.GetType("x^2 + 5"));
//Console.WriteLine(FunctionHelper.GetType("1"));
//Console.WriteLine(FunctionHelper.GetType("e^x"));
//Console.WriteLine(FunctionHelper.GetType("x^2 + x"));

//FourierSeries fs = new FourierSeries(new double[] { 1, 2, 4, -5}, new double[] { 1, 2, 4, -5 }, 5, FourierSeriesType.CosSin);
//Console.WriteLine(fs);

//TaylorSeries t = new TaylorSeries("cos(x)", 0, 5);
//Console.WriteLine(t);
//Console.WriteLine(t.GetIntegral());
//Console.WriteLine(t.GetIntegral().GetDerivative());
TaylorSeries test = new TaylorSeries("cos(x)", 0, 5);
TaylorSeries test2 = new TaylorSeries("cos(x)", 0, 5);
var test3 = test + test2;
Console.WriteLine(test3);