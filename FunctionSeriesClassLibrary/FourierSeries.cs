using System.Net.Http.Headers;

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
                        res += " + Cos(" + (j == 1 ? "" : j) + "x)";
                    else if (a[j] == -1)
                        res += " - Cos(" + (j == 1 ? "" : j) + "x)";
                    else
                        res += (a[j] > 0 ? " + " : " - ") + Math.Abs(a[j]) + "*" + "Cos(" + (j == 1 ? "" : j) + "x)";
                if (b[j] != 0)
                    if (b[j] == 1)
                        res += " + Sin(" + (j == 1 ? "" : j) + "x)";
                    else if (b[j] == -1)
                        res += " - Sin(" + (j == 1 ? "" : j) + "x)";
                    else
                        res += (b[j] > 0 ? " + " : " - ") + Math.Abs(b[j]) + "*" + "Sin(" + (j == 1 ? "" : j) + "x)";
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
                        res = "Cos(" + (i == 1 ? "" : i) + "x)";
                    else if (a[i] == -1)
                        res = "-Cos(" + (i == 1 ? "" : i) + "x)";
                    else
                        res = a[i] + "*" + "Cos(" + (i == 1 ? "" : i) + "x)";
                    if (b[i] != 0)
                        if (b[i] == 1)
                            res += " + Sin(" + (i == 1 ? "" : i) + "x)";
                        else if (b[i] == -1)
                            res += " - Sin(" + (i == 1 ? "" : i) + "x)";
                        else
                            res += " + " + b[i] + "*" + "Sin(" + (i == 1 ? "" : i) + "x)";
                    return res;
                }
                if (b[i] != 0)
                {
                    if (b[i] == 1)
                        res = "Sin(" + (i == 1 ? "" : i) + "x)";
                    else if (b[i] == -1)
                        res = "-Sin(" + (i == 1 ? "" : i) + "x)";
                    else
                        res = b[i] + "*" + "Sin(" + (i == 1 ? "" : i) + "x)";
                    return res;
                }
            }
            return res;
        }
    }
}