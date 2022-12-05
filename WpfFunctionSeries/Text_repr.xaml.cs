using System.Windows;
using FunctionSeriesClassLibrary;

namespace WpfFunctionSeries;

public partial class Text_repr : Window
{
    public Text_repr(int n, double per, string function, FourierSeriesType type)
    {
        InitializeComponent();
        Block.Text = $"{function}={new FourierSeries(n, per, function, type)}";
    }
}