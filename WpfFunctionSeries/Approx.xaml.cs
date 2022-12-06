using System.Windows;
using System.Windows.Input;

namespace WpfFunctionSeries;

public partial class Approx : Window
{
    public Approx()
    {
        InitializeComponent();
    }

    private void Tx_Terms_Input_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!IsInitialized) return;
    }

    private void Scr_Pow_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized) return;
    }

    private void Bt_Text_OnClick(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
    }
}