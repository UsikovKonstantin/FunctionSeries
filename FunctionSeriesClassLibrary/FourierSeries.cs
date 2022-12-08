using System.Numerics;

namespace FunctionSeriesClassLibrary
{
    /// <summary>
    /// Тип ряда Фурье.
    /// CosSin - разложение в ряд синусов и косинусов (функция общего вида).
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
        private double l = 0;  // полупериод функции
        private double[] a = new double[0];  // коэффициенты при косинусах 
        private double[] b = new double[0];  // коэффициенты при синусах
        private FourierSeriesType type = FourierSeriesType.CosSin;  // тип ряда

        /// <summary>
        /// Свойство "количество членов".
        /// </summary>
        public int N
        {
            get { return n; }
        }
        /// <summary>
        /// Свойство "полупериод функции".
        /// </summary>
        public double L
        {
            get { return l; }
        }
        /// <summary>
        /// Свойство "коэффициенты при косинусах".
        /// </summary>
        public double[] A
        {
            get { return a; }
        }
        /// <summary>
        /// Свойство "коэффициенты при синусах".
        /// </summary>
        public double[] B
        {
            get { return b; }
        }
        /// <summary>
        /// Свойство "тип ряда".
        /// </summary>
        public FourierSeriesType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Конструктор ряда Фурье.
        /// </summary>
        /// <param name="n"> количество членов </param>
        /// <param name="period"> период функции </param>
        /// <param name="function"> функция, по которой строится ряд </param>
        /// <param name="type"> тип ряда </param>
        public FourierSeries(int n, double period, string function, FourierSeriesType type)
        {
            this.n = n;
            this.type = type;
            l = period / 2;
            a = new double[n + 1];
            b = new double[n + 1];
            string polFunc = Interpreter.GetPolishExpression(function);
            FindCoefs(polFunc);
        }

        /// <summary>
        /// Поиск коэффициентов ряда Фурье.
        /// </summary>
        /// <param name="polFunc"> польская запись исходной функции </param>
        private void FindCoefs(string polFunc)
        {
            const int STEPS = 200, STEPS_SYMM = 100;
            double PI_DIV_L = Math.PI / l, ONE_DIV_L = 1 / l, TWO_DIV_L = ONE_DIV_L * 2;
            string polSin = " x * sin *", polCos = " x * cos *";
            if (type == FourierSeriesType.Sin)
                Parallel.For(1, n + 1, i =>
                    b[i] = TWO_DIV_L * Integral(polFunc + (i * PI_DIV_L) + polSin, 0, l, STEPS_SYMM));
            else if (type == FourierSeriesType.Cos)
                Parallel.For(0, n + 1, i =>
                    a[i] = TWO_DIV_L * Integral(polFunc + (i * PI_DIV_L) + polCos, 0, l, STEPS_SYMM));
            else
                Parallel.For(0, n + 1, i =>
                {
                    a[i] = ONE_DIV_L * Integral(polFunc + (i * PI_DIV_L) + polCos, -l, l, STEPS);
                    b[i] = ONE_DIV_L * Integral(polFunc + (i * PI_DIV_L) + polSin, -l, l, STEPS);
                });
        }

