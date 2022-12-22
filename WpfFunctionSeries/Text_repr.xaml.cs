using System;
using System.Windows;
using FunctionSeriesClassLibrary;

namespace WpfFunctionSeries;

public partial class Text_repr : Window
{
    public Text_repr( FourierApprox fa)
    {
        InitializeComponent();
        Block.Text = $"Частоты:{fa}";
    }
    public Text_repr( FourierSeries fs)
    {
        InitializeComponent();
        Block.Text = $"{fs}";
    }
    public Text_repr( TaylorSeries ts)
    {
        InitializeComponent();
        Block.Text = $"{ts}";
    }

    private void Text_repr_OnClosed(object? sender, EventArgs e)
    {
        if (Approx.window_count != 0)
        {
            Approx.window_count -= 1;
        }
    }
}