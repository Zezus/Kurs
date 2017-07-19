using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для PageAdmin.xaml
    /// </summary>
    public partial class PageAdmin : Page
    {
        TcpClient client;
        NetworkStream stream;
        const int buffersize = 12;
        byte[] wait1 = new byte[1];
        String[] lognema_Mas;
        string login;
        string messages;
        string name;
        string password;
        string countMes;
        List<User> listuser;
        Methods method = new Methods();

        public PageAdmin()
        {
            InitializeComponent();
            tbViewMessages.IsEnabled = false;
            btDel.IsEnabled = false;
            btLoadUsers.IsEnabled = true;
        }

        private void ButtonLoadUsers(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            //отправляем тип активной страницы
            string typePage = this.GetType().ToString();
            method.TypePage(stream, typePage);

            /*#region Отправляем название кнопки которая нажата
            var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
            byte[] methodType_byte = Encoding.Unicode.GetBytes(methodType);
            string methodType_lenght = methodType_byte.Length.ToString();
            byte[] methodType_lenght_byte = Encoding.Unicode.GetBytes(methodType_lenght);
            stream.Write(methodType_lenght_byte, 0, methodType_lenght_byte.Length);
            stream.Read(wait1, 0, wait1.Length);
            stream.Write(methodType_byte, 0, methodType_byte.Length);
            #endregion*/
            var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
            method.Send(stream, methodType);

            byte[] count_lenght_byte = new byte[buffersize];
            stream.Read(count_lenght_byte, 0, count_lenght_byte.Length);
            string count_lenght = Encoding.Unicode.GetString(count_lenght_byte);

            stream.Write(wait1, 0, wait1.Length);

            byte[] count_lenght_end = new byte[int.Parse(count_lenght)];
            stream.Read(count_lenght_end, 0, count_lenght_end.Length);
            string count = Encoding.Unicode.GetString(count_lenght_end);

            listuser = new List<User>(int.Parse(count));

            for (int i = 0; i < int.Parse(count); i++)
            {
                byte[] logname_buff = new byte[buffersize];
                stream.Read(logname_buff, 0, logname_buff.Length);
                string logname_lenght = Encoding.Unicode.GetString(logname_buff);
                stream.Write(wait1, 0, wait1.Length);
                byte[] logname_buff2 = new byte[int.Parse(logname_lenght)];
                stream.Read(logname_buff2, 0, logname_buff2.Length);
                string logname = Encoding.Unicode.GetString(logname_buff2);
                lognema_Mas = logname.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                login = lognema_Mas[0];
                name = lognema_Mas[1];
                password = lognema_Mas[2];
                countMes = lognema_Mas[3];

                lbtest.Items.Add($"{i + 1}.\t  {login}");

                byte[] otvetRegistra = new byte[2];
                stream.Read(otvetRegistra, 0, otvetRegistra.Length);
                string yes_no = Encoding.Unicode.GetString(otvetRegistra);
                if (yes_no == "0")
                {
                    stream.Write(wait1, 0, wait1.Length);
                    byte[] message_buff = new byte[buffersize];
                    stream.Read(message_buff, 0, message_buff.Length);
                    string mes_lenght = Encoding.Unicode.GetString(message_buff);
                    stream.Write(wait1, 0, wait1.Length);
                    byte[] mes_buff2 = new byte[int.Parse(mes_lenght)];
                    stream.Read(mes_buff2, 0, mes_buff2.Length);
                    string message_Text = Encoding.Unicode.GetString(mes_buff2);
                    messages = message_Text;
                    listuser.Add(new User { Login = login, Name = name, Password = password, Message = messages, Count = countMes });
                }
                else
                {
                    listuser.Add(new User { Login = login, Name = name, Password = password, Count = countMes });
                }
            }
            btLoadUsers.IsEnabled = false;
        }

        private void ButtonDelete(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            //отправляем тип активной страницы
            string typePage = this.GetType().ToString();
            method.TypePage(stream, typePage);

            //region Отправляем название кнопки которая нажата
            var methodType = System.Reflection.MethodBase.GetCurrentMethod().Name;
            method.Send(stream, methodType);

            var result = (MessageBox.Show("Вы уверены что хотите удалить данного пользователя", "Really", MessageBoxButton.YesNo, MessageBoxImage.Question));
            if (result == MessageBoxResult.Yes)
            {
                var item = lbtest.SelectedItems;
                var items_Mas = item[0].ToString().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                string items = items_Mas[1];
                foreach (var v in lbtest.SelectedItems)
                {
                    lbtest.Items.Remove(v);
                    break;
                }
                tbInfo.Clear(); tbViewMessages.Clear();
                tbInfo.IsEnabled = false; tbViewMessages.IsEnabled = false;

                byte[] selected_byte = Encoding.Unicode.GetBytes(items);
                string selected_lenght = selected_byte.Length.ToString();
                byte[] selected_lenght_byte = Encoding.Unicode.GetBytes(selected_lenght);
                stream.Write(selected_lenght_byte, 0, selected_lenght_byte.Length);
                stream.Read(wait1, 0, wait1.Length);
                stream.Write(selected_byte, 0, selected_byte.Length);
            }

        }
        public void ViewMessages()
        {
            tbViewMessages.IsEnabled = true;
            tbViewMessages.Clear();
            string item = lvUsersName.SelectedIndex.ToString();
            var items = lvUsersName.SelectedItems;
            User selected = (User)items[0];
            var userDate = listuser.First(x => x.Login == selected.Login);
            tbViewMessages.AppendText(userDate.Message);
        }

        private void lvUsersName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvUsersName.Background = Brushes.Red;
            btDel.IsEnabled = true;
            tbInfo.IsEnabled = true;
            tbInfo.Clear();
            tbInfo.Height = 150;
            tbInfo.Width = 270;

            string item = lvUsersName.SelectedIndex.ToString();
            var items = lvUsersName.SelectedItems;
            if (items.Count != 0)
            {
                User selected = (User)items[0];


                var a = listuser.First(i => i.Login == selected.Login);
                tbInfo.AppendText($"Login: {a.Login}{Environment.NewLine}{Environment.NewLine}Name: {a.Name}{Environment.NewLine}{Environment.NewLine}Password: {a.Password}{Environment.NewLine}{Environment.NewLine}Количество сообщений: {a.Count}");
                ViewMessages();
            }
            else
            {
                btDel.IsEnabled = false;
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var result = (MessageBox.Show("Вы уверены что хотите выйти?", "Really", MessageBoxButton.YesNo, MessageBoxImage.Question));
            if (result == MessageBoxResult.Yes)
            {
                ((ContentControl)(this.Parent)).Content = new PageMain();
            }
        }

        public void ViewMessages2()
        {
            tbViewMessages.IsEnabled = true;
            tbViewMessages.Clear();
            var item = lbtest.SelectedItems;
            var items_Mas = item[0].ToString().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string items = items_Mas[1];
            var userDate = listuser.First(x => x.Login == items);
            tbViewMessages.AppendText(userDate.Message);
        }
        private void lbtest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvUsersName.Background = Brushes.Red;
            btDel.IsEnabled = true;
            tbInfo.IsEnabled = true;
            tbInfo.Clear();
            tbInfo.Height = 150;
            tbInfo.Width = 201;

            var item = lbtest.SelectedItems;
            if (item.Count == 0)
            {
                lbtest.SelectedIndex = -1;
            }
            else
            {
                var items_Mas = item[0].ToString().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                string items = items_Mas[1];
                if (item.Count != 0)
                {
                    var a = listuser.First(i => i.Login == items);
                    tbInfo.AppendText($"Login: {a.Login}{Environment.NewLine}{Environment.NewLine}Name: {a.Name}{Environment.NewLine}{Environment.NewLine}Password: {a.Password}{Environment.NewLine}{Environment.NewLine}Количество сообщений: {a.Count}");
                    ViewMessages2();
                }
                else
                {
                    btDel.IsEnabled = false;
                }
            }
        }
    }
}