        /// <summary>
        /// Переопределение метода ToString().
        /// </summary>
        /// <returns> строковое представление ряда Фурье </returns>
        public override string ToString()
        {
            int i = 0;
            string res = FindFirst(ref i);
            double PI_DIV_L = Math.PI / l;
            for (int j = i + 1; j <= n; j++)
            {
                if (a[j] != 0)
                    if (Math.Abs(a[j]) == 1)
                        res += (a[j] == 1 ? " + " : " - ") + "Cos(" + (j * PI_DIV_L) + "x)";
                    else
                        res += (a[j] > 0 ? " + " : " - ") + Math.Abs(a[j]) + "*" + "Cos(" + (j * PI_DIV_L) + "x)";
                if (b[j] != 0)
                    if (Math.Abs(b[j]) == 1)
                        res += (b[j] == 1 ? " + " : " - ") + "Sin(" + (j * PI_DIV_L) + "x)";
                    else
                        res += (b[j] > 0 ? " + " : " - ") + Math.Abs(b[j]) + "*" + "Sin(" + (j * PI_DIV_L) + "x)";
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
            if (a[0] != 0) return (a[0] / 2).ToString();
            double PI_DIV_L = Math.PI / l;
            for (i = 1; i <= n; i++)
            {
                if (a[i] != 0)
                {
                    if (Math.Abs(a[i]) == 1)
                        res += (a[i] == 1 ? "" : "-") + "Cos(" + (i * PI_DIV_L) + "x)";
                    else
                        res += a[i] + "*" + "Cos(" + (i * PI_DIV_L) + "x)";
                    if (b[i] != 0)
                        if (Math.Abs(b[i]) == 1)
                            res += (b[i] == 1 ? " + " : " - ") + "Sin(" + (i * PI_DIV_L) + "x)";
                        else
                            res += " + " + b[i] + "*" + "Sin(" + (i * PI_DIV_L) + "x)";
                    return res;
                }
                if (b[i] != 0)
                {
                    if (Math.Abs(b[i]) == 1)
                        res += (b[i] == 1 ? "" : "-") + "Sin(" + (i * PI_DIV_L) + "x)";
                    else
                        res += b[i] + "*" + "Sin(" + (i * PI_DIV_L) + "x)";
                    return res;
                }
            }
            return res;
        }

        /// <summary>
        /// Вычисление значения в точке.
        /// </summary>
        /// <param name="x"> точка, в которой необходимо вычислить значение </param>
        /// <returns> значение ряда Фурье </returns>
        public double Compute(double x)
        { 
            double res = a[0] / 2;
            double k = Math.PI / l * x;
            for (int i = 1; i <= n; i++)
            {
                double c = i * k;
                res += a[i] * Math.Cos(c) + b[i] * Math.Sin(c);
            }
            return res;
        }

        /// <summary>
        /// Вычисление интеграла.
        /// </summary>
        /// <param name="polFunc"> польская запись функции </param>
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
                values[i] = Interpreter.SolvePolishExpression(polFunc, new Dictionary<char, double> { { 'x', a + step * i } }));
            for (int i = 0; i < steps; i++)
                res += values[i] + values[i + 1];
            res *= 0.5 * step;
            return res;
        }

        #region NUDFT
        /// <summary>
        /// Получение ряда Фурье посредством поля точек (Тип 2)
        /// </summary>
        /// <param name="point_cloud">Поле точек</param>
        /// <param name="precision">Количество коэффициентов</param>
        public FourierSeries(List<(double x, double y)> point_cloud, int precision)
        {
            n = precision;
            l = Get_Period(point_cloud);
            type = FourierSeriesType.CosSin;
            a = new double[n + 1];
            b = new double[n + 1];
            NDFT_Coefficients(point_cloud, precision);
        }

        double Get_Period(List<(double x, double y)> point_cloud)
        {
            double min_x = Double.MaxValue, max_x = Double.MinValue;
            
            foreach (var point in point_cloud)
            {
                min_x = Math.Min(min_x, point.x);
                max_x = Math.Max(max_x, point.x);
            }

            return (max_x - min_x) / 2;
        }

        void NDFT_Coefficients(List<(double x, double y)> point_cloud, int precision)
        {
            double Delta_K = Math.PI / l;
            for (int i = 0; i < precision; i++)
            {
                Complex Coef = Get_Specific(Delta_K, point_cloud, i + 1);
                a[i] = Math.Cos(Coef.Phase);
                b[i] = Math.Sin(Coef.Phase);
            }
        }

        Complex Get_Specific(double Delta_K, List<(double x, double y)> point_cloud, int frequency)
        {
            Complex Coef = new (0, 0);
            
            foreach (var point in point_cloud)
            {
                Coef += point.y * Complex.Pow(new(Math.E, 0),Complex.ImaginaryOne * Delta_K * frequency * point.x);
            }
            
            return Coef;
        }
        #endregion
    }
}