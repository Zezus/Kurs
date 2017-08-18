using System.Net;
using System.Net.Sockets;

namespace KursavayaServer
{
    public class Program
    {
        public static Connected Connected = new Connected();

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 3084);
            server.Start();
            while (true)
            {
                Connected.Listen(server);
                var type = Connected.Type();
                

                if (type == "KursTest.PageRegistr")
                {
                    Connected.Data();
                    var boool = Connected.YesNoRegistr();
                    Connected.PageRegistr(boool);
                }
                if (type == "KursTest.PageMain")
                {
                    Connected.Data();
                    var boool = Connected.YesNoMain();
                    Connected.PageMain(boool);
                }
                if (type == "KursTest.PageChat")
                {
                    Connected.PageChat();
                }
                if (type == "KursTest.PageAdmin")
                {
                    Connected.PageAdmin();
                }
                if (type == "KursTest.RemindPassword")
                {

                    Connected.Data();
                    var boool = Connected.YesNoRemimdPas();
                    Connected.PageRemimdPas(boool);
                }
            }
        }
    }
}
