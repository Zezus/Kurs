using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain
    {

        TcpClient _client;
        NetworkStream _stream;
        bool _flag = true;
        Methods method = new Methods();


        public PageMain()
        {
            InitializeComponent();
        }

        public bool TestDate(string log, string pas)
        {
            if (log == "admin")
            {
                int min = 1;
                int max = 16;
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
                                _flag = false;
                                MessageBox.Show("Максимальная длина ввода 16 символов", "Error", MessageBoxButton.OK);
                            }
                        }
                        else
                        {
                            _flag = false;
                            MessageBox.Show("Минимальная длина ввода 1 символов", "Error", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        _flag = false;
                        MessageBox.Show("Нельзя вводить пробелы", "Error", MessageBoxButton.OK);

                    }
                }
                else
                {
                    _flag = false;
                    MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                int min = 6;
                int max = 16;
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
                                _flag = false;
                                MessageBox.Show("Максимальная длина ввода 16 символов", "Error", MessageBoxButton.OK);
                            }
                        }
                        else
                        {
                            _flag = false;
                            MessageBox.Show("Минимальная длина ввода 6 символов", "Error", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        _flag = false;
                        MessageBox.Show("Нельзя вводить пробелы", "Error", MessageBoxButton.OK);

                    }
                }
                else
                {
                    _flag = false;
                    MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK);
                }
            }
            return _flag;
        }

        private void butLogin_Click(object sender, RoutedEventArgs e)
        {
            TestDate(tbLogin.Text, tbPassword.Password);
            if (_flag)
            {
                _client = new TcpClient();
                _client.Connect(new IPEndPoint(IPAddress.Parse(tbIP.Text), 3084));
                _stream = _client.GetStream();

                //отправляем тип активной страницы
                string typePage = GetType().ToString();
                method.TypePage(_stream, typePage);

                //отправляем данные с входа
                string userData = tbLogin.Text + " " + tbPassword.Password;
                method.Send(_stream, userData);

                byte[] otvetRegistra = new byte[2];
                _stream.Read(otvetRegistra, 0, otvetRegistra.Length);
                string yesNo = Encoding.Unicode.GetString(otvetRegistra);
                if (yesNo == "0")
                {
                    if (tbLogin.Text == "admin")
                    {
                        ((ContentControl)Parent).Content = new PageAdmin();
                        MessageBox.Show("Авторизация прошла успешно, теперь вы можете войти в чат", "Goog Job", MessageBoxButton.OK);
                    }
                    else
                    {
                        ((ContentControl)Parent).Content = new PageChat();
                        MessageBox.Show("Авторизация прошла успешно, теперь вы можете войти в чат", "Goog Job", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Error", MessageBoxButton.OK);
                    tbPassword.Clear();
                }
            }
        }
        private void butRegistr_Click(object sender, RoutedEventArgs e)
        {
            ((ContentControl)Parent).Content = new PageRegistr();

        }

        private void ButtonRememberPas(object sender, RoutedEventArgs e)
        {
            ((ContentControl)Parent).Content = new RemindPassword();
        }
    }
}
