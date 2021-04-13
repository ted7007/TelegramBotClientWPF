using System;
using System.Collections.Generic;
using System.IO;
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


namespace TelegramBotOnWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TGBotMessageClient botClient;
        public MainWindow()
        {
            InitializeComponent();
            string debugpath = AppDomain.CurrentDomain.BaseDirectory;
            
            string path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(
                          System.IO.Path.GetDirectoryName(debugpath)));
            string token = File.ReadAllText($@"{path}\token.txt");
            if (token.Length<5)
            {
                GetToken();
                
            }
            else
            {
                InitBot(token);
            }



           

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UsersBox.SelectedItem == null||SendText.Text==""||botClient==null)
                return;
            User user = UsersBox.SelectedItem as User;
            botClient.SendMessage(SendText.Text, Convert.ToInt64(user.ChatId));
            SendText.Text = "";
        }
        
        /// <summary>
        /// Метод получения токена
        /// </summary>
        private void GetToken()
        {
            GetTokenWindow w = new GetTokenWindow();
            if(w.ShowDialog()==true)
            {
                InitBot(w.TokenText.Text);
            }
            else if(w.DialogResult==false)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Инициализация бота
        /// </summary>
        /// <param name="token"></param>
       private void InitBot(string token)
       {
            try
            {
                botClient = new TGBotMessageClient(this, token);
                UsersBox.ItemsSource = botClient.Users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Токен не верный.", "Ошибка токена", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
