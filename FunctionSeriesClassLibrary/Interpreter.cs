using System;
using System.Collections.Generic;

namespace FunctionSeriesClassLibrary {
    public static class Interpreter {
        /// <summary>
        /// Возвращает польскую запись в строчном виде
        /// </summary>
        /// <param name="input">фунция в строчном виде</param>
        /// <returns>польская запись</returns>
        public static string GetPolishExpression(string input) {
            input = input.Trim();
            string output = "";
            Stack<string> operStack = new Stack<string>();

            bool holdMinus = false;
            bool isPrevElOper = true;
            for (int i = 0; i < input.Length; i++) {
                if (IsDelimeter(input[i])) continue;

                if (IsOperator(input[i])) HandleOperator(input[i], operStack, ref output, ref isPrevElOper, ref holdMinus);
                else if (IsFunction(i, input)) HandleFunction(input, ref i, operStack, ref isPrevElOper, ref holdMinus);
                else if (IsValue(input[i])) HandleValue(input, ref i, ref output, ref isPrevElOper, ref holdMinus);
            }

            // Когда прошли по всем символам, выкидываем из стека все оставшиеся там операции в строку
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";

            return output;
        }

        /// <summary>
        /// Решает польскую запись
        /// </summary>
        /// <param name="s">польская запись</param>
        /// <param name="variables">переменные, необходимые для решения польской записи</param>
        /// <returns>число</returns>
        public static double SolvePolishExpression(string s, Dictionary<char, double> variables) {
            Stack<double> values = new Stack<double>();
            string[] polishNotation = ConvertVarsToNums(s.Trim().Split(' '), variables);
            foreach (string cur in polishNotation) {
                if (!IsFunction(0, cur)) {
                    if (cur.Length == 1 && IsOperator(char.Parse(cur))) {
                        // Вычисление значений в зависимости от операции
                        double result = ComputeOperation(cur, values);
                        values.Push(result);
                    } else values.Push(double.Parse(cur));
                } else {
                    //  Вычисление значений в зависимости от операции
                    double result = ComputeOperation(cur, values);
                    values.Push(result);
                }
            }

            return values.Pop();
        }

        /// <summary>
        /// Проверяет если симовол является разделителем
        /// </summary>
        /// <param name="c">проверяемый символ</param>
        /// <returns>True - если символ разделитель, иначе - False</returns>
        private static bool IsDelimeter(char c) {
            if ((" ".IndexOf(c) != -1))
                return true;
            return false;
        }

        /// <summary>
        /// Проверяет если симовол является оператором
        /// </summary>
        /// <param name="c">проверяемый символ</param>
        /// <returns>True - если символ оператор, иначе - False</returns>
        private static bool IsOperator(char с) {
            if (("+-/÷:*×^()!_".IndexOf(с) != -1))
                return true;
            return false;
        }

        /// <summary>
        /// Проверяет если подстрока является функцией
        /// </summary>
        /// <param name="idx">индекс с которого нужно начать</param>
        /// <param name="s">исходная строка</param>
        /// <returns>True - если подстрока фукнция, иначе - False</returns>
        private static bool IsFunction(int idx, string s) {
            bool isFunction = idx <= s.Length - 2 && char.IsLetter(s[idx]) && char.IsLetter(s[idx + 1]);
            bool isFunctionWithMinus = idx <= s.Length - 3 && s[idx] == '-' && char.IsLetter(s[idx + 1]) && char.IsLetter(s[idx + 2]);
            bool isSqrt = '√' == s[idx] || (idx <= s.Length - 2 && '-' == s[idx] && '√' == s[idx + 1]);
            if (isFunction || isFunctionWithMinus || isSqrt)
                return true;
            return false;
        }

        /// <summary>
        /// Проверяет если символ значение(число/переменная)
        /// </summary>
        /// <param name="c">проверяемый символ</param>
        /// <returns>True - если символ значение, иначе - False</returns>
        private static bool IsValue(char c) {
            if (char.IsDigit(c) || char.IsLetter(c))
                return true;
            return false;
        }

