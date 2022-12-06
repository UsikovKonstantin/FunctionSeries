using FunctionSeriesClassLibrary;

FourierSeries fs = new FourierSeries(10, 6.28, "x", FourierSeriesType.Sin);
Console.WriteLine(fs);
Console.WriteLine(fs.Integral("x sin x cos *", 0, 5, 100));