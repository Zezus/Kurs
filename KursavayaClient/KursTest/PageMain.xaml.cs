using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {

        TcpClient client;
        NetworkStream stream;
        bool flag = true;
        Methods method = new Methods();

        public PageMain()
        {
            InitializeComponent();
        }

        public bool TestDate(string log, string pas)
        {
            if (log == "admin" && pas == "admin")
            {

            }
            else
            {
                int min = 6;
                int max = 10;
                if (log != "" && pas != "")
                {
                    if (log.IndexOf(' ') == -1 && pas.IndexOf(' ') == -1)
                    {
                        if (log.Length >= min && pas.Length >= min)
                        {
                            if (log.Length <= max && pas.Length <= max)
                            {

                            }
                            else
                            {
                                flag = false;
                                MessageBox.Show("Максимальная длина ввода 10 символов", "Error", MessageBoxButton.OK);
                            }
                        }
                        else
                        {
                            flag = false;
                            MessageBox.Show("Минимальная длина ввода 6 символов", "Error", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        flag = false;
                        MessageBox.Show("Нельзя вводить пробелы", "Error", MessageBoxButton.OK);

                    }
                }
                else
                {
                    flag = false;
                    MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK);
                }
            }
            return flag;
        }

        private void butLogin_Click(object sender, RoutedEventArgs e)
        {
            TestDate(tbLogin.Text, tbPassword.Password.ToString());
            if (flag == true)
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
                stream = client.GetStream();

                //отправляем тип активной страницы
                string typePage = this.GetType().ToString();
                method.TypePage(stream, typePage);

                //отправляем данные с входа
                string userData = tbLogin.Text + " " + tbPassword.Password.ToString();
                method.Send(stream, userData);

                byte[] otvetRegistra = new byte[2];
                stream.Read(otvetRegistra, 0, otvetRegistra.Length);
                string yes_no = Encoding.Unicode.GetString(otvetRegistra);
                if (yes_no == "0")
                {
                    if (tbLogin.Text == "admin" && tbPassword.Password.ToString() == "admin")
                    {
                        ((ContentControl)(this.Parent)).Content = new PageAdmin();
                        MessageBox.Show("Авторизация прошла успешно, теперь вы можете войти в чат", "Goog Job", MessageBoxButton.OK);
                    }
                    else
                    {
                        ((ContentControl)(this.Parent)).Content = new PageChat();
                        MessageBox.Show("Авторизация прошла успешно, теперь вы можете войти в чат", "Goog Job", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Error", MessageBoxButton.OK);
                    tbPassword.Clear();
                }

                // ((ContentControl)(this.Parent)).Content = new PageChat();

            }
        }
        private void butRegistr_Click(object sender, RoutedEventArgs e)
        {
            ((ContentControl)(this.Parent)).Content = new PageRegistr();

        }

        private void ButtonRememberPas(object sender, RoutedEventArgs e)
        {
            ((ContentControl)(this.Parent)).Content = new Remind_password();
        }
    }
}
