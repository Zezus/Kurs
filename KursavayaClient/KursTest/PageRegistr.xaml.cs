using System;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для PageRegistr.xaml
    /// </summary>
    public partial class PageRegistr
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _flag = true;
        private bool _check6;
        private bool _checkProb;
        private bool _checkRepPas;
        private readonly Methods _method = new Methods();

        public PageRegistr()
        {
            InitializeComponent();
        }

        public bool TestDate(string log, string mail, string pas, string repPas, string name)
        {
            int min = 6;
            int max = 10;
            if (log != "" && pas != "" && repPas != "" && name != "" && mail != "")
            {
                if (pas == repPas)
                {
                    if (log.IndexOf(' ') == -1 && pas.IndexOf(' ') == -1 && repPas.IndexOf(' ') == -1 && name.IndexOf(' ') == -1 && mail.IndexOf(' ') == -1)
                    {
                        if (log.Length >= min && pas.Length >= min && repPas.Length >= min && name.Length >= 4)
                        {
                            if (log.Length <= max && pas.Length <= max && repPas.Length <= max && name.Length <= max)
                            {

                            }
                            else
                            {
                                _flag = false;
                                MessageBox.Show("Максимальная длина ввода 10 символов", "Error", MessageBoxButton.OK);
                            }
                        }
                        else
                        {
                            _flag = false;
                            MessageBox.Show("Минимальная длина ввода 6 символов (поле name 4 символа)", "Error", MessageBoxButton.OK);
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
                    MessageBox.Show("Пароли не совпадают", "Error", MessageBoxButton.OK);

                }
            }
            else
            {
                _flag = false;
                MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK);
            }
            return _flag;
        }

        private static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public bool EmailValid()
        {
            try
            {
                MailAddress from = new MailAddress("remind-password-bot@mail.ru");
                MailAddress to = new MailAddress(tbMail.Text + mail_ru.Text);
                MailMessage mailSend = new MailMessage(from, to);
                mailSend.Body = "<h2></h2>";
                mailSend.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 25);
                smtp.Credentials = new NetworkCredential("remind-password-bot@mail.ru", "45626336rustam");
                smtp.EnableSsl = true;
                smtp.Send(mailSend);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void butRegistr_RegWin_Click(object sender, RoutedEventArgs e)
        {
            TestDate(tbLogin.Text, tbMail.Text, tbPassword.Password, tbRepeatPass.Password, tbName.Text);
            if (EmailValid())
            {
                if (_flag != true) return;
                _client = new TcpClient();
                _client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
                _stream = _client.GetStream();

                User user = new User();

                //отправляем тип активной страницы
                var typePage = GetType().ToString();
                _method.TypePage(_stream, typePage);

                //хешируем пароль 
                var passHashed = GetMd5Hash(tbPassword.Password);
                //ОТправляем данные с регистрации
                var userData = tbLogin.Text + " " + passHashed + " " + tbName.Text + " " + tbMail.Text +
                               mail_ru.Text + " " + user.CreatedAt + " " + user.ModifiedAt;
                _method.Send(_stream, userData);


                byte[] otvetRegistra = new byte[2];
                _stream.Read(otvetRegistra, 0, otvetRegistra.Length);
                string yesNo = Encoding.Unicode.GetString(otvetRegistra);
                if (yesNo == "0")
                {
                    ((ContentControl)Parent).Content = new PageMain();
                    MessageBox.Show("Регистрация прошла успешно, теперь вы можете войти в чат", "Goog Job",
                        MessageBoxButton.OK);
                    tbLogin.Clear();
                }
                else MessageBox.Show("Пользователь с таким логином уже существует", "Error", MessageBoxButton.OK);
                tbLogin.Clear();
            }
            else MessageBox.Show("Данной почты не существует", "Error", MessageBoxButton.OK); tbMail.Clear();
        }

        private void butExit_RegPage_Click(object sender, RoutedEventArgs e)
        {

            ((ContentControl)Parent).Content = new PageMain();
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbName.Text.Length >= 6)
            {
                _check6 = true;
            }
            else
            {
                _check6 = false;
            }
            if (tbName.Text.IndexOf(' ') == -1 && tbName.Text.Length != 0)
            {
                _checkProb = true;
            }
            else
            {
                _checkProb = false;
            }

            if (_checkProb)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Name.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Name.Text = "◄";
            }
            if (_check6)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (_checkProb != true)
                {
                    v_Name.Text = "◄";
                }
                else
                {
                    v_Name.Text = "";
                }
            }
            else
            {
                p_6symbol.Foreground = Brushes.Red;
                v_Name.Text = "◄";
            }
        }

        private void tbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbLogin.Text.Length >= 6)
            {
                _check6 = true;
            }
            else
            {
                _check6 = false;
            }
            if (tbLogin.Text.IndexOf(' ') == -1 && tbLogin.Text.Length != 0)
            {
                _checkProb = true;
            }
            else
            {
                _checkProb = false;
            }

            if (_checkProb)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Login.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Login.Text = "◄";
            }
            if (_check6)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (_checkProb != true)
                {
                    v_Login.Text = "◄";
                }
                else
                {
                    v_Login.Text = "";
                }
            }
            else
            {
                p_6symbol.Foreground = Brushes.Red;
                v_Login.Text = "◄";
            }
        }

        private void tbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Password.Length >= 6)
            {
                _check6 = true;
            }
            else
            {
                _check6 = false;
            }
            if (tbPassword.Password.IndexOf(' ') == -1 && tbPassword.Password.Length != 0)
            {
                _checkProb = true;
            }
            else
            {
                _checkProb = false;
            }

            if (_checkProb)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Pass.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Pass.Text = "◄";
            }
            if (_check6)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (_checkProb != true)
                {
                    v_Pass.Text = "◄";
                }
                else
                {
                    v_Pass.Text = "";
                }
            }
            else
            {
                p_6symbol.Foreground = Brushes.Red;
                v_Pass.Text = "◄";
            }
        }

        private void tbRepeatPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbRepeatPass.Password == tbPassword.Password)
            {
                _checkRepPas = true;
            }
            else
            {
                _checkRepPas = false;
            }

            if (_checkRepPas)
            {
                p_pass.Foreground = Brushes.Aqua;
                v_RepPas.Text = "";
            }
            else
            {
                p_pass.Foreground = Brushes.Red;
                v_RepPas.Text = "◄";
            }
        }

        private void cSelectmail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