        /// <summary>
        /// Метод ответственный за работу с оператороми при переводе функции в польскую запись
        /// </summary>
        /// <param name="oper">оператор</param>
        /// <param name="operStack">стэк операций</param>
        /// <param name="output">результат - польская запись</param>
        /// <param name="isPrevElOper">показывает если предыдущий элемент оператор, необходимо для работы с минусами</param>
        /// <param name="holdMinus">устанавливает если нужно применить минус к числу/переменной/функции/перед скобкой</param>
        private static void HandleOperator(char oper, Stack<string> operStack, ref string output, ref bool isPrevElOper, ref bool holdMinus) {
            if (oper == '(') {
                operStack.Push((holdMinus ? "-" : "") + oper.ToString());
                holdMinus = false;
            } else if (oper == ')') {
                // Выписываем все операторы до открывающей скобки в строку
                string s = operStack.Pop();

                while (s != "(" && s != "-(") {
                    output += s.ToString() + ' ';
                    s = operStack.Pop();
                }
                if (s == "-(") operStack.Push("_");
            } else {
                if (!isPrevElOper) {
                    while (operStack.Count > 0 && GetPriority(oper.ToString()) <= GetPriority(operStack.Peek().ToString()))
                        output += operStack.Pop().ToString() + " ";
                }

                if (oper == '-' && isPrevElOper) holdMinus = true;
                else operStack.Push(oper.ToString());

            }
            if (oper != ')' && oper != '!') isPrevElOper = true;
        }

        /// <summary>
        /// Метод ответственный за работу с функциями при переводе функции в польскую запись
        /// </summary>
        /// <param name="input">исходная строка</param>
        /// <param name="i">индекс с которого нужно начать</param>
        /// <param name="operStack">стэк операций</param>
        /// <param name="isPrevElOper">показывает если предыдущий элемент оператор, необходимо для работы с минусами</param>
        /// <param name="holdMinus">устанавливает если нужно применить минус к числу/переменной/функции/перед скобкой</param>
        /// <exception cref="Exception">если функция не существует</exception>
        private static void HandleFunction(string input, ref int i, Stack<string> operStack, ref bool isPrevElOper, ref bool holdMinus) {
            HashSet<string> possibleFuncs = new HashSet<string>() { "sin", "cos", "tan", "ctan", "asin", "acos", "atan", "actan", "log", "ln", "sqrt", "√", "-sin", "-cos", "-tan", "-ctan", "-asin", "-acos", "-atan", "-actan", "-log", "-ln", "-sqrt", "-√" };
            string func = "";
            if (holdMinus) {
                func += "-";
                holdMinus = false;
            }
            // Читаем до разделителя или оператора, чтобы получить число
            while (!IsDelimeter(input[i]) && !IsOperator(input[i])) {
                func += input[i];
                i++;

                if (possibleFuncs.Contains(func)) break;
                // Если символ - последний, то выходим из цикла
                if (i == input.Length) break;
            }
            if (!possibleFuncs.Contains(func)) throw new Exception("Выражение введено не верно");
            operStack.Push(func);

            i--;
            isPrevElOper = false;
        }

        /// <summary>
        /// Метод ответственный за работу с значения при переводе функции в польскую запись
        /// </summary>
        /// <param name="input">исходная строка</param>
        /// <param name="i">индекс с которого нужно начать</param>
        /// <param name="output">результат - польская запись</param>
        /// <param name="isPrevElOper">показывает если предыдущий элемент оператор, необходимо для работы с минусами</param>
        /// <param name="holdMinus">устанавливает если нужно применить минус к числу/переменной/функции/перед скобкой</param>
        private static void HandleValue(string input, ref int i, ref string output, ref bool isPrevElOper, ref bool holdMinus) {
            if (holdMinus) {
                output += "-";
                holdMinus = false;
            }
            // Читаем до разделителя или оператора, чтобы получить число
            while (!IsDelimeter(input[i]) && !IsOperator(input[i])) {
                output += input[i];
                i++;

                // Если символ - последний, то выходим из цикла
                if (i == input.Length) break;
            }

            output += ' ';
            i--;
            isPrevElOper = false;
        }

        /// <summary>
        /// Возвращает приоритет оператора/функции/скобки
        /// </summary>
        /// <param name="s">проверяемый оператор/функция/скобка</param>
        /// <returns>приоритет в виде числа</returns>
        private static int GetPriority(string s) {
            switch (s) {
                case "(": return 0;
                case "-(": return 0;
                case ")": return 1;
                case "+": return 2;
                case "_": return 2;
                case "-": return 2;
                case "*": return 3;
                case "/": return 3;
                case "^": return 4;
                default: return 5;
            }
        }

