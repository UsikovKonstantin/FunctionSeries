using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FunctionSeriesClassLibrary;
using ScottPlot;

namespace WpfFunctionSeries;

public partial class Approx : Window
{
    public Approx()
    {
        InitializeComponent();
        var cur_lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.AddPoint(0, 0);
        W_Plot.Plot.SetAxisLimits(cur_lim);
    }

    private int prev_scroll_value = 5;

    private void Scr_Pow_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized) return;
        if (Math.Abs(Math.Round(Scr_Pow.Value) - prev_scroll_value) > 0.9)
        {
            prev_scroll_value = (int)Math.Round(Scr_Pow.Value);
            Tx_Terms_Input.Text = prev_scroll_value.ToString();
        }
    }

    private void Bt_Text_OnClick(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        new Text_repr(int.Parse(Tx_Terms_Input.Text), 3.14, "x", FourierSeriesType.CosSin).Show();
    }

    private void Tx_Terms_Input_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!IsInitialized) return;
        List<char> symbols = new(Tx_Terms_Input.Text);
        for (var index = 0; index < symbols.Count; index++)
        {
            var chars = symbols[index];
            if (chars < '0' || chars > '9')
            {
                symbols.RemoveAt(index);
                index--;
            }
        }

        string corr = new(symbols.ToArray());
        if (corr.Length == 0) corr = "5";
        if (corr == "0") corr = "1";
        Tx_Terms_Input.Text = corr;
        if (int.Parse(corr) < 101) Scr_Pow.Value = int.Parse(corr);
        Calculate();
    }

    private List<(double x, double y)> point_cloud = new();

    private void W_Plot_OnMouseEnter(object sender, MouseEventArgs e)
    {
        W_Plot.Focusable = true;
        W_Plot.Focus();
    }

    private void W_Plot_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!IsInitialized) return;
        while (e.IsDown && e.Key != Key.D)
        {
            if (e.Key == Key.S)
            {
                var point = W_Plot.GetMouseCoordinates();
                foreach (var points in point_cloud)
                    if (points.x == point.x && points.y == point.y)
                        return;
                point_cloud.Add(point);
                Calculate();
            }
        }
        if (e.Key == Key.D && point_cloud.Count > 0)
        {
            var coord = W_Plot.GetMouseCoordinates();
            double min_dist = double.MaxValue;
            int min = 0;
            for (int i = 0; i < point_cloud.Count; i++)
            {
                double dist = Math.Sqrt(Math.Pow(point_cloud[i].x - coord.x, 2) +
                                        Math.Pow(point_cloud[i].y - coord.y, 2));
                if (min_dist > dist)
                {
                    min_dist = dist;
                    min = i;
                }
            }
            point_cloud.RemoveAt(min);
            Calculate();
        }
    }

    void Calculate()
    {
        var cur_lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.Clear();
        foreach (var point in point_cloud)
        {
            W_Plot.Plot.AddPoint(point.x, point.y, Color.Blue);
        }

        W_Plot.Plot.SetAxisLimits(cur_lim);
        int precision = Math.Min(point_cloud.Count, int.Parse(Tx_Terms_Input.Text));
        FourierSeries fs = new(point_cloud, precision);
        W_Plot.Plot.AddFunction((x) => fs.Compute(x));
        W_Plot.Refresh();
    }
}