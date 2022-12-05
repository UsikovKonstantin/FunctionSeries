using System.Windows;

namespace WpfFunctionSeries;

public partial class Help : Window
{
    public Help()
    {
        InitializeComponent();
        Block.Text = "Функция должна содержать переменную x\n" +
                     "Допустимые функции: sin, cos, tan, ctan, arcsin, arccos, arctan, arcctan, log, ln, sqrt\n" +
                     "Допустимые операторы: ^ + - / * !\n" +
                     "Функции записываются: функция()\n" +
                     "Функция не может содержать переменных кроме x";
    }
}