        /// <summary>
        /// Вычисление операции при решение польской записи
        /// </summary>
        /// <param name="oper">операция</param>
        /// <param name="values">стэк значений</param>
        /// <returns>результат вычисления операции</returns>
        /// <exception cref="Exception">если данная операция не существует</exception>
        private static double ComputeOperation(string oper, Stack<double> values) {
            switch (oper) {
                case "+": {
                        double firstVal = values.Pop();
                        double secondVal = values.Pop();
                        return firstVal + secondVal;
                    }
                case "-": {
                        double firstVal = values.Pop();
                        if (values.Count > 0) {
                            double secondVal = values.Pop();
                            return secondVal - firstVal;
                        } else {
                            return -firstVal;
                        }
                    }
                case "_": {
                        double firstVal = values.Pop();
                        return -firstVal;
                    }
                case "*":
                case "×": {
                        double firstVal = values.Pop();
                        double secondVal = values.Pop();
                        return secondVal * firstVal;
                    }
                case "/":
                case ":":
                case "÷": {
                        double firstVal = values.Pop();
                        double secondVal = values.Pop();
                        return secondVal / firstVal;
                    }
                case "^": {
                        double firstVal = values.Pop();
                        double secondVal = values.Pop();
                        return Math.Pow(secondVal, firstVal);
                    }
                case "!": {
                        double firstVal = values.Pop();
                        double res = 1;
                        for (int i = 2; i <= firstVal; i++) {
                            res *= i;
                        }
                        return res;
                    }
                case "log":
                case "-log": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Log10(firstVal);
                        return Math.Log10(firstVal);
                    }
                case "ln":
                case "-ln": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Log(firstVal);
                        return Math.Log(firstVal);
                    }
                case "sin":
                case "-sin": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Sin(firstVal);
                        return Math.Sin(firstVal);
                    }
                case "cos":
                case "-cos": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Cos(firstVal);
                        return Math.Cos(firstVal);
                    }
                case "tan":
                case "-tan": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Tan(firstVal);
                        return Math.Tan(firstVal);
                    }
                case "ctan":
                case "-ctan": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -1 / Math.Tan(firstVal);
                        return 1 / Math.Tan(firstVal);
                    }
                case "asin":
                case "-asin": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Asin(firstVal);
                        return Math.Asin(firstVal);
                    }
                case "acos":
                case "-acos": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Acos(firstVal);
                        return Math.Acos(firstVal);
                    }
                case "atan":
                case "-atan": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Atan(firstVal);
                        return Math.Atan(firstVal);
                    }
                case "actan":
                case "-actan": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -1 / Math.Atan(firstVal);
                        return 1 / Math.Atan(firstVal);
                    }
                case "sqrt":
                case "√":
                case "-sqrt":
                case "-√": {
                        double firstVal = values.Pop();
                        if (oper[0] == '-') return -Math.Sqrt(firstVal);
                        return Math.Sqrt(firstVal);
                    }
                default: throw new Exception("Несуществующая функция или оператор");
            }
        }

        /// <summary>
        /// Заменяет перменные на числа в польской записи
        /// </summary>
        /// <param name="polishNotation">польская запись</param>
        /// <param name="variables">переменные и соответствующее число для этой переменной</param>
        /// <returns>польская запись, вместо переменных - числа</returns>
        /// <exception cref="ApplicationException">если переменной в польской записи нет в словаре variables</exception>
        private static string[] ConvertVarsToNums(string[] polishNotation, Dictionary<char, double> variables) {
            for (int i = 0; i < polishNotation.Length; i++) {
                // Если текущий элемент - переменная
                bool isVar = polishNotation[i].Length == 1 && char.IsLetter(char.Parse(polishNotation[i]));
                bool isVarWithMinus = polishNotation[i].Length == 2 && polishNotation[i][0] == '-' && char.IsLetter(polishNotation[i][1]);
                if (isVar || isVarWithMinus) {
                    if (polishNotation[i] == "π" || polishNotation[i] == "-π") {
                        if (polishNotation[i].Length == 1) polishNotation[i] = $"{Math.PI}";
                        else polishNotation[i] = $"-{Math.PI}";
                    } else if (polishNotation[i] == "e" || polishNotation[i] == "-e") {
                        if (polishNotation[i].Length == 1) polishNotation[i] = $"{Math.Exp(1)}";
                        else polishNotation[i] = $"-{Math.Exp(1)}";
                    } else {
                        if (polishNotation[i].Length == 1) {
                            bool hasVar = variables.TryGetValue(char.Parse(polishNotation[i]), out double num);
                            if (!hasVar) throw new ApplicationException($"Переменной '{polishNotation[i]}' нету");
                            polishNotation[i] = $"{num}";
                        } else {
                            bool hasVar = variables.TryGetValue(polishNotation[i][1], out double num);
                            if (!hasVar) throw new ApplicationException($"Переменной '{polishNotation[i][1]}' нету");
                            polishNotation[i] = $"-{num}";
                        }
                    }
                }
            }

            return polishNotation;
        }
    }
}
