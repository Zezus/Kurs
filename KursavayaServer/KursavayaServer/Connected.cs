using KursavayaServer.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Mail;

namespace KursavayaServer
{
    public class Connected
    {
        const int buffersize = 12;
        TcpListener server;
        TcpClient client;
        NetworkStream stream;
        string data;
        bool login_search;
        string login;
        string mail;
        string password;
        string name;
        string countMes;
        String[] date_Mas;
        String[] sms_Mas;
        User user;
        byte[] wait1 = new byte[1];
        Random rnd = new Random();


        public void Listen(TcpListener server)
        {
            client = server.AcceptTcpClient();
            Console.WriteLine(Environment.NewLine + "Подключен!!");
            stream = client.GetStream();
        }

        public string Type()
        {
            byte[] type_Lenght = new byte[buffersize];

            stream.Read(type_Lenght, 0, type_Lenght.Length);
            string typeSms_Lenght = Encoding.Unicode.GetString(type_Lenght);

            int typeBuffer_lenght = int.Parse(typeSms_Lenght);
            byte[] type_sms = new byte[typeBuffer_lenght];

            stream.Write(wait1, 0, wait1.Length);

            stream.Read(type_sms, 0, type_sms.Length);
            string type = Encoding.Unicode.GetString(type_sms);

            Console.WriteLine($"Принял:\tТип окна от которого пришло сообщение  {type} ");
            return type;
        }

        public string Data()
        {
            byte[] data_Lenght = new byte[buffersize];
            stream.Read(data_Lenght, 0, data_Lenght.Length);
            string dataSms_Lenght = Encoding.Unicode.GetString(data_Lenght);

            int dataBuffer_Lenght = int.Parse(dataSms_Lenght);
            byte[] data_sms = new byte[dataBuffer_Lenght];

            stream.Write(wait1, 0, wait1.Length);

            stream.Read(data_sms, 0, data_sms.Length);
            data = Encoding.Unicode.GetString(data_sms);

            Console.WriteLine($"Принял:\tДанные пользователя при регистрации  {data}");
            return data;
        }

