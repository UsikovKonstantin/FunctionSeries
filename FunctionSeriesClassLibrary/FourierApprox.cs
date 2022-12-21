using System.Numerics;
using System.Text;

namespace FunctionSeriesClassLibrary;

/// <summary>
/// Класс для аппроксимации ряда Фурье по полю точек
/// </summary>
public class FourierApprox
{
    internal List<(double x, double y)> Point_cloud; //Поле точек для аппроксимации
    internal List<Complex> Frequencies = new(); // Частоты
    internal int num_points = 0; // Количество точек в аппроксимации
    internal double period = 0.0; // Период ряда 
    internal int Frequncy_Count = 0; // Количество частот

    public enum transform_type
    {
        slow,
        fast
    }
    /// <summary>
    /// Получение ряда Фурье посредством поля точек (Тип 2)
    /// </summary>
    /// <param name="point_cloud">Поле точек</param>
    /// /// <param name="precision">Сколько нужно частот</param>
    public FourierApprox(List<(double x, double y)> point_cloud, int precision, transform_type type )
    {
        num_points = point_cloud.Count;
        Point_cloud = point_cloud;
        Frequncy_Count = precision;
        period = Get_Period();
        switch (type)
        {
            case transform_type.slow:
                NDFT_Coefficients();
                break;
            case transform_type.fast:
                if (num_points <= 32)
                {
                    var t = new FourierApprox(point_cloud, precision, transform_type.slow);
                    Frequencies = t.Frequencies;
                }
                else
                {
                    var clouds = split_cloud(); 
                    var even = new FourierApprox(clouds.point_cloud_even,precision, transform_type.fast);
                    var odd = new FourierApprox(clouds.point_cloud_odd,precision, transform_type.fast);
                    for (int i = 0; i < Frequncy_Count * 4; i++)
                    {
                        Complex Factor = Complex.Pow(new(Math.E, 0),
                            -2 * Complex.ImaginaryOne * Math.PI * i / num_points);
                        Frequencies.Add(even.Frequencies[i] + (odd.Frequencies[i] * Factor));
                    }
                }
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
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
        for (int i = -Frequncy_Count; i < Frequncy_Count; i++)
        {
            Frequencies.Add(Get_Specific(i,Dk));
        }
    }

    private (List<(double x, double y)> point_cloud_even, List<(double x, double y)> point_cloud_odd) split_cloud()
    {
        List<(double x, double y)> point_cloud_even = new();
        List<(double x, double y)> point_cloud_odd = new();
        for (int i = 0; i < num_points; i++)
        {
            if (i % 2 == 0)
            {
                point_cloud_even.Add(Point_cloud[i]);
            }
            else
            {
                point_cloud_odd.Add(Point_cloud[i]);
            }
        }

        return (point_cloud_even, point_cloud_odd);
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
        for (int i = -Frequncy_Count; i < Frequncy_Count; i++, counter++)
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