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
    /// Логика взаимодействия для PageChat.xaml
    /// </summary>
    public partial class PageChat : Page
    {
        TcpClient client;
        NetworkStream stream;
        int count = 1;
        Methods method = new Methods();


        public PageChat()
        {
            InitializeComponent();
            btSend.IsEnabled = false;
        }

        private void butExit_ChatPage_Click(object sender, RoutedEventArgs e)
        {
            var result = (MessageBox.Show("Вы уверены что хотите выйти из аккаунта", "Really", MessageBoxButton.YesNo, MessageBoxImage.Question));
            if (result == MessageBoxResult.Yes)
            {
                ((ContentControl)(this.Parent)).Content = new PageMain();
            }

        }

        public void Sender(string s)
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            //отправляем тип активной страницы
            string typePage = this.GetType().ToString();
            method.TypePage(stream, typePage);

            //отправляем смс
            string sms = s;
            method.Send(stream, sms);
        }


        public void ButtonSend(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            //отправляем тип активной страницы
            string typePage = this.GetType().ToString();
            method.TypePage(stream, typePage);

            //count++;
            string time = DateTime.Now.ToString("h:mm tt");
            string smsView = time + " " + tbSend.Text;

            //отправляем смс и время и счетчик
            string sms = time + " " + tbSend.Text + " " + count.ToString();
            method.Send(stream, sms);

            tbChat.AppendText(smsView + Environment.NewLine);
            tbSend.Clear();
        }

        private void tbSend_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSend.Text == "")
            {
                btSend.IsEnabled = false;
            }
            else
            {
                btSend.IsEnabled = true;
            }
            tbCount.Text = tbSend.Text.Length + "/" + "70";
        }

        private void tbSend_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbSend.Text == "")
                {
                    btSend.IsEnabled = false;
                }
                else
                {
                    ButtonSend(this, e);
                }
            }
        }

        private void btSend_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
