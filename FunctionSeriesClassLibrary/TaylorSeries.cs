using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace FunctionSeriesClassLibrary {
    public class TaylorSeries {
        private string function; // заданная функция
        private double x0; // x0
        private double[] coefs; // коэффициенты
        private int n; // кол-во коэффициентов в ряде

        /// <summary>
        /// Свойство "функция"
        /// </summary>
        public string Function { get { return function; } }
        /// <summary>
        /// Свойство "x0"
        /// </summary>
        public double X0 { get { return x0; } }
        /// <summary>
        /// Свойтсво "коэффициенты"
        /// </summary>
        public double[] Coefs { get { return coefs; } }
        /// <summary>
        /// Свойство "кол-во коэффициентов в ряде"
        /// </summary>
        public int N { get { return n; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="function">заданная функция</param>
        /// <param name="x0">x0</param>
        /// <param name="n">кол-во коэффициентов в ряде</param>
        /// <param name="computeCoefs">вычислять ли коэффициенты сразу же при создания объекта</param>
        /// <exception cref="Exception">если кол-во коэффициентов в ряду меньше нуля</exception>
        public TaylorSeries(string function, double x0, int n, bool computeCoefs = true) {
            if (n < 0) throw new Exception("Кол-во коэффициентов в ряду должно быть больше нуля");
            this.function = function;
            this.x0 = x0;
            coefs = new double[n + 1];
            this.n = n;
            if (computeCoefs) FindCoefs();
        }

        /// <summary>
        /// Сложение рядов Тейлора
        /// </summary>
        /// <param name="t1">ряд Тейлора 1</param>
        /// <param name="t2">ряд Тейлора 2</param>
        /// <returns>результат сложения</returns>
        /// <exception cref="Exception">если x0 у рядов не равны</exception>
        public static TaylorSeries operator +(TaylorSeries t1, TaylorSeries t2) {
            if (t1.X0 != t2.X0) throw new Exception("x0 должны быть равны");
            int n = Math.Max(t1.N, t2.N);
            string function = $"{t1.Function} + ${t2.Function}";
            double[] coefs = new double[n + 1];
            for (int i = 0; i < t1.Coefs.Length; i++) coefs[i] += t1.Coefs[i];
            for (int i = 0; i < t2.Coefs.Length; i++) coefs[i] += t2.Coefs[i];
            TaylorSeries res = new TaylorSeries(function, t1.X0, n, false);
            res.coefs = coefs;
            return res;
        }

        /// <summary>
        /// Переопределение унарного оператора "+"
        /// </summary>
        /// <param name="ts">ряд Тейлора</param>
        /// <returns>ряд Тейлора</returns>
        public static TaylorSeries operator +(TaylorSeries ts) {
            return ts;
        }


        /// <summary>
        /// Вычитание рядов Тейлора
        /// </summary>
        /// <param name="t1">ряд Тейлора 1</param>
        /// <param name="t2">ряд Тейлора 2</param>
        /// <returns>результат вычитания</returns>
        /// <exception cref="Exception">если x0 у рядов не равны</exception>
        public static TaylorSeries operator -(TaylorSeries t1, TaylorSeries t2) {
            if (t1.X0 != t2.X0) throw new Exception("x0 должны быть равны");
            int n = Math.Max(t1.N, t2.N);
            string function = $"{t1.Function} - ${t2.Function}";
            double[] coefs = new double[n + 1];
            for (int i = 0; i < t1.Coefs.Length; i++) coefs[i] += t1.Coefs[i];
            for (int i = 0; i < t2.Coefs.Length; i++) coefs[i] -= t2.Coefs[i];
            TaylorSeries res = new TaylorSeries(function, t1.X0, n, false);
            res.coefs = coefs;
            return res;
        }

        /// <summary>
        /// Переопределение унарного оператора "-"
        /// </summary>
        /// <param name="ts">ряд Тейлора</param>
        /// <returns>ряд Тейлора</returns>
        public static TaylorSeries operator -(TaylorSeries ts) {
            double[] coefs = new double[ts.N + 1];
            for (int i = 0; i < ts.Coefs.Length; i++) coefs[i] -= ts.Coefs[i];
            TaylorSeries res = new TaylorSeries(ts.Function, ts.X0, ts.N, false);
            res.coefs = coefs;
            return res;
        }

        /// <summary>
        /// Найти первый коэфициент ряда
        /// </summary>
        /// <returns></returns>
        public double FindFirst() {
            string pol = Interpreter.GetPolishExpression(function);
            double res = Interpreter.SolvePolishExpression(pol, new Dictionary<char, double>() { { 'x', x0 } });
            return res;
        }

        /// <summary>
        /// Найти все коэфициенты ряда
        /// </summary>
        public void FindCoefs() {
            coefs[0] = FindFirst();

            Expr x = Expr.Variable("x");
            Expr prevDerivative = new Expr(Infix.Parse(function).ResultValue);
            for (int i = 1; i < n + 1; i++) {
                Expr derivative = prevDerivative.Differentiate(x);
                string pol = Interpreter.GetPolishExpression(derivative.ToString());
                coefs[i] = Interpreter.SolvePolishExpression(pol, new Dictionary<char, double>() { { 'x', x0 } });
                double factorial = 1;
                for (int j = 2; j <= i; j++) factorial *= j;
                coefs[i] /= factorial;
                prevDerivative = derivative;
            }
        }

        /// <summary>
        /// Дифференцировать ряд Тейлора
        /// </summary>
        /// <returns>производная исходного ряда Тейлора</returns>
        public TaylorSeries GetDerivative() {
            double[] newCoefs = new double[n + 1];
            for (int i = 1; i < n + 1; i++) {
                newCoefs[i - 1] = coefs[i] * i;
            }
            TaylorSeries newTaylorSeries;
            if (n != 0) newTaylorSeries = new TaylorSeries($"d({function})/dx", x0, n - 1, false);
            else newTaylorSeries = new TaylorSeries($"d({function})/dx", x0, n, false);
            newTaylorSeries.coefs = newCoefs;
            return newTaylorSeries;
        }

        /// <summary>
        /// Проинтегрировать ряд Тейлора
        /// </summary>
        /// <returns>проинтегрированный исходный ряд Тейлора</returns>
        public TaylorSeries GetIntegral() {
            double[] newCoefs = new double[n + 2];
            for (int i = 0; i < n + 1; i++) {
                newCoefs[i + 1] = coefs[i] / (i + 1);
            }
            TaylorSeries newTaylorSeries = new TaylorSeries($"∫({function})dx", x0, n + 1, false);
            newTaylorSeries.coefs = newCoefs;
            return newTaylorSeries;
        }

        /// <summary>
        /// Высчитывание ряда Тейлора в опредёлнной точке
        /// </summary>
        /// <param name="x">значения точки</param>
        /// <returns>значения ряда Тейлора</returns>
        public double Compute(double x) {
            double res = coefs[0];

            for (int i = 1; i < n + 1; i++) {
                double coef = coefs[i];
                coef *= Math.Pow(x - x0, i);
                res += coef;
            }

            return res;
        }

        /// <summary>
        /// Перевод ряда Тейлора в строковый вид
        /// </summary>
        /// <returns>строковый вид ряда Тейлора</returns>
        public override string ToString() {
            string res = coefs[0].ToString();
            bool hasFirst = true;
            if (coefs[0] == 0) {
                res = "";
                hasFirst = false;
            }

            string x = "x";
            if (x0 != 0) x = $"(x{(x0 >= 0 ? "-" : "+")}{Math.Abs(x0)})";

            for (int i = 1; i < n + 1; i++) {
                double coef = coefs[i];
                if (coef == 0) continue;
                if (!hasFirst) {
                    res += $"{(coef >= 0 ? "" : "-")}{(Math.Abs(coef) == 1 ? "" : $"{Math.Abs(coef)}*")}{x}{(i > 1 ? $"^{i}" : "")}";
                    hasFirst = true;
                    continue;
                };
                res += $"{(coef >= 0 ? " + " : " - ")}{(Math.Abs(coef) == 1 ? "" : $"{Math.Abs(coef)}*")}{x}{(i > 1 ? $"^{i}" : "")}";
            }

            return res;
        }
    }
}
