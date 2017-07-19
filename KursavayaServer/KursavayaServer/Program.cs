using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KursavayaServer
{
    public class Program
    {
        static Connected connected = new Connected();

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 3084);
            server.Start();
            while (true)
            {
                connected.Listen(server);
                var type = connected.Type();
                

                if (type == "KursTest.PageRegistr")
                {
                    var data = connected.Data();
                    var boool = connected.YesNoRegistr();
                    connected.PageRegistr(boool);
                }
                if (type == "KursTest.PageMain")
                {
                    var data = connected.Data();
                    var boool = connected.YesNoMain();
                    connected.PageMain(boool);
                }
                if (type == "KursTest.PageChat")
                {
                    connected.PageChat();
                }
                if (type == "KursTest.PageAdmin")
                {
                    connected.PageAdmin();
                }
                if (type == "KursTest.Remind_password")
                {
                    var data = connected.Data();
                    var boool = connected.YesNoRemimdPas();
                    connected.PageRemimdPas(boool);
                }
            }
        }
    }
}
