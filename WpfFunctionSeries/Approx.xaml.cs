using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FunctionSeriesClassLibrary;

namespace WpfFunctionSeries;

public partial class Approx : Window
{
    private readonly List<(double x, double y)> point_cloud = new();

    private int prev_scroll_value = 5;

    public Approx()
    {
        InitializeComponent();
        var cur_lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.AddPoint(0, 0);
        W_Plot.Plot.SetAxisLimits(cur_lim);
    }

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
        if (point_cloud.Count == 0) return;
        new Text_repr(Text_repr.Text_type.approx, fa: new FourierApprox(point_cloud, int.Parse(Tx_Terms_Input.Text)))
            .Show();
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

    private void W_Plot_OnMouseEnter(object sender, MouseEventArgs e)
    {
        W_Plot.Focusable = true;
        W_Plot.Focus();
    }

    private void W_Plot_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!IsInitialized) return;
        if (e.Key == Key.S)
        {
            var point = W_Plot.GetMouseCoordinates();
            foreach (var points in point_cloud)
                if (points.x == point.x && points.y == point.y)
                    return;
            point_cloud.Add(point);
            Calculate();
        }

        if (e.Key == Key.D && point_cloud.Count > 0)
        {
            var coord = W_Plot.GetMouseCoordinates();
            var min_dist = double.MaxValue;
            var min = 0;
            for (var i = 0; i < point_cloud.Count; i++)
            {
                var dist = Math.Sqrt(Math.Pow(point_cloud[i].x - coord.x, 2) +
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

    private void Calculate()
    {
        var cur_lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.Clear();
        foreach (var point in point_cloud) W_Plot.Plot.AddPoint(point.x, point.y, Color.Blue);
        W_Plot.Plot.SetAxisLimits(cur_lim);
        if (point_cloud.Count != 0)
        {
            var precision = int.Parse(Tx_Terms_Input.Text);
            FourierApprox fs = new(point_cloud, precision);
            W_Plot.Plot.AddFunction(x => fs.Compute(x), Color.Orange);
        }

        W_Plot.Refresh();
    }
}