namespace FunctionSeriesClassLibrary
{
    /// <summary>
    /// Перечисление "Тип функции".
    /// Even - четная функция
    /// Odd - нечетная функция
    /// Neither - функция общего вида
    /// </summary>
    public enum FunctionType
    {
        Even,
        Odd,
        Neither
    }

    /// <summary>
    /// Класс для определения четности функции.
    /// </summary>
    public static class FunctionHelper
    {
        static Random rnd = new Random();  // генератор случайных чисел
        const int STEPS = 1000;  // количество проверок
        const int CHECK_RADIUS = 1000;  // область проверки
        const double EPS = 1e-5;  // возможная погрешность

        /// <summary>
        /// Возвращает тип функции.
        /// </summary>
        /// <param name="function"> строковое представление функции </param>
        /// <returns> тип функции </returns>
        public static FunctionType GetType(string function)
        {
            string polFunc = Interpreter.GetPolishExpression(function);
            bool isEven = true, isOdd = true;
            for (int i = 1; i < STEPS; i++)
            {
                double x = rnd.NextDouble() * CHECK_RADIUS;
                double y1 = Interpreter.SolvePolishExpression(polFunc, new Dictionary<char, double>() { { 'x', x } });
                double y2 = Interpreter.SolvePolishExpression(polFunc, new Dictionary<char, double>() { { 'x', -x } });
                isEven = isEven && (Math.Abs(y1 - y2) < EPS);
                isOdd = isOdd && (Math.Abs(y1 + y2) < EPS);
            }
            if (isEven)
                return FunctionType.Even;
            else if (isOdd)
                return FunctionType.Odd;
            else
                return FunctionType.Neither;
        }
    }
}
