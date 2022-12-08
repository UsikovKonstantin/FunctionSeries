using FunctionSeriesClassLibrary;
using System.Diagnostics;
using System.Numerics;

//FourierSeries fs = new FourierSeries(10, 6.28, "x", FourierSeriesType.Sin);
//Console.WriteLine(fs);

//Stopwatch sw = Stopwatch.StartNew();
//Console.WriteLine(fs.Integral("e x ^", -3.14, 3.14, 5000000));
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);

string test = "3";
bool test2 = Interpreter.IsVarInPolExpression(test);
Console.WriteLine(test2);

Console.WriteLine(new FourierSeries(new List<(double x, double y)>() {(1,1),(2,2)}, 2).Compute(1.4));