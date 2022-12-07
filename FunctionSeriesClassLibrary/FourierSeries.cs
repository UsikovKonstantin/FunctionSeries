

namespace FunctionSeriesClassLibrary
{
    /// <summary>
    /// Тип ряда Фурье.
    /// CosSin - разложение в ряд синусов и косинусов.
    /// Cos - разложение в ряд косинусов (функция рассматривается как чётная).
    /// Sin -  разложение в ряд синусов (функция рассматривается как нечётная).
    /// </summary>
    public enum FourierSeriesType
    {
        CosSin,
        Cos,
        Sin
    }

    /// <summary>
    /// Класс "Ряд Фурье".
    /// </summary>
    public class FourierSeries
    {
        private int n = 0;  // количество членов
        private double period = 0;  // период функции
        private string function = "";  // функция, по которой строится ряд
        private string polFunc = "";  // польская запись
        private double[] a = new double[0];  // коэффициенты при косинусах 
        private double[] b = new double[0];  // коэффициенты при синусах
        private FourierSeriesType type = FourierSeriesType.CosSin;  // тип ряда

        // Свойства
        public int N
        {
            get { return n; }
        }
        public string Function
        {
            get { return function; }
        }
        public double Period
        {
            get { return period; }
        }
        public double[] A
        {
            get { return a; }
        }
        public double[] B
        {
            get { return b; }
        }
        public FourierSeriesType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="n"> количество членов </param>
        /// <param name="period"> период функции </param>
        /// <param name="function"> функция, по которой строится ряд </param>
        /// <param name="type"> тип ряда </param>
        public FourierSeries(int n, double period, string function, FourierSeriesType type)
        {
            this.n = n;
            this.period = period;
            this.function = function;
            this.type = type;
            a = new double[n + 1];
            b = new double[n + 1];
            polFunc = Interpreter.GetPolishExpression(function);

            if (type == FourierSeriesType.Sin)
                Parallel.For(1, n + 1, i =>
                {
                    b[i] = 4 / period * Integral(polFunc + (i * Math.PI / period * 2) + " x * sin *", 0, period / 2, 100);
                });
            else if (type == FourierSeriesType.Cos)
                Parallel.For(0, n + 1, i =>
                {
                    a[i] = 4 / period * Integral(polFunc + (i * Math.PI / period * 2) + " x * cos *", 0, period / 2, 100);
                });
            else
                Parallel.For(0, n + 1, i =>
                {
                    a[i] = 2 / period * Integral(polFunc + (i * Math.PI / period * 2) + " x * cos *", -period / 2, period / 2, 200);
                    b[i] = 2 / period * Integral(polFunc + (i * Math.PI / period * 2) + " x * sin *", -period / 2, period / 2, 200);
                });
        }

        /// <summary>
        /// Вычисление значения в точке.
        /// </summary>
        /// <param name="x"> точка, в которой необходимо вычислить значение </param>
        /// <returns> значение ряда Фурье </returns>
        public double Compute(double x)
        { 
            double res = a[0] / 2;
            double c = Math.PI / period * 2 * x;
            for (int i = 1; i <= n; i++)
            {
                double cc = i * c;
                res += a[i] * Math.Cos(cc) + b[i] * Math.Sin(cc);
            }
            return res;
        }

        /// <summary>
        /// Переопределение метода ToString().
        /// </summary>
        /// <returns> строковое представление ряда Фурье</returns>
        public override string ToString()
        {
            int i = 0;
            string res = FindFirst(ref i);
            for (int j = i + 1; j <= n; j++)
            {
                if (a[j] != 0)
                    if (a[j] == 1)
                        res += " + Cos(" + (j * Math.PI / period * 2) + "x)";
                    else if (a[j] == -1)
                        res += " - Cos(" + (j * Math.PI / period * 2) + "x)";
                    else
                        res += (a[j] > 0 ? " + " : " - ") + Math.Abs(a[j]) + "*" + "Cos(" + (j * Math.PI / period * 2) + "x)";
                if (b[j] != 0)
                    if (b[j] == 1)
                        res += " + Sin(" + (j * Math.PI / period * 2) + "x)";
                    else if (b[j] == -1)
                        res += " - Sin(" + (j * Math.PI / period * 2) + "x)";
                    else
                        res += (b[j] > 0 ? " + " : " - ") + Math.Abs(b[j]) + "*" + "Sin(" + (j * Math.PI / period * 2) + "x)";
            }
            return res;
        }

        /// <summary>
        /// Поиск первого ненулевого члена в массиве коэффициентов.
        /// </summary>
        /// <param name="i"> индекс </param>
        /// <returns> первый ненулевой член ряда </returns>
        private string FindFirst(ref int i)
        {
            string res = "";
            if (a[0] != 0)
                return (a[0] / 2).ToString();
            for (i = 1; i <= n; i++)
            {
                if (a[i] != 0)
                {
                    if (a[i] == 1)
                        res = "Cos(" + (i * Math.PI / period * 2) + "x)";
                    else if (a[i] == -1)
                        res = "-Cos(" + (i * Math.PI / period * 2) + "x)";
                    else
                        res = a[i] + "*" + "Cos(" + (i * Math.PI / period * 2) + "x)";
                    if (b[i] != 0)
                        if (b[i] == 1)
                            res += " + Sin(" + (i * Math.PI / period * 2) + "x)";
                        else if (b[i] == -1)
                            res += " - Sin(" + (i * Math.PI / period * 2) + "x)";
                        else
                            res += " + " + b[i] + "*" + "Sin(" + (i * Math.PI / period * 2) + "x)";
                    return res;
                }
                if (b[i] != 0)
                {
                    if (b[i] == 1)
                        res = "Sin(" + (i * Math.PI / period * 2) + "x)";
                    else if (b[i] == -1)
                        res = "-Sin(" + (i * Math.PI / period * 2) + "x)";
                    else
                        res = b[i] + "*" + "Sin(" + (i * Math.PI / period * 2) + "x)";
                    return res;
                }
            }
            return res;
        }

        /// <summary>
        /// Вычисление интеграла.
        /// </summary>
        /// <param name="a"> начальный предел интегрирования </param>
        /// <param name="b"> конечный предел интегрирования </param>
        /// <param name="steps"> количество шагов </param>
        /// <returns> значение интеграла </returns>
        public double Integral(string polFunc, double a, double b, int steps)
        {
            double step = (b - a) / steps;  // ширина шага
            double res = 0;
            double[] values = new double[steps + 1];
            Parallel.For(0, steps + 1, i =>
            {
                values[i] = Interpreter.SolvePolishExpression(polFunc, new Dictionary<char, double> { { 'x', a + step * i } });
            });
            for (int i = 0; i < steps; i++)
                res += values[i] + values[i + 1];
            res *= 0.5 * step;
            return res;
        }
    }
}