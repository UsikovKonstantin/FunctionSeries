﻿using System;
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
        }

        private int prev_scroll_value = 5;
        private void Scr_Pow_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized) return;
            if (Math.Abs(Math.Round(Scr_Pow.Value) - prev_scroll_value) > 0.9)
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
            checks_period();
            if (!checks_Fun()) return;
                
        }

        void checks_terms()
        {
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
            Tx_Terms_Input.Text = corr;
            if (int.Parse(corr) < 101) Scr_Pow.Value = int.Parse(corr);
        }

        void checks_period()
        {
            List<char> symbols = new(Tx_Per_Input.Text);
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if ((chars < '0' || chars > '9') && chars != '.')
                {
                    symbols.RemoveAt(index);
                    index--;
                }
            }

            int dot_counter = 0;
            for (var index = 0; index < symbols.Count; index++)
            {
                var chars = symbols[index];
                if (chars == '.')
                {
                    dot_counter++;
                    if (dot_counter >= 2)
                    {
                        symbols.RemoveAt(index);
                        index--;
                    }
                }
            }
            string corr = new(symbols.ToArray());
            if (corr.Length == 0) corr = "3.14";
            Tx_Per_Input.Text = corr;
        }

        bool checks_Fun()
        {
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

        string convert_to_interpret(string function)
        {
            return function.Replace("sqrt", "√");
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