using KursTest;

namespace Kursavaya 
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
       // TcpClient client;
       // NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();
            PageMain pageMain = new PageMain();
            Content = pageMain;
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
