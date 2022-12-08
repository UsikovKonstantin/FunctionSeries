using FunctionSeriesClassLibrary;
using System.Diagnostics;

//FourierSeries fs = new FourierSeries(10, 6.28, "x", FourierSeriesType.Sin);
//Console.WriteLine(fs);

//Stopwatch sw = Stopwatch.StartNew();
//Console.WriteLine(fs.Integral("e x ^", -3.14, 3.14, 5000000));
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);

string test = "3";
bool test2 = Interpreter.IsVarInPolExpression(test);
Console.WriteLine(test2);