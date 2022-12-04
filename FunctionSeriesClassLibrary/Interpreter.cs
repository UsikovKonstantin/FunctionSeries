using System;
using System.Collections.Generic;

namespace FunctionSeriesClassLibrary
{
    public static class Interpreter
    {
        // Метод возвращает обратную польскую запись
        public static string GetPolishExpression(string input)
        {
            string output = "";
            Stack<string> operStack = new Stack<string>();

            for (int i = 0; i < input.Length; i++)
            {
                if (IsDelimeter(input[i])) continue;

                if (IsOperator(input[i]))
                {
                    if (input[i] == '(')
                        operStack.Push(input[i].ToString());
                    else if (input[i] == ')')
                    {
                        // Выписываем все операторы до открывающей скобки в строку
                        string s = operStack.Pop();

                        while (s != "(")
                        {
                            output += s.ToString() + ' ';
                            s = operStack.Pop();
                        }
                    }
                    else
                    {
                        while (operStack.Count > 0 && GetPriority(input[i].ToString()) <= GetPriority(operStack.Peek().ToString()))
                            output += operStack.Pop().ToString() + " ";

                        operStack.Push(input[i].ToString());
                    }
                }
                else if (IsFunction(i, input))
                {
                    HashSet<string> possibleFuncs = new HashSet<string>() { "sin", "cos", "tan", "ctan", "arcsin", "arccos", "arctan", "arcctan", "log", "ln", "sqrt", "√" };
                    string func = "";
                    // Читаем до разделителя или оператора, чтобы получить число
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        func += input[i];
                        i++;

                        if (possibleFuncs.Contains(func)) break;
                        // Если символ - последний, то выходим из цикла
                        if (i == input.Length) break;
                    }
                    if (!possibleFuncs.Contains(func)) throw new Exception("Выражение введено не верно");
                    operStack.Push(func);

                    i--;
                }
                else if (char.IsDigit(input[i]) || char.IsLetter(input[i]))
                {
                    // Читаем до разделителя или оператора, чтобы получить число
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        output += input[i];
                        i++;

                        // Если символ - последний, то выходим из цикла
                        if (i == input.Length) break;
                    }

                    output += ' ';
                    i--;
                }
            }

            // Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";

