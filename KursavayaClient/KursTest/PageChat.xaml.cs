using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KursTest
{
    /// <summary>
    /// Логика взаимодействия для PageChat.xaml
    /// </summary>
    public partial class PageChat
    {
        TcpClient _client;
        NetworkStream _stream;
        int count = 1;
        Methods method = new Methods();
        readonly PageMain _pm = new PageMain();



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
                ((ContentControl)Parent).Content = new PageMain();

            }

        }

        public void Sender(string s)
        {
            _client = new TcpClient();
            _client.Connect(new IPEndPoint(IPAddress.Parse(_pm.tbIP.Text), 3084));
            _stream = _client.GetStream();

            //отправляем тип активной страницы
            string typePage = GetType().ToString();
            method.TypePage(_stream, typePage);

            //отправляем смс
            string sms = s;
            method.Send(_stream, sms);
        }


        public void ButtonSend(object sender, RoutedEventArgs e)
        {
            _client = new TcpClient();
            _client.Connect(new IPEndPoint(IPAddress.Parse(_pm.tbIP.Text), 3084));
            _stream = _client.GetStream();

            //отправляем тип активной страницы
            string typePage = GetType().ToString();
            method.TypePage(_stream, typePage);

            //count++;
            string time = DateTime.Now.ToString("h:mm tt");
            string smsView = time + " " + tbSend.Text;

            //отправляем смс и время и счетчик
            string sms = time + " " + tbSend.Text + " " + count.ToString();
            method.Send(_stream, sms);

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
    }
}
