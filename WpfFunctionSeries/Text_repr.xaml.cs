using System;
using System.Windows;
using FunctionSeriesClassLibrary;

namespace WpfFunctionSeries;

public partial class Text_repr : Window
{
    public enum Text_type
    {
        function,
        approx,
        taylor
    }

    public Text_repr(Text_type type, FourierSeries fs = null, FourierApprox fa = null, TaylorSeries ts = null)
    {
        InitializeComponent();
        switch (type)
        {
            case Text_type.function:
                Block.Text = $"{fs}";
                break;
            case Text_type.approx:
                Block.Text = $"Частоты:{fa}";
                break;
            case Text_type.taylor:
                Block.Text = $"{ts}";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}