            return output;
        }

        public static double SolvePolishExpression(string s, Dictionary<char, double> variables)
        {
            Stack<double> values = new Stack<double>();
            string[] polishNotation = ConvertVarsToNums(s.Trim().Split(' '), variables);
            foreach (string cur in polishNotation)
            {
                if (!IsFunction(0, cur))
                {
                    if (cur.Length == 1 && IsOperator(char.Parse(cur)))
                    {
                        //  Вычисление значений в зависимости от операции
                        double result = ComputeOperation(cur, values);
                        values.Push(result);
                    }
                    else values.Push(double.Parse(cur));
                }
                else
                {
                    //  Вычисление значений в зависимости от операции
                    double result = ComputeOperation(cur, values);
                    values.Push(result);
                }
            }

            return values.Pop();
        }

        // Есть ли переменные в выражении
        public static bool IsVarInPolExpression(string s)
        {
            string[] polishNotation = GetPolishExpression(s).Trim().Split(' ');

            for (int i = 0; i < polishNotation.Length; i++)
            {
                // Если текущий элемент - переменная
                bool isConst = polishNotation[i] == "e" || polishNotation[i] == "π";
                if (polishNotation[i].Length == 1 && char.IsLetter(char.Parse(polishNotation[i])) && !isConst) return true;
            }

            return false;
        }

        // Получить все переменные в выражении
        public static List<char> GetVarsFromExpression(string s)
        {
            List<char> res = new List<char>();

            string[] polishNotation = GetPolishExpression(s).Trim().Split(' ');
            for (int i = 0; i < polishNotation.Length; i++)
            {
                // Если текущий элемент - переменная
                bool isConst = polishNotation[i] == "e" || polishNotation[i] == "π";
                if (polishNotation[i].Length == 1 && char.IsLetter(char.Parse(polishNotation[i])) && !isConst) res.Add(char.Parse(polishNotation[i]));
            }

            return res;
        }

        // Метод возвращает true, если проверяемый символ - пробел
        private static bool IsDelimeter(char c)
        {
            if ((" ".IndexOf(c) != -1))
                return true;
            return false;
        }

        // Метод возвращает true, если проверяемый символ - оператор
        private static bool IsOperator(char с)
        {
            if (("+-/÷:*×^()!".IndexOf(с) != -1))
                return true;
            return false;
        }

        // Метод возвращает true, если проверяемый символ - функция
        private static bool IsFunction(int idx, string s)
        {
            bool isFunction = char.IsLetter(s[idx]) && idx <= s.Length - 2 && char.IsLetter(s[idx + 1]);
            bool isSqrt = '√' == s[idx];
            if (isFunction || isSqrt)
                return true;
            return false;
        }

        // Метод возвращает приоритет оператора
        private static int GetPriority(string s)
        {
            if (s.Length > 1 || s == "√" || s == "!") return 7;
            switch (s)
            {
                case "(": return 0;
                case ")": return 1;
                case "+": return 2;
                case "-": return 3;
                case "*": return 4;
                case "/": return 4;
                case "^": return 5;
                default: return 8;
            }
        }

        // Выполнение операций
        private static double ComputeOperation(string oper, Stack<double> values)
        {
            if (oper == "+")
            {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return firstVal + secondVal;
            }
            else if (oper == "-")
            {
                double firstVal = values.Pop();
                if (values.Count > 0)
                {
                    double secondVal = values.Pop();
                    return secondVal - firstVal;
                }
                else
                {
                    return -firstVal;
                }
            }
            else if (oper == "*" || oper == "×")
            {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return secondVal * firstVal;
            }
            else if (oper == "/" || oper == ":" || oper == "÷")
            {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return secondVal / firstVal;
            }
            else if (oper == "^")
            {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return Math.Pow(secondVal, firstVal);
            }
            else if (oper == "!")
            {
                double firstVal = values.Pop();
                double res = 1;
                for (int i = 2; i <= firstVal; i++)
                {
                    res *= i;
                }
                return res;
            }
            else if (oper == "log")
            {
                double firstVal = values.Pop();
                return Math.Log10(firstVal);
            }
            else if (oper == "ln")
            {
                double firstVal = values.Pop();
                return Math.Log(firstVal);
            }
            else if (oper == "sin")
            {
                double firstVal = values.Pop();
                return Math.Sin(firstVal);
            }
            else if (oper == "cos")
            {
                double firstVal = values.Pop();
                return Math.Cos(firstVal);
            }
            else if (oper == "tan")
            {
                double firstVal = values.Pop();
                return Math.Tan(firstVal);
            }
            else if (oper == "ctan")
            {
                double firstVal = values.Pop();
                return 1 / Math.Tan(firstVal);
            }
            else if (oper == "arcsin")
            {
                double firstVal = values.Pop();
                return Math.Asin(firstVal);
            }
            else if (oper == "arccos")
            {
                double firstVal = values.Pop();
                return Math.Acos(firstVal);
            }
            else if (oper == "arctan")
            {
                double firstVal = values.Pop();
                return Math.Atan(firstVal);
            }
            else if (oper == "arcctan")
            {
                double firstVal = values.Pop();
                return 1 / Math.Atan(firstVal);
            }
            else if (oper == "sqrt" || oper == "√")
            {
                double firstVal = values.Pop();
                return Math.Sqrt(firstVal);
            }
            else
            {
                throw new Exception("Несуществующая функция или оператор");
            }
        }

        // Заменить переменные на числа
        private static string[] ConvertVarsToNums(string[] polishNotation, Dictionary<char, double> variables)
        {
            for (int i = 0; i < polishNotation.Length; i++)
            {
                // Если текущий элемент - переменная
                if (polishNotation[i].Length == 1 && char.IsLetter(char.Parse(polishNotation[i])))
                {
                    if (polishNotation[i] == "π")
                    {
                        polishNotation[i] = $"{Math.PI}";
                    }
                    else if (polishNotation[i] == "e")
                    {
                        polishNotation[i] = $"{Math.Exp(1)}";
                    }
                    else
                    {
                        // Достаём число для переменной, если его нет выкидывает ошибку
                        bool hasVar = variables.TryGetValue(char.Parse(polishNotation[i]), out double num);
                        if (!hasVar) throw new ApplicationException($"Переменной '{polishNotation[i]}' нету");
                        polishNotation[i] = $"{num}";
                    }
                }
            }

            return polishNotation;
        }
    }
}
