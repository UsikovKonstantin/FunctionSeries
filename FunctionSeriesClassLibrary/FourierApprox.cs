using System.Numerics;

namespace FunctionSeriesClassLibrary;

/// <summary>
/// Класс для аппроксимации ряда Фурье по полю точек
/// </summary>
public class FourierApprox
{
    private int n = 0; // количество членов
    private double l = 0; // полупериод функции
    private double[] a = new double[0]; // коэффициенты при косинусах 
    private double[] b = new double[0]; // коэффициенты при синусах

    /// <summary>
    /// Получение ряда Фурье посредством поля точек (Тип 2)
    /// </summary>
    /// <param name="point_cloud">Поле точек</param>
    /// <param name="precision">Количество коэффициентов</param>
    public FourierApprox(List<(double x, double y)> point_cloud, int precision)
    {
        n = precision;
        l = Get_Period(point_cloud);
        a = new double[n + 1];
        b = new double[n + 1];
        NDFT_Coefficients(point_cloud, precision);
    }

    double Get_Period(List<(double x, double y)> point_cloud)
    {
        double min_x = Double.MaxValue, max_x = Double.MinValue;

        foreach (var point in point_cloud)
        {
            min_x = Math.Min(min_x, point.x);
            max_x = Math.Max(max_x, point.x);
        }

        return (max_x - min_x) / 2;
    }

    void NDFT_Coefficients(List<(double x, double y)> point_cloud, int precision)
    {
        double Delta_K = (2 * Math.PI) / point_cloud.Count;
        for (int i = 0; i < precision; i++)
        {
            Complex Coef = Get_Specific(Delta_K, point_cloud, i + 1);
            a[i] = Coef.Magnitude * Math.Cos(Coef.Phase);
            b[i] = Coef.Magnitude * Math.Sin(Coef.Phase);
        }
    }

    Complex Get_Specific(double Delta_K, List<(double x, double y)> point_cloud, int frequency)
    {
        Complex Coef = new(0, 0);

        foreach (var point in point_cloud)
        {
            Coef += point.y * Complex.Pow(new(Math.E, 0), Complex.ImaginaryOne * -Delta_K * frequency * point.x);
        }

        return Coef;
    }

    public double Compute(double x)
    {
        double res = a[0] / 2;
        double k = Math.PI / l * x;
        for (int i = 1; i <= n; i++)
        {
            double c = i * k;
            res += a[i] * Math.Cos(c) + b[i] * Math.Sin(c);
        }

        return res;
    }
}