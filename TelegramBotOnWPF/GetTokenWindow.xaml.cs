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
using System.Windows.Shapes;

namespace TelegramBotOnWPF
{
    /// <summary>
    /// Логика взаимодействия для GetTokenWindow.xaml
    /// </summary>
    public partial class GetTokenWindow : Window
    {
        public GetTokenWindow()
        {
            InitializeComponent();
        }

        private void SaveToken_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
