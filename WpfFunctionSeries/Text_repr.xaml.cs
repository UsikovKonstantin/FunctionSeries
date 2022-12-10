using System;
using System.Windows;
using FunctionSeriesClassLibrary;

namespace WpfFunctionSeries;

public partial class Text_repr : Window
{
    public enum Text_type
    {
        function,
        approx
    }
    public Text_repr(Text_type type, FourierSeries fs = null, FourierApprox fa = null)
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
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
    }
}