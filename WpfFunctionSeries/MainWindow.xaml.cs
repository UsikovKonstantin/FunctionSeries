using System;
using System.Collections.Generic;
using System.Globalization;
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
using FunctionSeriesClassLibrary;

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
            Set_Up_UI();
        }

        private int prev_scroll_value = 5;
        private void Scr_Pow_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized) return;
            // Вычисления только в случае достаточного отличия от текущего значения
            if (Math.Abs(Math.Round(Scr_Pow.Value) - prev_scroll_value) > 0.9)
            {
                prev_scroll_value = (int)Math.Round(Scr_Pow.Value);
                Tx_Terms_Input.Text = prev_scroll_value.ToString();
                General_Calc(sender, e);
            }
        }

        void Set_Up_UI()
        {
            if (Rb_Fourier.IsChecked.Value)
            {
                GBox_Add_fun.Header = "Период функции";
                Grid t = (Grid)GBox_Add_fun.Parent;
                t.ColumnDefinitions[1].Width = new GridLength(110);
                Grid_sub.RowDefinitions[2].Height = new GridLength(50);
                Grid_sub.RowDefinitions[3].Height = new GridLength(50);
                Grid_sub.RowDefinitions[4].Height = new GridLength(0);
                Grid_main.RowDefinitions[0].Height = new GridLength(200);
            }
            else if (Rb_Taylor.IsChecked.Value)
            {
                GBox_Add_fun.Header = "Коэффинциент разложения";
                Grid t = (Grid)GBox_Add_fun.Parent;
                t.ColumnDefinitions[1].Width = new GridLength(200);
                Grid_sub.RowDefinitions[2].Height = new GridLength(0);
                Grid_sub.RowDefinitions[3].Height = new GridLength(0);
                Grid_sub.RowDefinitions[4].Height = new GridLength(50);
                Grid_main.RowDefinitions[0].Height = new GridLength(150);
            }
        }
        private void General_Calc(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Set_Up_UI();
            if (!checks_Fun()) return;
            try
            {
                var tx = (TextBox)sender;
                if (tx.Name == "Tx_Fun_Input")
                    set_Fun_type();
            }
            catch (Exception exception){}
            if (Rb_Fourier.IsChecked.Value)
            {
                checks_terms();
                checks_period();
                FourierSeriesType type;
                if (Rb_Cos.IsChecked.Value) type = FourierSeriesType.Cos;
                else if (Rb_Sin.IsChecked.Value) type = FourierSeriesType.Sin;
                else type = FourierSeriesType.CosSin;
                FourierSeries fs = new(int.Parse(Tx_Terms_Input.Text), double.Parse(Tx_Per_Input.Text), Tx_Fun_Input.Text,
                    type);
                Update_Plot(fs);
            }
            else if (Rb_Taylor.IsChecked.Value)
            {
                checks_terms_taylor();
                checks_x0();
                TaylorSeries ts = new(Tx_Fun_Input.Text, Double.Parse(Tx_Per_Input.Text),
                    int.Parse(Tx_Tay_Terms_Input.Text));
                Update_Plot(ts);
            }
            
        }

        void set_Fun_type()
        {
            string pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
            double TOLERANCE = 0.000001;
            if (Math.Abs(solve_at(-1,pol) - solve_at(1,pol)) < TOLERANCE) Rb_Cos.IsChecked = true;
            else if (Math.Abs(solve_at(1, pol) - (-solve_at(-1, pol))) < TOLERANCE) Rb_Sin.IsChecked = true;
            else Rb_Asym.IsChecked = true;
        }

        double solve_at(double x, string polish)
        {
            return Interpreter.SolvePolishExpression(polish, new() {{'x',x}});
        }
        void checks_terms_taylor()
        {
            // Проверка количества коэфициентов
            List<char> symbols = new(Tx_Tay_Terms_Input.Text);
            // Удаление недопустимых символов
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars < '0' || chars > '9')
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }
            //Замена текущей строки на корректную
            string corr = new(symbols.ToArray());
            if (corr.Length == 0) corr = "5";
            if (corr == "0") corr = "1";
            Tx_Tay_Terms_Input.Text = corr;
            if (int.Parse(corr) < 101) Scr_Tay_Terms.Value = int.Parse(Tx_Tay_Terms_Input.Text);
        }
        void checks_x0()
        {
            // Проверка периода функции
            List<char> symbols = new(Tx_Per_Input.Text);
            // Удаление недопустимых символов
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if ((chars < '0' || chars > '9') && chars.ToString() != System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator && chars != '-')
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }
            // Максимум одна точка
            int dot_counter = 0;
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars.ToString() == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    dot_counter++;
                    if (dot_counter >= 2)
                    {
                        symbols.RemoveAt(index);
                        index--;
                    }
                }
            }
            int minus_counter = 0;
            if (symbols[0] != '-')
            {
                minus_counter = 1;
            }
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars == '-')
                {
                    minus_counter++;
                    if (minus_counter >= 2)
                    {
                        symbols.RemoveAt(index);
                        index--;
                    }
                }
            }
            //Замена текущей строки на корректную
            string corr = new(symbols.ToArray());
            if (corr.Length == 0) corr = $"0";
            Tx_Per_Input.Text = corr;
        }
        private void Update_Plot(FourierSeries fs)
        { // Работа с графиком
            W_Plot.Plot.Clear();
            string pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
            W_Plot.Plot.AddFunction((x) => call_fun(x, pol));
            W_Plot.Plot.AddFunction((x) => fs.Compute(x));
            // постановка более удобных границ графика
            double max_x = double.Parse(Tx_Per_Input.Text);
            double step = (max_x * 2) / 1000;
            double min_y = double.MaxValue, max_y = double.MinValue;
            for (double i = -max_x; i <= max_x; i += step)
            {
                double val = fs.Compute(i);
                double val2 = call_fun(i, pol);
                min_y = Math.Min(val, min_y);
                max_y = Math.Max(val, max_y);
                min_y = Math.Min(val2, min_y);
                max_y = Math.Max(val2, max_y);
            }

            double aver = (max_y + min_y) / 2;
            double diff = Math.Abs(aver - max_y) * 1.1;
            if (diff < 0.000001) diff = 1;
            W_Plot.Plot.SetAxisLimits(-max_x * 3, max_x * 3, aver - diff, aver + diff);
            W_Plot.Refresh();
        }
        private void Update_Plot(TaylorSeries fs)
        { // Работа с графиком
            W_Plot.Plot.Clear();
            string pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
            W_Plot.Plot.AddFunction((x) => Interpreter.SolvePolishExpression(pol, new() { { 'x', x } }));
            W_Plot.Plot.AddFunction((x) => fs.Compute(x));
            // постановка более удобных границ графика
            double max_x = 5;
            double step = (max_x * 2) / 1000;
            double min_y = double.MaxValue, max_y = double.MinValue;
            for (double i = -max_x; i <= max_x; i += step)
            {
                double val = fs.Compute(i);
                double val2 = Interpreter.SolvePolishExpression(pol, new() { { 'x', i } });
                min_y = Math.Min(val, min_y);
                max_y = Math.Max(val, max_y);
                min_y = Math.Min(val2, min_y);
                max_y = Math.Max(val2, max_y);
            }

            double aver = (max_y + min_y) / 2;
            double diff = Math.Abs(aver - max_y) * 1.1;
            if (diff < 0.000001) diff = 1;
            W_Plot.Plot.SetAxisLimits(-max_x, max_x, aver - diff, aver + diff);
            W_Plot.Refresh();
        }
        double call_fun(double x, string polish)
        {// Вызывает функцию таким образом, как если бы она была периодической
            double period = Double.Parse(Tx_Per_Input.Text);
            if (Rb_Asym.IsChecked.Value) x -= (Math.Round(x / period))*period;
            if (Rb_Sin.IsChecked.Value)
            {
                x -= (Math.Round(x / period))*period;
                if (x < 0) return -Interpreter.SolvePolishExpression(polish, new() { { 'x', -x } });
            }
            if (Rb_Cos.IsChecked.Value)
            {
                x -= (Math.Round(x / period))*period;
                x = Math.Abs(x);
            }
            return Interpreter.SolvePolishExpression(polish, new() { { 'x', x } });
        }

        void checks_terms()
        {// Проверка количества коэфициентов
            List<char> symbols = new(Tx_Terms_Input.Text);
            // Удаление недопустимых символов
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars < '0' || chars > '9')
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }
            //Замена текущей строки на корректную
            string corr = new(symbols.ToArray());
            if (corr.Length == 0) corr = "5";
            if (corr == "0") corr = "1";
            Tx_Terms_Input.Text = corr;
            // перевод слайдера на новое значение
            if (int.Parse(corr) < 101) Scr_Pow.Value = int.Parse(corr);
        }

        void checks_period()
        {// Проверка периода функции
            List<char> symbols = new(Tx_Per_Input.Text);
            // Удаление недопустимых символов
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if ((chars < '0' || chars > '9') && chars.ToString() != System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }
            // Максимум одна точка
            int dot_counter = 0;
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars.ToString() == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    dot_counter++;
                    if (dot_counter >= 2)
                    {
                        symbols.RemoveAt(index);
                        index--;
                    }
                }
            }
            //Замена текущей строки на корректную
            string corr = new(symbols.ToArray());
            if (corr.Length == 0) corr = $"3{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}14";
            if (corr == "0") corr = $"3{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}14";
            Tx_Per_Input.Text = corr;
        }

        bool checks_Fun()
        {// Проверка функции
            try
            {
                Dictionary<char, double> t = new();
                t.Add('x',0.5);
                string function = convert_to_interpret(Tx_Fun_Input.Text);
                Interpreter.SolvePolishExpression(Interpreter.GetPolishExpression(function), t);
                Tx_Fun_Input.ToolTip = "";
                Tx_Fun_Input.Background = Brushes.White;
                return true;
            }
            catch (Exception)
            {
                Tx_Fun_Input.ToolTip = "Функция записана неверно.";
                Tx_Fun_Input.Background = Brushes.Red;
                return false;
            }
        }
        // Конвертация в формат понимаемый интерпретатором
        string convert_to_interpret(string function) => function.Replace("sqrt", "√");
        
        private void Bt_Help_OnClick(object sender, RoutedEventArgs e)
        {// Вызов окна со справкой
            if (!IsInitialized) return;
            new Help().Show();
        }

        private void Bt_Approx_OnClick(object sender, RoutedEventArgs e)
        {// Вызов окна аппроксимации
            if (!IsInitialized) return;
            new Approx().Show();
        }

        private void Bt_Text_OnClick(object sender, RoutedEventArgs e) 
        {// Вызов окна с функцией и соотвествующим ей рядом
            if (!IsInitialized) return;
            if (!checks_Fun()) return;
            if (Rb_Fourier.IsChecked.Value)
            {
                if (Rb_Sin.IsChecked.Value) new Text_repr(Text_repr.Text_type.function,new (int.Parse(Tx_Terms_Input.Text),Double.Parse(Tx_Per_Input.Text),Tx_Fun_Input.Text,FourierSeriesType.Sin)).Show();
                if (Rb_Cos.IsChecked.Value) new Text_repr(Text_repr.Text_type.function,new (int.Parse(Tx_Terms_Input.Text),Double.Parse(Tx_Per_Input.Text),Tx_Fun_Input.Text,FourierSeriesType.Cos)).Show();
                if (Rb_Asym.IsChecked.Value) new Text_repr(Text_repr.Text_type.function,new (int.Parse(Tx_Terms_Input.Text),Double.Parse(Tx_Per_Input.Text),Tx_Fun_Input.Text,FourierSeriesType.CosSin)).Show();    
            }
            else if (Rb_Taylor.IsChecked.Value)
            {
                new Text_repr(Text_repr.Text_type.taylor, ts:new (Tx_Fun_Input.Text,Double.Parse(Tx_Per_Input.Text),int.Parse(Tx_Per_Input.Text) )).Show();
            }
            
        }

        int prev_scroll_value_Tay = 5;
        private void Scr_Tay_Terms_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized) return;
            if (Math.Abs(Math.Round(Scr_Tay_Terms.Value) - prev_scroll_value_Tay) > 0.9)
            {
                prev_scroll_value_Tay = (int)Math.Round(Scr_Tay_Terms.Value);
                Tx_Tay_Terms_Input.Text = prev_scroll_value_Tay.ToString();
                General_Calc(sender, e);
            }
        }
    }
}