using System.Numerics;
using System.Text;

namespace FunctionSeriesClassLibrary;

/// <summary>
/// Класс для аппроксимации ряда Фурье по полю точек
/// </summary>
public class FourierApprox
{
    private List<(double x, double y)> Point_cloud; //Поле точек для аппроксимации
    private List<Complex> Frequencies = new(); // Частоты
    private int num_points = 0; // Количество точек в аппроксимации
    private double period = 0.0; // Период ряда 
    private int Frequncy_Count = 0; // Количество частот
    
    /// <summary>
    /// Получение ряда Фурье посредством поля точек (Тип 2)
    /// </summary>
    /// <param name="point_cloud">Поле точек</param>
    /// /// <param name="precision">Сколько нужно частот</param>
    public FourierApprox(List<(double x, double y)> point_cloud, int precision)
    {
        num_points = point_cloud.Count;
        Point_cloud = point_cloud;
        Frequncy_Count = precision;
        period = Get_Period();
        NDFT_Coefficients();
    }
    /// <summary>
    /// Период ряда по полю точек
    /// </summary>
    /// <returns>Период ряда</returns>
    private double Get_Period()
    {
        return Point_cloud.Max((point) => point.x) - Point_cloud.Min((point) => point.x);
    }

    /// <summary>
    /// Получение коэффициентов ряда Фурье
    /// </summary>
    void NDFT_Coefficients()
    {
        Complex Dk = -Complex.ImaginaryOne * (2 * Math.PI / period);
        for (int i = -Frequncy_Count; i <= Frequncy_Count; i++)
        {
            Frequencies.Add(Get_Specific(i,Dk));
        }
    }

    /// <summary>
    /// Получение специфической частоты в ряде
    /// </summary>
    /// <param name="i">Порядковый номер частоты</param>
    /// <param name="dk">Константа ряда</param>
    /// <returns>Частота в комплексной форме</returns>
    private Complex Get_Specific(int i, Complex dk)
    {
        Complex Coef = new(0, 0);
        foreach (var Point in Point_cloud)
        {
            Coef += Point.y * Complex.Pow(new(Math.E, 0), dk * i * Point.x);
        }

        return Coef;
    }
    
    /// <summary>
    /// Вычисление значения в точке посредством инверсного преобразования
    /// </summary>
    /// <param name="x">Координата для вычисления</param>
    /// <returns>Значение при координате</returns>
    public double Compute(double x)
    {
        Complex result = 0;
        Complex Dk = Complex.ImaginaryOne * (2 * Math.PI / period);
        int counter = 0;
        for (int i = -Frequncy_Count; i <= Frequncy_Count; i++, counter++)
        {
            result += Frequencies[counter] * Complex.Pow(new(Math.E, 0), Dk * i * x);
        }

        result /= num_points;
        return result.Real;
    }

    /// <summary>
    /// Строковое представление ряда
    /// </summary>
    /// <returns>Строка</returns>
    public override string ToString()
    {
        StringBuilder builder = new();
        foreach (var frequency in Frequencies)
        {
            builder.Append($"{frequency}, ");
        }
        return builder.ToString();
    }
}