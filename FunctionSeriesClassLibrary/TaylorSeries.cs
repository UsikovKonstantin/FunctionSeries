using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace FunctionSeriesClassLibrary {
    public class TaylorSeries {
        /// <summary>
        /// Заданная функция
        /// </summary>
        public string Function { get; }
        /// <summary>
        /// x0
        /// </summary>
        public double X0 { get; }
        /// <summary>
        /// Коэффициенты
        /// </summary>
        public double[] Coefs { get; }
        /// <summary>
        /// Кол-во коэффициентов в ряде
        /// </summary>
        public int N { get; }


        public TaylorSeries(string function, double x0, int n) {
            if (n < 0) throw new Exception("Кол-во коэффициентов в ряду должно быть больше нуля");
            Function = function;
            X0 = x0;
            Coefs = new double[n + 1];
            N = n;
            FindCoefs();
        }

        /// <summary>
        /// Найти первый коэфициент ряда
        /// </summary>
        /// <returns></returns>
        public double FindFirst() {
            string pol = Interpreter.GetPolishExpression(Function);
            double res = Interpreter.SolvePolishExpression(pol, new Dictionary<char, double>() { { 'x', X0 } });
            return res;
        }

        /// <summary>
        /// Найти все коэфициенты ряда
        /// </summary>
        public void FindCoefs() {
            Coefs[0] = FindFirst();

            Expr x = Expr.Variable("x");
            Expr prevDerivative = new Expr(Infix.Parse(Function).ResultValue);
            for (int i = 1; i < N + 1; i++) {
                Expr derivative = prevDerivative.Differentiate(x);
                string pol = Interpreter.GetPolishExpression(derivative.ToString());
                Coefs[i] = Interpreter.SolvePolishExpression(pol, new Dictionary<char, double>() { { 'x', X0 } });
                int factorial = 1;
                for (int j = 2; j <= i; j++) factorial *= j;
                Coefs[i] /= factorial;
                prevDerivative = derivative;
            }
        }

        /// <summary>
        /// Высчитывание ряда Тейлора в опредёлнной точке
        /// </summary>
        /// <param name="x">значения точки</param>
        /// <returns>значения ряда Тейлора</returns>
        public double Compute(double x) {
            double res = Coefs[0];

            for (int i = 1; i < N + 1; i++) {
                double coef = Coefs[i];
                coef *= Math.Pow(x - X0, i);
                res += coef;
            }

            return res;
        }

        /// <summary>
        /// Перевод ряда Тейлора в строковый вид
        /// </summary>
        /// <returns>строковый вид ряда Тейлора</returns>
        public override string ToString() {
            string res = Coefs[0].ToString();
            bool hasFirst = true;
            if (Coefs[0] == 0) {
                res = "";
                hasFirst = false;
            }

            string x = "x";
            if (X0 != 0) x = $"(x{(X0 >= 0 ? "-" : "+")}{Math.Abs(X0)})";

            for (int i = 1; i < N + 1; i++) {
                double coef = Coefs[i];
                if (coef == 0) continue;
                if (!hasFirst) {
                    res += $"{(Math.Abs(coef) == 1 ? "" : $"{coef}*")}{x}{(i > 1 ? $"^{i}" : "")}";
                    hasFirst = true;
                    continue;
                };
                res += $"{(coef >= 0 ? " + " : " - ")}{(Math.Abs(coef) == 1 ? "" : $"{Math.Abs(coef)}*")}{x}{(i > 1 ? $"^{i}" : "")}";
            }

            return res;
        }
    }
}
