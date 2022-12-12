using System;
using System.Collections.Generic;

namespace FunctionSeriesClassLibrary {
    public static class Interpreter {
        // Метод возвращает обратную польскую запись
        public static string GetPolishExpression(string input) {
            input = input.Trim();
            string output = "";
            Stack<string> operStack = new Stack<string>();

            int i = 0;
            bool holdMinus = false;
            bool isPrevElOper = false;
            if (input[0] == '-') {
                holdMinus = true;
                i = 1;
            }
            for (; i < input.Length; i++) {
                if (IsDelimeter(input[i])) continue;

                if (IsOperator(input[i])) {
                    if (input[i] == '(')
                        operStack.Push(input[i].ToString());
                    else if (input[i] == ')') {
                        // Выписываем все операторы до открывающей скобки в строку
                        string s = operStack.Pop();

                        while (s != "(") {
                            output += s.ToString() + ' ';
                            s = operStack.Pop();
                        }
                    } else {
                        if (!isPrevElOper) {
                            while (operStack.Count > 0 && GetPriority(input[i].ToString()) <= GetPriority(operStack.Peek().ToString()))
                                output += operStack.Pop().ToString() + " ";
                        }

                        if (input[i] == '-' && isPrevElOper) holdMinus = true;
                        else operStack.Push(input[i].ToString());

                    }
                    if (input[i] != ')' && input[i] != '!') isPrevElOper = true;
                } else if (IsFunction(i, input)) {
                    HashSet<string> possibleFuncs = new HashSet<string>() { "sin", "cos", "tan", "ctan", "arcsin", "arccos", "arctan", "arcctan", "log", "ln", "sqrt", "√", "-sin", "-cos", "-tan", "-ctan", "-arcsin", "-arccos", "-arctan", "-arcctan", "-log", "-ln", "-sqrt", "-√" };
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
                } else if (char.IsDigit(input[i]) || char.IsLetter(input[i])) {
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
            }

            // Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";

            return output;
        }

        public static double SolvePolishExpression(string s, Dictionary<char, double> variables) {
            Stack<double> values = new Stack<double>();
            string[] polishNotation = ConvertVarsToNums(s.Trim().Split(' '), variables);
            foreach (string cur in polishNotation) {
                if (!IsFunction(0, cur)) {
                    if (cur.Length == 1 && IsOperator(char.Parse(cur))) {
                        //  Вычисление значений в зависимости от операции
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

        // Есть ли переменные в выражении
        public static bool IsVarInPolExpression(string s) {
            string[] polishNotation = GetPolishExpression(s).Trim().Split(' ');

            for (int i = 0; i < polishNotation.Length; i++) {
                // Если текущий элемент - переменная
                bool isConst = polishNotation[i] == "e" || polishNotation[i] == "π" || polishNotation[i] == "-e" || polishNotation[i] == "-π";
                bool isVar = polishNotation[i].Length == 1 && char.IsLetter(char.Parse(polishNotation[i]));
                bool isVarWithMinus = polishNotation[i].Length == 2 && polishNotation[i][0] == '-' && char.IsLetter(polishNotation[i][1]);
                if (isVar || isVarWithMinus && !isConst) return true;
            }

            return false;
        }

        public static string GetLexems(string s) {
            string res = "";
            for (int i = 0; i < s.Length; i++) {
                if (IsDelimeter(s[i])) continue;

                string type = "";
                string value = "";
                string priority = "";
                if (IsOperator(s[i])) {
                    if (s[i] == '(') type = "left_bracket";
                    else if (s[i] == ')') type = "right_bracket";
                    else type = "operation_sign";

                    value = s[i].ToString();
                    priority = GetPriority(value).ToString();
                } else if (IsFunction(i, s)) {
                    type = "function";

                    HashSet<string> possibleFuncs = new HashSet<string>() { "sin", "cos", "tan", "ctan", "arcsin", "arccos", "arctan", "arcctan", "log", "ln", "sqrt", "√", "-sin", "-cos", "-tan", "-ctan", "-arcsin", "-arccos", "-arctan", "-arcctan", "-log", "-ln", "-sqrt", "-√" };
                    value = "";
                    // Читаем до разделителя или оператора, чтобы получить число
                    while (!IsDelimeter(s[i]) && !IsOperator(s[i])) {
                        value += s[i];
                        i++;

                        if (possibleFuncs.Contains(value)) break;
                        // Если символ - последний, то выходим из цикла
                        if (i == s.Length) break;
                    }
                    if (!possibleFuncs.Contains(value)) throw new Exception("Выражение введено не верно");

                    priority = GetPriority(value).ToString();
                    i--;
                } else if (char.IsDigit(s[i]) || char.IsLetter(s[i])) {
                    if (char.IsDigit(s[i]) || s[i] == 'e' || s[i] == 'π') type = "number";
                    else if (char.IsLetter(s[i])) type = "indetificator";

                    value = "";
                    // Читаем до разделителя или оператора, чтобы получить число
                    while (!IsDelimeter(s[i]) && !IsOperator(s[i])) {
                        value += s[i];
                        i++;

                        // Если символ - последний, то выходим из цикла
                        if (i == s.Length) break;
                    }

                    i--;
                }
                res += $"type={type}; value={value}; {(priority != "" ? "priority=" + priority : "")}\n";
            }
            return res;
        }

        // Метод возвращает true, если проверяемый символ - пробел
        private static bool IsDelimeter(char c) {
            if ((" ".IndexOf(c) != -1))
                return true;
            return false;
        }

        // Метод возвращает true, если проверяемый символ - оператор
        private static bool IsOperator(char с) {
            if (("+-/÷:*×^()!".IndexOf(с) != -1))
                return true;
            return false;
        }

        // Метод возвращает true, если проверяемый символ - функция
        private static bool IsFunction(int idx, string s) {
            bool isFunction = idx <= s.Length - 2 && char.IsLetter(s[idx]) && char.IsLetter(s[idx + 1]);
            bool isFunctionWithMinus = idx <= s.Length - 3 && s[idx] == '-' && char.IsLetter(s[idx + 1]) && char.IsLetter(s[idx + 2]);
            bool isSqrt = '√' == s[idx] || (idx <= s.Length - 2 && '-' == s[idx] && '√' == s[idx + 1]);
            if (isFunction || isFunctionWithMinus || isSqrt)
                return true;
            return false;
        }

        // Метод возвращает приоритет оператора
        private static int GetPriority(string s) {
            if (s.Length > 1 || s == "√" || s == "!") return 6;
            switch (s) {
                case "(": return 0;
                case ")": return 1;
                case "+": return 2;
                case "-": return 2;
                case "*": return 3;
                case "/": return 3;
                case "^": return 4;
                default: return 5;
            }
        }

        // Выполнение операций
        private static double ComputeOperation(string oper, Stack<double> values) {
            if (oper == "+") {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return firstVal + secondVal;
            } else if (oper == "-") {
                double firstVal = values.Pop();
                if (values.Count > 0) {
                    double secondVal = values.Pop();
                    return secondVal - firstVal;
                } else {
                    return -firstVal;
                }
            } else if (oper == "*" || oper == "×") {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return secondVal * firstVal;
            } else if (oper == "/" || oper == ":" || oper == "÷") {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return secondVal / firstVal;
            } else if (oper == "^") {
                double firstVal = values.Pop();
                double secondVal = values.Pop();
                return Math.Pow(secondVal, firstVal);
            } else if (oper == "!") {
                double firstVal = values.Pop();
                double res = 1;
                for (int i = 2; i <= firstVal; i++) {
                    res *= i;
                }
                return res;
            } else if (oper == "log" || oper == "-log") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Log10(firstVal);
                return Math.Log10(firstVal);
            } else if (oper == "ln" || oper == "-ln") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Log(firstVal);
                return Math.Log(firstVal);
            } else if (oper == "sin" || oper == "-sin") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Sin(firstVal);
                return Math.Sin(firstVal);
            } else if (oper == "cos" || oper == "-cos") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Cos(firstVal);
                return Math.Cos(firstVal);
            } else if (oper == "tan" || oper == "-tan") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Tan(firstVal);
                return Math.Tan(firstVal);
            } else if (oper == "ctan" || oper == "-ctan") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -1 / Math.Tan(firstVal);
                return 1 / Math.Tan(firstVal);
            } else if (oper == "arcsin" || oper == "-arcsin") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Asin(firstVal);
                return Math.Asin(firstVal);
            } else if (oper == "arccos" || oper == "-arccos") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Acos(firstVal);
                return Math.Acos(firstVal);
            } else if (oper == "arctan" || oper == "-arctan") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Atan(firstVal);
                return Math.Atan(firstVal);
            } else if (oper == "arcctan" || oper == "-arcctan") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -1 / Math.Atan(firstVal);
                return 1 / Math.Atan(firstVal);
            } else if (oper == "sqrt" || oper == "√" || oper == "-sqrt" || oper == "-√") {
                double firstVal = values.Pop();
                if (oper[0] == '-') return -Math.Sqrt(firstVal);
                return Math.Sqrt(firstVal);
            } else {
                throw new Exception("Несуществующая функция или оператор");
            }
        }

        // Заменить переменные на числа
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
