using KursTest;
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

namespace Kursavaya 
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // TcpClient client;
       // NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();
            PageMain pageMain = new PageMain();
            this.Content = pageMain;
         //   client = new TcpClient();
          //  client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
           // stream = client.GetStream();
            //Connect();
        }

       /* public void Connect()
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3084));
            stream = client.GetStream();

            string a = "привет";
            byte[] dataBytes = Encoding.Unicode.GetBytes(a);
            stream.Write(dataBytes, 0, dataBytes.Length);
           
        }*/
    }
}
