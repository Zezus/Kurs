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
    /// Логика взаимодействия для PageRegistr.xaml
    /// </summary>
    public partial class PageRegistr : Page
    {
        TcpClient client;
        NetworkStream stream;
        bool flag = true;
        bool check6 = false;
        bool checkProb = false;
        bool checkRepPas = false;
        Methods method = new Methods();

        public PageRegistr()
        {
            InitializeComponent();
            cSelectmail.Items.Add("@mail.ru");
            cSelectmail.Items.Add("@gmail.com");
        }

        public bool TestDate(string log, string mail, string pas, string rep_pas, string name)
        {
            int min = 6;
            int max = 10;
            if (log != "" && pas != "" && rep_pas != "" && name != "" && mail != "")
            {
                if (pas == rep_pas)
                {
                    if (log.IndexOf(' ') == -1 && pas.IndexOf(' ') == -1 && rep_pas.IndexOf(' ') == -1 && name.IndexOf(' ') == -1 && mail.IndexOf(' ') == -1)
                    {
                        if (log.Length >= min && pas.Length >= min && rep_pas.Length >= min && name.Length >= 4)
                        {
                            if (log.Length <= max && pas.Length <= max && rep_pas.Length <= max && name.Length <= max)
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
                            MessageBox.Show("Минимальная длина ввода 6 символов (поле name 4 символа)", "Error", MessageBoxButton.OK);
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

        private void butRegistr_RegWin_Click(object sender, RoutedEventArgs e)
        {
            TestDate(tbLogin.Text, tbMail.Text, tbPassword.Password.ToString(), tbRepeatPass.Password.ToString(), tbName.Text);
            if (flag == true)
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
                stream = client.GetStream();

                //отправляем тип активной страницы
                string typePage = this.GetType().ToString();
                method.TypePage(stream, typePage);

                //ОТправляем данные с регистрации
                string userData = tbLogin.Text + " " + tbPassword.Password.ToString() + " " + tbName.Text + " " + tbMail.Text + cSelectmail.SelectedItem.ToString();
                method.Send(stream, userData);


                byte[] otvetRegistra = new byte[2];
                stream.Read(otvetRegistra, 0, otvetRegistra.Length);
                string yes_no = Encoding.Unicode.GetString(otvetRegistra);
                if (yes_no == "0")
                {
                    ((ContentControl)(this.Parent)).Content = new PageMain();
                    MessageBox.Show("Регистрация прошла успешно, теперь вы можете войти в чат", "Goog Job", MessageBoxButton.OK); tbLogin.Clear();
                }
                else MessageBox.Show("Пользователь с таким логином уже существует", "Error", MessageBoxButton.OK); tbLogin.Clear();

            }
        }

        private void butExit_RegPage_Click(object sender, RoutedEventArgs e)
        {

            ((ContentControl)(this.Parent)).Content = new PageMain();
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
         {
            if (tbName.Text.Length >= 6)
            {
                check6 = true;
            }
            else
            {
                check6 = false;
            }
            if (tbName.Text.IndexOf(' ') == -1 && tbName.Text.Length!=0)
            {
                checkProb = true;
            }
            else
            {
                checkProb = false;
            }

            if (checkProb == true)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Name.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Name.Text = "◄";
            }
            if (check6 == true)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (checkProb != true)
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
                check6 = true;
            }
            else
            {
                check6 = false;
            }
            if (tbLogin.Text.IndexOf(' ') == -1 && tbLogin.Text.Length != 0)
            {
                checkProb = true;
            }
            else
            {
                checkProb = false;
            }

            if (checkProb == true)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Login.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Login.Text = "◄";
            }
            if (check6 == true)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (checkProb != true)
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
            if (tbPassword.Password.ToString().Length >= 6)
            {
                check6 = true;
            }
            else
            {
                check6 = false;
            }
            if (tbPassword.Password.ToString().IndexOf(' ') == -1 && tbPassword.Password.ToString().Length != 0)
            {
                checkProb = true;
            }
            else
            {
                checkProb = false;
            }

            if (checkProb == true)
            {
                p_probel.Foreground = Brushes.Aqua;
                v_Pass.Text = "";
            }
            else
            {
                p_probel.Foreground = Brushes.Red;
                v_Pass.Text = "◄";
            }
            if (check6 == true)
            {
                p_6symbol.Foreground = Brushes.Aqua;
                if (checkProb != true)
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
            if (tbRepeatPass.Password.ToString() == tbPassword.Password.ToString())
            {
                checkRepPas = true;
            }
            else
            {
                checkRepPas = false;
            }

            if (checkRepPas == true)
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
            var items = cSelectmail.SelectedItem.ToString();
        }
    }
}
