using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FunctionSeriesClassLibrary;
using ScottPlot;

namespace WpfFunctionSeries;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int prev_scroll_value = 5;

    private int prev_scroll_value_Tay = 5;
    private bool taylor_empty = true;

    private TaylorSeries taylor_saved;

    public MainWindow()
    {
        InitializeComponent();
        Set_Up_UI();
        Tx_Per_Input.Text = (2 * Math.PI).ToString();
    }

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

    private void Set_Up_UI()
    {
        if (Rb_Fourier.IsChecked.Value)
        {
            GBox_Add_fun.Header = "Период функции";
            var t = (Grid)GBox_Add_fun.Parent;
            Tx_Per_Input.Text = (2 * Math.PI).ToString();
            t.ColumnDefinitions[1].Width = new GridLength(170);
            Grid_sub.RowDefinitions[2].Height = new GridLength(70);
            Grid_sub.RowDefinitions[3].Height = new GridLength(70);
            Grid_sub.RowDefinitions[4].Height = new GridLength(0);
            Grid_main.RowDefinitions[0].Height = new GridLength(320);
        }
        else if (Rb_Taylor.IsChecked.Value)
        {
            GBox_Add_fun.Header = "Коэффинциент разложения";
            var t = (Grid)GBox_Add_fun.Parent;
            Tx_Per_Input.Text = "0";
            t.ColumnDefinitions[1].Width = new GridLength(270);
            Grid_sub.RowDefinitions[2].Height = new GridLength(0);
            Grid_sub.RowDefinitions[3].Height = new GridLength(0);
            Grid_sub.RowDefinitions[4].Height = new GridLength(70);
            Grid_main.RowDefinitions[0].Height = new GridLength(210);
        }
    }

    private async void General_Calc(object sender, RoutedEventArgs e)
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
        catch (Exception exception)
        {
        }

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
            var cts = new CancellationTokenSource();
            var ts = Task.Run(() =>
            {
                var ds = Dispatcher;
                string fun_text = null;
                ds.Invoke(() => fun_text = Tx_Fun_Input.Text);
                double x0 = 0;
                ds.Invoke(() => x0 = double.Parse(Tx_Per_Input.Text));
                var n = 5;
                ds.Invoke(() => n = int.Parse(Tx_Tay_Terms_Input.Text));
                return new TaylorSeries(fun_text, x0, n, ct: cts.Token);
            }, cts.Token);
            var sw = Stopwatch.StartNew();
            var success = false;
            var tas = Task.Run(() => success = ts.Wait(1000));
            await tas;
            if (success)
            {
                Scr_Tay_Terms.Background = Brushes.White;
                Scr_Tay_Terms.ToolTip = "";
                Update_Plot(ts.Result);
            }
            else
            {
                cts.Cancel();
                Scr_Tay_Terms.Background = Brushes.Red;
                Scr_Tay_Terms.ToolTip = "Слишком высокая степень";
            }
        }
    }

    private void set_Fun_type()
    {
        var type = FunctionHelper.GetType(Tx_Fun_Input.Text);
        switch (type)
        {
            case FunctionType.Even:
                Rb_Cos.IsChecked = true;
                break;
            case FunctionType.Odd:
                Rb_Sin.IsChecked = true;
                break;
            case FunctionType.Neither:
                Rb_Asym.IsChecked = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private double solve_at(double x, string polish)
    {
        return Interpreter.SolvePolishExpression(polish, new Dictionary<char, double> { { 'x', x } });
    }

    private void checks_terms_taylor()
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

    private void checks_x0()
    {
        // Проверка периода функции
        List<char> symbols = new(Tx_Per_Input.Text);
        // Удаление недопустимых символов
        for (var index = 0; index < symbols.Count; index++)
        {
            var chars = symbols[index];
            if ((chars < '0' || chars > '9') &&
                chars.ToString() != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator && chars != '-')
            {
                symbols.RemoveAt(index);
                index--;
            }
        }

        // Максимум одна точка
        var dot_counter = 0;
        for (var index = 0; index < symbols.Count; index++)
        {
            var chars = symbols[index];
            if (chars.ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            {
                dot_counter++;
                if (dot_counter >= 2)
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }
        }

        var minus_counter = 0;
        if (symbols[0] != '-') minus_counter = 1;
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
        if (corr.Length == 0) corr = "0";
        Tx_Per_Input.Text = corr;
    }

    private void Update_Plot(FourierSeries fs)
    {
        // Работа с графиком
        W_Plot.Plot.Clear();
        var pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
        W_Plot.Plot.AddFunction(x => call_fun(x, pol));
        W_Plot.Plot.AddFunction(x => fs.Compute(x));
        // постановка более удобных границ графика
        var max_x = double.Parse(Tx_Per_Input.Text);
        var step = max_x * 2 / 1000;
        double min_y = double.MaxValue, max_y = double.MinValue;
        for (var i = -max_x; i <= max_x; i += step)
        {
            var val = fs.Compute(i);
            var val2 = call_fun(i, pol);
            min_y = Math.Min(val, min_y);
            max_y = Math.Max(val, max_y);
            min_y = Math.Min(val2, min_y);
            max_y = Math.Max(val2, max_y);
        }

        var aver = (max_y + min_y) / 2;
        var diff = Math.Abs(aver - max_y) * 1.1;
        if (diff < 0.000001) diff = 1;
        W_Plot.Plot.SetAxisLimits(-max_x * 3, max_x * 3, aver - diff, aver + diff);
        W_Plot.Refresh();
    }

    private void Update_Plot(TaylorSeries fs)
    {
        // Работа с графиком
        var lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.Clear();
        if (!taylor_empty && Tx_Fun_Input.Text == taylor_saved.Function) W_Plot.Plot.SetAxisLimits(lim);
        var pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
        W_Plot.Plot.AddFunction(
            x => Interpreter.SolvePolishExpression(pol, new Dictionary<char, double> { { 'x', x } }));
        // постановка более удобных границ графика
        double max_x = 5;
        var step = max_x * 2 / 1000;
        double min_y = double.MaxValue, max_y = double.MinValue;
        for (var i = -max_x; i <= max_x; i += step)
        {
            var val = fs.Compute(i);
            var val2 = Interpreter.SolvePolishExpression(pol, new Dictionary<char, double> { { 'x', i } });
            min_y = Math.Min(val, min_y);
            max_y = Math.Max(val, max_y);
            min_y = Math.Min(val2, min_y);
            max_y = Math.Max(val2, max_y);
        }

        var aver = (max_y + min_y) / 2;
        var diff = Math.Abs(aver - max_y) * 1.1;
        if (diff < 0.000001) diff = 1;
        W_Plot.Plot.SetAxisLimits(-max_x, max_x, aver - diff, aver + diff);
        var t = cloud_at_range(fs);
        List<double> xs = new(), ys = new();
        for (var i = 0; i < t.Count; i++)
        {
            xs.Add(t[i].x);
            ys.Add(t[i].y);
        }

        W_Plot.Plot.AddScatter(xs.ToArray(), ys.ToArray(), markerShape: MarkerShape.none);
        if (!taylor_empty && Tx_Fun_Input.Text == taylor_saved.Function) W_Plot.Plot.SetAxisLimits(lim);
        taylor_saved = fs;
        taylor_empty = false;
        W_Plot_OnAxesChanged(new object(), new EventArgs());
        W_Plot.Refresh();
    }

    private List<(double x, double y)> cloud_at_range(TaylorSeries ts)
    {
        List<(double x, double y)> cloud = new();
        double min_x = W_Plot.Plot.GetAxisLimits().XMin, max_x = W_Plot.Plot.GetAxisLimits().XMax;
        var aver = (min_x + max_x) / 2;
        var diff = Math.Abs(aver - min_x);
        var multiplier = 1.3;
        min_x = aver - diff * multiplier;
        max_x = aver + diff * multiplier;
        double min_y = W_Plot.Plot.GetAxisLimits().YMin - 1, max_y = W_Plot.Plot.GetAxisLimits().YMax + 1;
        aver = (min_y + max_y) / 2;
        diff = Math.Abs(aver - min_y);
        min_y = aver - diff * multiplier;
        max_y = aver + diff * multiplier;
        var step = (max_x - min_x) / 1000;
        for (var i = min_x; i <= max_x; i += step)
        {
            var val = ts.Compute(i);
            val = Math.Min(Math.Max(min_y, val), max_y);
            if (double.IsNaN(val))
            {
                if (ts.N % 2 == 0) val = max_y;
                else val = min_y;
            }

            cloud.Add((i, val));
        }

        return cloud;
    }

    private double call_fun(double x, string polish)
    {
        // Вызывает функцию таким образом, как если бы она была периодической
        var period = double.Parse(Tx_Per_Input.Text);
        if (Rb_Asym.IsChecked.Value) x -= Math.Round(x / period) * period;
        if (Rb_Sin.IsChecked.Value)
        {
            x -= Math.Round(x / period) * period;
            if (x < 0) return -Interpreter.SolvePolishExpression(polish, new Dictionary<char, double> { { 'x', -x } });
        }

        if (Rb_Cos.IsChecked.Value)
        {
            x -= Math.Round(x / period) * period;
            x = Math.Abs(x);
        }

        return Interpreter.SolvePolishExpression(polish, new Dictionary<char, double> { { 'x', x } });
    }

    private void checks_terms()
    {
        // Проверка количества коэфициентов
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

    private void checks_period()
    {
        // Проверка периода функции
        List<char> symbols = new(Tx_Per_Input.Text);
        // Удаление недопустимых символов
        for (var index = 0; index < symbols.Count; index++)
        {
            var chars = symbols[index];
            if ((chars < '0' || chars > '9') &&
                chars.ToString() != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            {
                symbols.RemoveAt(index);
                index--;
            }
        }

        // Максимум одна точка
        var dot_counter = 0;
        for (var index = 0; index < symbols.Count; index++)
        {
            var chars = symbols[index];
            if (chars.ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
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
        if (corr.Length == 0) corr = $"3{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}14";
        if (corr == "0") corr = $"3{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}14";
        Tx_Per_Input.Text = corr;
    }

    private bool checks_Fun()
    {
        // Проверка функции
        try
        {
            Dictionary<char, double> t = new();
            t.Add('x', 0.5);
            var function = convert_to_interpret(Tx_Fun_Input.Text);
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
    private string convert_to_interpret(string function)
    {
        return function.Replace("sqrt", "√");
    }

    private void Bt_Help_OnClick(object sender, RoutedEventArgs e)
    {
        // Вызов окна со справкой
        if (!IsInitialized) return;
        new Help().Show();
    }

    private void Bt_Approx_OnClick(object sender, RoutedEventArgs e)
    {
        // Вызов окна аппроксимации
        if (!IsInitialized) return;
        new Approx().Show();
    }

    private void Bt_Text_OnClick(object sender, RoutedEventArgs e)
    {
        // Вызов окна с функцией и соотвествующим ей рядом
        if (!IsInitialized) return;
        if (!checks_Fun()) return;
        if (Rb_Fourier.IsChecked.Value)
        {
            if (Rb_Sin.IsChecked.Value)
                new Text_repr(new FourierSeries(int.Parse(Tx_Terms_Input.Text), double.Parse(Tx_Per_Input.Text),
                    Tx_Fun_Input.Text, FourierSeriesType.Sin)).Show();
            if (Rb_Cos.IsChecked.Value)
                new Text_repr(new FourierSeries(int.Parse(Tx_Terms_Input.Text), double.Parse(Tx_Per_Input.Text),
                    Tx_Fun_Input.Text, FourierSeriesType.Cos)).Show();
            if (Rb_Asym.IsChecked.Value)
                new Text_repr(new FourierSeries(int.Parse(Tx_Terms_Input.Text), double.Parse(Tx_Per_Input.Text),
                    Tx_Fun_Input.Text, FourierSeriesType.CosSin)).Show();
        }
        else if (Rb_Taylor.IsChecked.Value)
        {
            new Text_repr(new TaylorSeries(Tx_Fun_Input.Text, double.Parse(Tx_Per_Input.Text),
                    int.Parse(Tx_Tay_Terms_Input.Text)))
                .Show();
        }
    }

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

    private void W_Plot_OnAxesChanged(object? sender, EventArgs e)
    {
        if (!IsInitialized) return;
        if (!Rb_Taylor.IsChecked.Value || taylor_empty) return;
        var lim = W_Plot.Plot.GetAxisLimits();
        W_Plot.Plot.Clear();
        W_Plot.Plot.SetAxisLimits(lim);
        var pol = Interpreter.GetPolishExpression(Tx_Fun_Input.Text);
        W_Plot.Plot.AddFunction(
            x => Interpreter.SolvePolishExpression(pol, new Dictionary<char, double> { { 'x', x } }));
        var t = cloud_at_range(taylor_saved);
        List<double> xs = new(), ys = new();
        for (var i = 0; i < t.Count; i++)
        {
            xs.Add(t[i].x);
            ys.Add(t[i].y);
        }

        W_Plot.Plot.AddScatter(xs.ToArray(), ys.ToArray(), markerShape: MarkerShape.none);
        W_Plot.Plot.SetAxisLimits(lim);
    }
}