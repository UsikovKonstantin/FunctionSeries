using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfFunctionSeries
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int prev_scroll_value = 5;
        private void Scr_Pow_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized) return;
            if (Math.Abs(Math.Round(Scr_Pow.Value) - prev_scroll_value) > 0.1)
            {
                prev_scroll_value = (int)Math.Round(Scr_Pow.Value);
                Tx_Terms_Input.Text = prev_scroll_value.ToString();
                General_Calc(sender, e);
            }
            
        }
        
        private void General_Calc(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            checks_terms();
        }

        void checks_terms()
        {
            
        }
        private void Bt_Help_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
        }

        private void Bt_Approx_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
        }

        private void Bt_Text_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
        }
    }
}