using System.Numerics;
using System.Text;

namespace FunctionSeriesClassLibrary;

/// <summary>
/// Класс для аппроксимации ряда Фурье по полю точек
/// </summary>
public class FourierApprox
{
    private List<(Complex Coef, int freq)> Frequencies = new(); // Частоты
    private int num_points = 0; // Количество точек в аппроксимации
    /// <summary>
    /// Получение ряда Фурье посредством поля точек (Тип 2)
    /// </summary>
    /// <param name="point_cloud">Поле точек</param>
    public FourierApprox(List<(double x, double y)> point_cloud)
    {
        num_points = point_cloud.Count;
        NDFT_Coefficients(point_cloud);
    }

    void NDFT_Coefficients(List<(double x, double y)> point_cloud)
    {
        for (int i = 0; i < num_points; i++)
        {
            Complex Coef = Get_Specific(point_cloud, i);
            Frequencies.Add((Coef,i));
        }

        Frequencies.Sort((x, y) => -x.Coef.Magnitude.CompareTo(y.Coef.Magnitude));
    }

    Complex Get_Specific(List<(double x, double y)> point_cloud, int frequency)
    {
        Complex Coef = new Complex(0, 0);
        Complex modex_constant = 2 * Math.PI * Complex.ImaginaryOne * frequency;
        foreach (var point in point_cloud)
        {
            Coef += point.y * Complex.Pow(new (Math.E, 0), modex_constant * point.x);
        }
        
        return Coef;
    }
    
    public double Compute(double x, int precision)
    {
        Complex result = new (0, 0);
        Complex modex_constant = 2 * Math.PI * Complex.ImaginaryOne;
        for (int i = 0; i < precision; i++)
        {
            result += Frequencies[i].Coef.Magnitude * Complex.Pow(new (Math.E, 0), modex_constant * x * Frequencies[i].freq);
        }
        
        double res = result.Phase;
        return res;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        foreach (var frequency in Frequencies)
        {
            builder.Append($"{frequency.freq}:{frequency.Coef}, ");
        }
        return builder.ToString();
    }
}