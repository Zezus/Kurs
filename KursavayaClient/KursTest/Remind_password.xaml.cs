using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для Remind_password.xaml
    /// </summary>
    public partial class RemindPassword
    {
        TcpClient _client;
        NetworkStream _stream;
        const int Buffersize = 12;
        string _kod;
        bool _flag = true;
        byte[] wait1 = new byte[1];
        Methods method = new Methods();

        public RemindPassword()
        {
            InitializeComponent();
            LabKod.Visibility = Visibility.Hidden;
            tbKod.Visibility = Visibility.Hidden;
            btKodEnter.Visibility = Visibility.Hidden;
            LabNewPas.Visibility = Visibility.Hidden;
            tbPas.Visibility = Visibility.Hidden;
            LabRepPas.Visibility = Visibility.Hidden;
            tbRepPas.Visibility = Visibility.Hidden;
            btChangePas.Visibility = Visibility.Hidden;
            btOK.IsEnabled = false;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            _client = new TcpClient();
            _client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            _stream = _client.GetStream();

            //отправляем тип активной страницы
            string typePage = GetType().ToString();
            method.TypePage(_stream, typePage);

            //отправляем данные с tbLogin
            string userData = tbLogin.Text;
            method.Send(_stream, userData);

            //Отправляем название кнопки которая нажата
            var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
            method.Send(_stream, methodType);


            byte[] otvetRegistra = new byte[2];
            _stream.Read(otvetRegistra, 0, otvetRegistra.Length);
            string yesNo = Encoding.Unicode.GetString(otvetRegistra);
            if (yesNo == "0")
            {
                _stream.Write(wait1, 0, wait1.Length);

                string sms;

                byte[] mailLenght = new byte[Buffersize];
                _stream.Read(mailLenght, 0, mailLenght.Length);
                string mailSmsLenght = Encoding.Unicode.GetString(mailLenght);

                int dataBufferLenght = int.Parse(mailSmsLenght);
                byte[] mailSms = new byte[dataBufferLenght];

                _stream.Write(wait1, 0, wait1.Length);

                _stream.Read(mailSms, 0, mailSms.Length);
                sms = Encoding.Unicode.GetString(mailSms);
                var smsMas = sms.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                _kod = smsMas[1];
                LabKod.Visibility = Visibility.Visible;
                tbKod.Visibility = Visibility.Visible;
                btKodEnter.Visibility = Visibility.Visible;
                btKodEnter.IsEnabled = false;
                if (btOK.Content.ToString() == "Получить\r\nкод снова")
                {
                    tbPas.Visibility = Visibility.Hidden;
                    tbRepPas.Visibility = Visibility.Hidden;
                    btChangePas.Visibility = Visibility.Hidden;
                    LabNewPas.Visibility = Visibility.Hidden;
                    LabRepPas.Visibility = Visibility.Hidden;
                    tbKod.Clear();
                }
                btOK.Content = "Получить" + Environment.NewLine + "код снова";
                tbKod.Clear();
                MessageBox.Show("Код для восстановления пароля отправлен на вашу почту", "Info", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Такого логина нет", "Error", MessageBoxButton.OK); tbLogin.Clear();
                tbKod.Visibility = Visibility.Hidden;
                LabKod.Visibility = Visibility.Hidden;
                btKodEnter.Visibility = Visibility.Hidden;
                tbPas.Visibility = Visibility.Hidden;
                tbRepPas.Visibility = Visibility.Hidden;
                btChangePas.Visibility = Visibility.Hidden;
                LabNewPas.Visibility = Visibility.Hidden;
                LabRepPas.Visibility = Visibility.Hidden;
            }
        }

        private void btKodEnter_Click(object sender, RoutedEventArgs e)
        {
            if (tbKod.Text == _kod)
            {
                LabKod.Visibility = Visibility.Hidden;
                tbKod.Visibility = Visibility.Hidden;
                btKodEnter.Visibility = Visibility.Hidden;
                tbPas.Visibility = Visibility.Visible;
                tbRepPas.Visibility = Visibility.Visible;
                btChangePas.Visibility = Visibility.Visible;
                LabNewPas.Visibility = Visibility.Visible;
                LabRepPas.Visibility = Visibility.Visible;
                btOK.IsEnabled = false;
                tbKod.Clear();
            }
            else
            {
                MessageBox.Show("Введенный вами код неверный, попробуйте снова", "Error", MessageBoxButton.OK); //tbLogin.Clear();
                tbKod.Clear();
            }
        }

        private void tbKod_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbKod.Text.Length != 0)
            {
                btKodEnter.IsEnabled = true;
            }
            else
            {
                btKodEnter.IsEnabled = false;
            }
        }

        private void tbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbLogin.Text.Length != 0)
            {
                btOK.IsEnabled = true;
            }
            else
            {
                btOK.IsEnabled = false;
            }
        }

        public bool TestPas(string pas, string repPas)
        {
            int min = 6;
            int max = 16;
            if (pas != "" && repPas != "")
            {
                if (pas == repPas)
                {
                    if (pas.IndexOf(' ') == -1 && repPas.IndexOf(' ') == -1)
                    {
                        if (pas.Length >= min && repPas.Length >= min)
                        {
                            if (pas.Length <= max && repPas.Length <= max)
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

        private void btChangePas_Click(object sender, RoutedEventArgs e)
        {
            TestPas(tbPas.Text, tbRepPas.Text);
            if (_flag)
            {

                _client = new TcpClient();
                _client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
                _stream = _client.GetStream();

                //отправляем тип активной страницы
                string typePage = GetType().ToString();
                method.TypePage(_stream, typePage);

                //отправляем данные с tbLogin
                string userData = tbLogin.Text;
                method.Send(_stream, userData);

                //Отправляем название кнопки которая нажата
                var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
                method.Send(_stream, methodType);

                //отправляем данные с tbPas
                string pas = tbPas.Text;
                method.Send(_stream, pas);

                tbPas.Clear();
                tbRepPas.Clear();
                ((ContentControl)Parent).Content = new PageMain();
                MessageBox.Show("Ваш пароль успешно изменен", "Good Job", MessageBoxButton.OK);
            }
            else
            {
                tbRepPas.Clear();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = (MessageBox.Show("Вы уверены что хотите выйти?", "Really", MessageBoxButton.YesNo, MessageBoxImage.Question));
            if (result == MessageBoxResult.Yes)
            {
                ((ContentControl)Parent).Content = new PageMain();
            }
        }
    }
}
