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
    /// Логика взаимодействия для Remind_password.xaml
    /// </summary>
    public partial class Remind_password : Page
    {
        TcpClient client;
        NetworkStream stream;
        const int buffersize = 12;
        string kod;
        bool flag = true;
        byte[] wait1 = new byte[1];
        Methods method = new Methods();

        public Remind_password()
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
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            //отправляем тип активной страницы
            string typePage = this.GetType().ToString();
            method.TypePage(stream, typePage);

            //отправляем данные с tbLogin
            string userData = tbLogin.Text;
            method.Send(stream, userData);

            //Отправляем название кнопки которая нажата
            var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
            method.Send(stream, methodType);


            byte[] otvetRegistra = new byte[2];
            stream.Read(otvetRegistra, 0, otvetRegistra.Length);
            string yes_no = Encoding.Unicode.GetString(otvetRegistra);
            if (yes_no == "0")
            {
                stream.Write(wait1, 0, wait1.Length);

                string sms;
                string mail;

                byte[] mail_Lenght = new byte[buffersize];
                stream.Read(mail_Lenght, 0, mail_Lenght.Length);
                string mailSms_Lenght = Encoding.Unicode.GetString(mail_Lenght);

                int dataBuffer_Lenght = int.Parse(mailSms_Lenght);
                byte[] mail_sms = new byte[dataBuffer_Lenght];

                stream.Write(wait1, 0, wait1.Length);

                stream.Read(mail_sms, 0, mail_sms.Length);
                sms = Encoding.Unicode.GetString(mail_sms);
                var sms_Mas = sms.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                mail = sms_Mas[0];
                kod = sms_Mas[1];
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
            if (tbKod.Text == kod)
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

        public bool TestPas(string pas, string rep_pas)
        {
            int min = 6;
            int max = 16;
            if (pas != "" && rep_pas != "")
            {
                if (pas == rep_pas)
                {
                    if (pas.IndexOf(' ') == -1 && rep_pas.IndexOf(' ') == -1)
                    {
                        if (pas.Length >= min && rep_pas.Length >= min)
                        {
                            if (pas.Length <= max && rep_pas.Length <= max)
                            {

                            }
                            else
                            {
                                flag = false;
                                MessageBox.Show("Максимальная длина ввода 16 символов", "Error", MessageBoxButton.OK);
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
                    MessageBox.Show("Пароли не совпадают", "Error", MessageBoxButton.OK);

                }
            }
            else
            {
                flag = false;
                MessageBox.Show("Все поля должны быть заполнены", "Error", MessageBoxButton.OK);
            }
            return flag;
        }

        private void btChangePas_Click(object sender, RoutedEventArgs e)
        {
            TestPas(tbPas.Text, tbRepPas.Text);
            if (flag==true)
            {

                client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
                stream = client.GetStream();

                //отправляем тип активной страницы
                string typePage = this.GetType().ToString();
                method.TypePage(stream, typePage);

                //отправляем данные с tbLogin
                string userData = tbLogin.Text;
                method.Send(stream, userData);

                //Отправляем название кнопки которая нажата
                var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
                method.Send(stream, methodType);

                //отправляем данные с tbPas
                string pas = tbPas.Text.ToString();
                method.Send(stream, pas);

                tbPas.Clear();
                tbRepPas.Clear();
                ((ContentControl)(this.Parent)).Content = new PageMain();
                MessageBox.Show("Ваш пароль успешно изменен", "Good Job", MessageBoxButton.OK);
            }
            else
            {
                tbRepPas.Clear();
            }
        }

        private void tbRepPas_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = (MessageBox.Show("Вы уверены что хотите выйти?", "Really", MessageBoxButton.YesNo, MessageBoxImage.Question));
            if (result == MessageBoxResult.Yes)
            {
                ((ContentControl)(this.Parent)).Content = new PageMain();
            }
        }
    }
}
