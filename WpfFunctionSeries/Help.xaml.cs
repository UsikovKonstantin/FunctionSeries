using System.Windows;

namespace WpfFunctionSeries;

public partial class Help : Window
{
    public Help()
    {
        InitializeComponent();
        Block.Text = "Функция должна содержать переменную x\n" +
                     "Допустимые функции: sin, cos, tan, ctan, arcsin, arccos, atan, actan, log, ln, sqrt,\n" +
                     "sinh, cosh\n" +
                     "Допустимые операторы: ^ + - / * !\n" +
                     "Функции записываются: функция()\n" +
                     "Можно записать функцию без переменных\n" +
                     "Функция не может содержать переменных кроме x\n";
    }
}