        public bool YesNoRegistr()
        {
            date_Mas = data.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            login = date_Mas[0];
            login_search = true;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == login)
                {
                    login_search = false;
                }
            }
            return login_search;
        }

        public void PageRegistr(bool b)
        {
            if (b == true)
            {
                password = date_Mas[1];
                name = date_Mas[2];
                mail = date_Mas[3];
                var storage = new XmlStorage("UserBase.xml");
                var users = storage.Load();
                users.Add(new User() { Login = login, Name = name, Password = password, Mail = mail });
                storage.Save(users);


                Console.WriteLine($"Система:  Данные пользователя при регистрации  {data}  (Пользователь добавлен)");
                string otvet = "0";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                stream.Write(otvetRegistra, 0, otvetRegistra.Length);
            }
            else
            {
                Console.WriteLine($"Система:\tДанные пользователя при регистрации  {data}  (Уже существует)");
                string otvet = "1";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                stream.Write(otvetRegistra, 0, otvetRegistra.Length);

            }
        }

        public bool YesNoMain()
        {
            date_Mas = data.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            login = date_Mas[0];
            password = date_Mas[1];
            login_search = false;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == login)
                {
                    login_search = true;
                }
            }
            return login_search;
        }

        public void PageMain(bool b)
        {
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();
            bool check_user = false;
            if (b == true)
            {
                foreach (var item in users)
                {
                    if (login == item.Login && password == item.Password)
                    {
                        check_user = true;
                        user = item;
                    }

                }
                if (check_user == true)
                {

                    string otvet = "0";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    Console.WriteLine($"Система:  Вы вошли");
                }
                else
                {
                    Console.WriteLine($"Система:\tДанные введены неверно");
                    string otvet = "1";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    stream.Write(otvetRegistra, 0, otvetRegistra.Length);

                }
            }
            else
            {
                Console.WriteLine($"Система:\tДанные введены неверно");
                string otvet = "1";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                stream.Write(otvetRegistra, 0, otvetRegistra.Length);

            }
        }

        public void PageChat()
        {
            byte[] sms_byte_lenght = new byte[buffersize];
            stream.Read(sms_byte_lenght, 0, sms_byte_lenght.Length);
            string sms_lenght = Encoding.Unicode.GetString(sms_byte_lenght);

            int sms_buff_lenght = int.Parse(sms_lenght);
            byte[] sms_byte = new byte[sms_buff_lenght];

            stream.Write(wait1, 0, wait1.Length);

            stream.Read(sms_byte, 0, sms_byte.Length);
            var sms = Encoding.Unicode.GetString(sms_byte);
            sms_Mas = sms.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var time = sms_Mas[0];
            countMes = sms_Mas.Last();
            var smsLine = "";
            for (int i = 1; i < sms_Mas.Length; i++)
            {
                smsLine += sms_Mas[i];
                smsLine += " ";
            }
            Console.WriteLine($"{login}:  {smsLine}    {time}");
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();
            user.Message += $"{Environment.NewLine}\t{time}\t{smsLine.Remove(smsLine.Length - 2, 1)}";
            //user.Count = int.Parse(countMes);
            var c = user.Count + int.Parse(countMes);
            user.Count = c;
            users.Add(user);
            storage.Save(users);
            List<User> listUser = new List<User>();

            foreach (var item in users)
            {
                if (item.Login == login)
                {
                    listUser.Add(item);
                }
            }
            var a = listUser[0];
            users.Remove(a);
            storage.Save(users);
        }

        public void PageAdmin()
        {
            #region Прием названия кнопки которая нажата в "Admin"
            byte[] methodType_byte_lenght = new byte[buffersize];
            stream.Read(methodType_byte_lenght, 0, methodType_byte_lenght.Length);
            string methodType_lenght = Encoding.Unicode.GetString(methodType_byte_lenght);
            int methodType_buff_lenght = int.Parse(methodType_lenght);
            byte[] methodType_byte = new byte[methodType_buff_lenght];

            stream.Write(wait1, 0, wait1.Length);

            stream.Read(methodType_byte, 0, methodType_byte.Length);
            var methodType = Encoding.Unicode.GetString(methodType_byte);
            #endregion

            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();
            int count = users.Count();

            if (methodType == "ButtonLoadUsers")
            {
                byte[] count_byte = Encoding.Unicode.GetBytes(count.ToString());
                string count_lenght = count_byte.Length.ToString();
                byte[] count_lenght_byte = Encoding.Unicode.GetBytes(count_lenght);
                stream.Write(count_lenght_byte, 0, count_lenght_byte.Length);
                stream.Read(wait1, 0, wait1.Length);
                stream.Write(count_byte, 0, count_byte.Length);

                foreach (var item in users)
                {
                    byte[] logname_byte = Encoding.Unicode.GetBytes(item.Login.ToString() + " " + item.Name.ToString() + " " + item.Password.ToString() + " " + item.Count.ToString());
                    byte[] logname_lenght = Encoding.Unicode.GetBytes(logname_byte.Length.ToString());
                    stream.Write(logname_lenght, 0, logname_lenght.Length);
                    stream.Read(wait1, 0, wait1.Length);
                    stream.Write(logname_byte, 0, logname_byte.Length);


                    if (item.Message != null)
                    {
                        string otvet = "0";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        stream.Write(otvetRegistra, 0, otvetRegistra.Length);

                        stream.Read(wait1, 0, wait1.Length);

                        byte[] message_byte = Encoding.Unicode.GetBytes(item.Message.ToString());
                        byte[] message_byte_lenght = Encoding.Unicode.GetBytes(message_byte.Length.ToString());
                        stream.Write(message_byte_lenght, 0, message_byte_lenght.Length);
                        stream.Read(wait1, 0, wait1.Length);
                        stream.Write(message_byte, 0, message_byte.Length);
                    }
                    else
                    {
                        string otvet = "1";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    }
                }
            }
            if (methodType == "ButtonDelete")
            {
                #region Получаем логин пользователя которого нужно удалить из базы
                byte[] selected_byte_lenght = new byte[buffersize];
                stream.Read(selected_byte_lenght, 0, selected_byte_lenght.Length);
                string selected_lenght = Encoding.Unicode.GetString(selected_byte_lenght);
                int selected_buff_lenght = int.Parse(selected_lenght);
                byte[] selected_byte = new byte[selected_buff_lenght];

                stream.Write(wait1, 0, wait1.Length);

                stream.Read(selected_byte, 0, selected_byte.Length);
                var selectedLogin = Encoding.Unicode.GetString(selected_byte);
                #endregion

                User p = users.First(x => x.Login == selectedLogin);
                users.Remove(p);
                storage.Save(users);
            }
        }

        public bool YesNoRemimdPas()
        {
            date_Mas = data.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            login = date_Mas[0];
            login_search = false;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == login)
                {
                    login_search = true;
                }
            }
            return login_search;
        }




        public void PageRemimdPas(bool b)
        {
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            #region Прием названия кнопки которая нажата в "Remind"
            //stream.Write(wait1, 0, wait1.Length);
            byte[] methodType_byte_lenght = new byte[buffersize];
            stream.Read(methodType_byte_lenght, 0, methodType_byte_lenght.Length);
            string methodType_lenght = Encoding.Unicode.GetString(methodType_byte_lenght);
            int methodType_buff_lenght = int.Parse(methodType_lenght);
            byte[] methodType_byte = new byte[methodType_buff_lenght];

            stream.Write(wait1, 0, wait1.Length);

            stream.Read(methodType_byte, 0, methodType_byte.Length);
            var methodType = Encoding.Unicode.GetString(methodType_byte);
            //stream.Write(wait1, 0, wait1.Length);

            #endregion

            if (methodType == "btOK_Click")
            {
                if (b == true)
                {
                    // int random = rnd.Next(10000, 99999);
                    int random = 1;
                    
                    foreach (var item in users)
                    {
                        if (item.Login == login)
                        {
                            mail = item.Mail;
                        }
                    }
                    string otvet = "0";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    Console.WriteLine($"Система:  Такой логин для восстановления есть");
                    stream.Read(wait1, 0, wait1.Length);

                    string userMail = mail + " " + random;
                    byte[] userMail_byte = Encoding.Unicode.GetBytes(userMail);
                    string userMail_lenght = userMail_byte.Length.ToString();
                    byte[] userMail_lenght_byte = Encoding.Unicode.GetBytes(userMail_lenght);
                    stream.Write(userMail_lenght_byte, 0, userMail_lenght_byte.Length);
                    stream.Read(wait1, 0, wait1.Length);
                    stream.Write(userMail_byte, 0, userMail_byte.Length);

                    MailAddress from = new MailAddress("remind-password-bot@mail.ru");
                    MailAddress to = new MailAddress(mail);
                    MailMessage mailSend = new MailMessage(from, to);
                    mailSend.Body = "<h2>Код для восстановления пароля </h2>" + random;
                    mailSend.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient("smtp.mail.ru", 25);
                    smtp.Credentials = new NetworkCredential("remind-password-bot@mail.ru", "45626336rustam");
                    smtp.EnableSsl = true;
                    smtp.Send(mailSend);

                }
                else
                {
                    Console.WriteLine($"Система:\tТакого логина для восстановления нет");
                    string otvet = "1";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                }
            }
            if (methodType == "btChangePas_Click")
            {
                #region Прием пароля для изменения 
                byte[] pas_byte_lenght = new byte[buffersize];
                stream.Read(pas_byte_lenght, 0, pas_byte_lenght.Length);
                string pas_lenght = Encoding.Unicode.GetString(pas_byte_lenght);
                int pas_buff_lenght = int.Parse(pas_lenght);
                byte[] pas_byte = new byte[pas_buff_lenght];
                stream.Write(wait1, 0, wait1.Length);
                stream.Read(pas_byte, 0, pas_byte.Length);
                var pas = Encoding.Unicode.GetString(pas_byte);
                #endregion

                User p = users.First(x => x.Login == login);
                p.Password = pas.ToString();
                users.Add(p);
                storage.Save(users);
                List<User> listUser = new List<User>();

                foreach (var item in users)
                {
                    if (item.Login == login)
                    {
                        listUser.Add(item);
                    }
                }
                var a = listUser[0];
                users.Remove(a);
                storage.Save(users);
            }
        }
    }
}
