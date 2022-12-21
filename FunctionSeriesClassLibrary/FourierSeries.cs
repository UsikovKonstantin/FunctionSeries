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
        /// Конструктор по умолчанию.
        /// Создаёт ряд sin(x) + cos(x).
        /// </summary>
        public FourierSeries()
        {
            n = 1;
            a = new double[] { 0, 1 };
            b = new double[] { 0, 1 };
            l = Math.PI;
            type = FourierSeriesType.CosSin;
        }

        /// <summary>
        /// Конструктор ряда Фурье по функции.
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
        /// Конструктор ряда Фурье по массивам коэффициентов.
        /// </summary>
        /// <param name="A"> коэффициенты при косинусах </param>
        /// <param name="B"> коэффициенты при синусах </param>
        /// <param name="period"> период функции </param>
        /// <param name="type"> тип ряда </param>
        public FourierSeries(double[] A, double[] B, double period, FourierSeriesType type)
        {
            n = A.Length - 1;
            a = A;
            b = B;
            l = period / 2;
            this.type = type;
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

        /// <summary>
        /// Почленное дифференцирование ряда Фурье.
        /// </summary>
        /// <returns> ряд Фурье - результат дифференцирования </returns>
        public FourierSeries GetDerivative()
        {
            double[] AA = new double[n + 1];
            double[] BB = new double[n + 1];
            double step = Math.PI / l;
            for (int i = 1; i < n + 1; i++)
            {
                double c = i * step;
                AA[i] = B[i] * c;
                BB[i] = -A[i] * c;
            }
            return new FourierSeries(AA, BB, 2 * l, type);
        }

        /// <summary>
        /// Почленное интегрирование ряда Фурье.
        /// </summary>
        /// <returns> ряд Фурье - результат интегрирования </returns>
        public FourierSeries GetIntegral()
        {
            double[] AA = new double[n + 1];
            double[] BB = new double[n + 1];
            double step = Math.PI / l;
            for (int i = 1; i < n + 1; i++)
            {
                double c = i * step;
                AA[i] = -B[i] / c;
                BB[i] = A[i] / c;
            }
            return new FourierSeries(AA, BB, 2 * l, type);
        }

        /// <summary>
        /// Сложение рядов с одинаковым периодом.
        /// </summary>
        /// <param name="f1"> первый ряд </param>
        /// <param name="f2"> второй ряд </param>
        /// <returns> сумма рядов </returns>
        public static FourierSeries operator +(FourierSeries f1, FourierSeries f2)
        {
            int n = Math.Max(f1.n, f2.n);
            int m = Math.Min(f1.n, f2.n);
            double[] A = new double[n + 1];
            double[] B = new double[n + 1];
            for (int i = 0; i <= m; i++)
            {
                A[i] += f1.A[i] + f2.A[i];
                B[i] += f1.B[i] + f2.B[i];
            }
            for (int i = m + 1; i <= n; i++)
            {
                A[i] += f1.n > f2.n ? f1.A[i] : f2.A[i];
                B[i] += f1.n > f2.n ? f1.B[i] : f2.B[i];
            }
            FourierSeriesType resType;
            if (f1.type == FourierSeriesType.Sin && f2.type == FourierSeriesType.Sin)
                resType = FourierSeriesType.Sin;
            else if (f1.type == FourierSeriesType.Cos && f2.type == FourierSeriesType.Cos)
                resType = FourierSeriesType.Cos;
            else
                resType = FourierSeriesType.CosSin;
            return new FourierSeries(A, B, 2 * f1.l, resType);
        }

        /// <summary>
        /// Вычитание рядов с одинаковым периодом.
        /// </summary>
        /// <param name="f1"> первый ряд </param>
        /// <param name="f2"> второй ряд </param>
        /// <returns> разность рядов </returns>
        public static FourierSeries operator -(FourierSeries f1, FourierSeries f2)
        {
            int n = Math.Max(f1.n, f2.n);
            int m = Math.Min(f1.n, f2.n);
            double[] A = new double[n + 1];
            double[] B = new double[n + 1];
            for (int i = 0; i <= m; i++)
            {
                A[i] += f1.A[i] - f2.A[i];
                B[i] += f1.B[i] - f2.B[i];
            }
            for (int i = m + 1; i <= n; i++)
            {
                A[i] += f1.n > f2.n ? f1.A[i] : -f2.A[i];
                B[i] += f1.n > f2.n ? f1.B[i] : -f2.B[i];
            }
            FourierSeriesType resType;
            if (f1.type == FourierSeriesType.Sin && f2.type == FourierSeriesType.Sin)
                resType = FourierSeriesType.Sin;
            else if (f1.type == FourierSeriesType.Cos && f2.type == FourierSeriesType.Cos)
                resType = FourierSeriesType.Cos;
            else
                resType = FourierSeriesType.CosSin;
            return new FourierSeries(A, B, 2 * f1.l, resType);
        }

        /// <summary>
        /// Переопределение унарного оператора "+".
        /// </summary>
        /// <param name="f1"> ряд Фурье </param>
        /// <returns> результат применения оператора </returns>
        public static FourierSeries operator +(FourierSeries fs)
        {
            return fs;
        }

        /// <summary>
        /// Переопределение унарного оператора "-".
        /// </summary>
        /// <param name="f1"> ряд Фурье </param>
        /// <returns> результат применения оператора </returns>
        public static FourierSeries operator -(FourierSeries fs)
        {
            int n = fs.n;
            double[] A = new double[n + 1];
            double[] B = new double[n + 1];
            for (int i = 0; i <= n; i++)
            {
                A[i] = -fs.A[i];
                B[i] = -fs.B[i];
            }
            return new FourierSeries(A, B, 2 * fs.l, fs.type);
        }

        /// <summary>
        /// Переопределение оператора сравнения.
        /// </summary>
        /// <param name="f1"> первый ряд </param>
        /// <param name="f2"> второй ряд </param>
        /// <returns> true - если ряды равны, иначе - false </returns>
        public static bool operator ==(FourierSeries f1, FourierSeries f2)
        {
            if (f1.n == f2.n && f1.l == f2.l && f1.type == f2.type &&
                f1.a.SequenceEqual(f2.a) && f1.b.SequenceEqual(f2.b))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Переопределение оператора сравнения.
        /// </summary>
        /// <param name="f1"> первый ряд </param>
        /// <param name="f2"> второй ряд </param>
        /// <returns> true - если ряды не равны, иначе - false </returns>
        public static bool operator !=(FourierSeries f1, FourierSeries f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Переопределение метода Equals().
        /// </summary>
        /// <param name="obj"> объект для сравнения </param>
        /// <returns> true - если объекты равны, иначе - false </returns>
        public override bool Equals(object? obj)
        {
            if (obj is FourierSeries fs)
                return this == fs;
            else
                return false;
        }

        /// <summary>
        /// Переопределение метода GetHashCode().
        /// </summary>
        /// <returns> хэш-код </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(n, l, a, b, type);
        }

        /// <summary>
        /// Умножение ряда на число.
        /// </summary>
        /// <param name="k"> число </param>
        /// <param name="fs"> ряд Фурье </param>
        /// <returns> ряд Фурье - результат умножения </returns>
        public static FourierSeries operator *(double k, FourierSeries fs)
        {
            int n = fs.n;
            double[] A = new double[n + 1];
            double[] B = new double[n + 1];
            for (int i = 0; i <= n; i++)
            {
                A[i] = k * fs.A[i];
                B[i] = k * fs.B[i];
            }
            return new FourierSeries(A, B, 2 * fs.l, fs.type);
        }

        /// <summary>
        /// Умножение ряда на число.
        /// </summary>
        /// <param name="fs"> ряд Фурье </param>
        /// <param name="k"> число </param>
        /// <returns> ряд Фурье - результат умножения </returns>
        public static FourierSeries operator *(FourierSeries fs, double k)
        {
            return k * fs;
        }
    }
}