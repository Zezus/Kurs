using KursavayaServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;

namespace KursavayaServer
{
    public class Connected
    {
        private const int Buffersize = 12;
        private TcpListener _server;
        private TcpClient _client;
        private NetworkStream _stream;
        private string _data;
        private bool _loginSearch;
        private string _login;
        private string _mail;
        private string _password;
        private string _name;
        private string _countMes;
        private string _createTime;
        private string _modifiedTime;
        private String[] _dateMas;
        private String[] _smsMas;
        private User _user;
        readonly byte[] _wait1 = new byte[1];
        readonly Random _rnd = new Random();


        public void Listen(TcpListener server)
        {
            _client = server.AcceptTcpClient();
            Console.WriteLine(Environment.NewLine + "Подключен!!");
            _stream = _client.GetStream();
        }

        public string Type()
        {
            byte[] typeLenght = new byte[Buffersize];

            _stream.Read(typeLenght, 0, typeLenght.Length);
            string typeSmsLenght = Encoding.Unicode.GetString(typeLenght);

            int typeBufferLenght = int.Parse(typeSmsLenght);
            byte[] typeSms = new byte[typeBufferLenght];

            _stream.Write(_wait1, 0, _wait1.Length);

            _stream.Read(typeSms, 0, typeSms.Length);
            string type = Encoding.Unicode.GetString(typeSms);

            Console.WriteLine($"Принял:\tТип окна от которого пришло сообщение  {type}  ");
            return type;
        }

        public string Data()
        {

            byte[] dataLenght = new byte[Buffersize];
            _stream.Read(dataLenght, 0, dataLenght.Length);
            string dataSmsLenght = Encoding.Unicode.GetString(dataLenght);

            int dataBufferLenght = int.Parse(dataSmsLenght);
            byte[] dataSms = new byte[dataBufferLenght];

            _stream.Write(_wait1, 0, _wait1.Length);

            _stream.Read(dataSms, 0, dataSms.Length);
            _data = Encoding.Unicode.GetString(dataSms);

            Console.WriteLine($"Принял:\tДанные пользователя при регистрации  {_data}");
            return _data;
        }

        public bool YesNoRegistr()
        {
            _dateMas = _data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            _login = _dateMas[0];
            _loginSearch = true;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == _login)
                {
                    _loginSearch = false;
                }
            }
            return _loginSearch;
        }

        public void PageRegistr(bool b)
        {
            if (b)
            {
                _password = _dateMas[1];
                _name = _dateMas[2];
                _mail = _dateMas[3];
                _createTime = _dateMas[4] + " " + _dateMas[5];
                _modifiedTime = _dateMas[7] + " " + _dateMas[8];
                var storage = new XmlStorage("UserBase.xml");
                var users = storage.Load();
                users.Add(new User() { Login = _login, Name = _name, Password = _password, Mail = _mail, CreatedAt = _createTime, ModifiedAt = _modifiedTime });
                storage.Save(users);


                Console.WriteLine($"Система:  Данные пользователя при регистрации  {_data}  (Пользователь добавлен)");
                string otvet = "0";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
            }
            else
            {
                Console.WriteLine($"Система:\tДанные пользователя при регистрации  {_data}  (Уже существует)");
                string otvet = "1";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                _stream.Write(otvetRegistra, 0, otvetRegistra.Length);

            }
        }

        public bool YesNoMain()
        {
            _dateMas = _data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            _login = _dateMas[0];
            _password = _dateMas[1];
            _loginSearch = false;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == _login)
                {
                    _loginSearch = true;
                }
            }
            return _loginSearch;
        }

        static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public void PageMain(bool b)
        {
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();


            #region Проверяем хешированный пароль
            bool hashpass = false;
            foreach (var item in users)
            {
                if (_login == item.Login)
                {
                    if (_login == "admin" && _password == "admin")
                    {
                        hashpass = true;
                    }
                    else
                    {
                        if (_password == null)
                        {
                            throw new ArgumentNullException("b");
                        }
                        var passhash = GetMd5Hash(_password);
                        byte[] passHashbBytes1 = Encoding.UTF8.GetBytes(passhash);
                        if (item.Password != null)
                        {
                            byte[] passHashbBytes2 = Encoding.UTF8.GetBytes(item.Password);

                            hashpass = ByteArrayCompare(passHashbBytes1, passHashbBytes2);
                        }
                    }
                }

            }
            #endregion

            bool checkUser = false;
            if (b)
            {
                foreach (var item in users)
                {
                    if (_login == item.Login && hashpass)
                    {
                        checkUser = true;
                        _user = item;
                    }

                }
                if (checkUser)
                {

                    string otvet = "0";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    Console.WriteLine($"Система:  Вы вошли");
                }
                else
                {
                    Console.WriteLine($"Система:\tДанные введены неверно");
                    string otvet = "1";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    _stream.Write(otvetRegistra, 0, otvetRegistra.Length);

                }
            }
            else
            {
                Console.WriteLine($"Система:\tДанные введены неверно");
                string otvet = "1";
                byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                _stream.Write(otvetRegistra, 0, otvetRegistra.Length);

            }
        }

        public void PageChat()
        {
            byte[] smsByteLenght = new byte[Buffersize];
            _stream.Read(smsByteLenght, 0, smsByteLenght.Length);
            string smsLenght = Encoding.Unicode.GetString(smsByteLenght);

            int smsBuffLenght = int.Parse(smsLenght);
            byte[] smsByte = new byte[smsBuffLenght];

            _stream.Write(_wait1, 0, _wait1.Length);

            _stream.Read(smsByte, 0, smsByte.Length);
            var sms = Encoding.Unicode.GetString(smsByte);
            _smsMas = sms.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var time = _smsMas[0];
            _countMes = _smsMas.Last();
            var smsLine = "";
            for (int i = 1; i < _smsMas.Length; i++)
            {
                smsLine += _smsMas[i];
                smsLine += " ";
            }
            Console.WriteLine($"{_login}:  {smsLine}    {time}");
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();
            _user.Message += $"{Environment.NewLine}  {time}    {smsLine.Remove(smsLine.Length - 2, 1)}";
            //user.Count = int.Parse(countMes);
            var c = _user.Count + int.Parse(_countMes);
            _user.Count = c;
            users.Add(_user);
            storage.Save(users);
            List<User> listUser = new List<User>();

            foreach (var item in users)
            {
                if (item.Login == _login)
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
            byte[] methodTypeByteLenght = new byte[Buffersize];
            _stream.Read(methodTypeByteLenght, 0, methodTypeByteLenght.Length);
            string methodTypeLenght = Encoding.Unicode.GetString(methodTypeByteLenght);
            int methodTypeBuffLenght = int.Parse(methodTypeLenght);
            byte[] methodTypeByte = new byte[methodTypeBuffLenght];

            _stream.Write(_wait1, 0, _wait1.Length);

            _stream.Read(methodTypeByte, 0, methodTypeByte.Length);
            var methodType = Encoding.Unicode.GetString(methodTypeByte);
            #endregion

            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();
            int count = users.Count()-1;

            if (methodType == "ButtonLoadUsers")
            {
                byte[] countByte = Encoding.Unicode.GetBytes(count.ToString());
                string countLenght = countByte.Length.ToString();
                byte[] countLenghtByte = Encoding.Unicode.GetBytes(countLenght);
                _stream.Write(countLenghtByte, 0, countLenghtByte.Length);
                _stream.Read(_wait1, 0, _wait1.Length);
                _stream.Write(countByte, 0, countByte.Length);

                for (int i = 1; i < users.Count; i++)
                {
                    byte[] lognameByte = Encoding.Unicode.GetBytes(users[i].Login + " " + users[i].Name + " " + users[i].Password + " " + users[i].Count.ToString() + " " + users[i].CreatedAt + " " + users[i].ModifiedAt);
                    byte[] lognameLenght = Encoding.Unicode.GetBytes(lognameByte.Length.ToString());
                    _stream.Write(lognameLenght, 0, lognameLenght.Length);
                    _stream.Read(_wait1, 0, _wait1.Length);
                    _stream.Write(lognameByte, 0, lognameByte.Length);


                    if (users[i].Message != null)
                    {
                        string otvet = "0";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        _stream.Write(otvetRegistra, 0, otvetRegistra.Length);

                        _stream.Read(_wait1, 0, _wait1.Length);

                        byte[] messageByte = Encoding.Unicode.GetBytes(users[i].Message);
                        byte[] messageByteLenght = Encoding.Unicode.GetBytes(messageByte.Length.ToString());
                        _stream.Write(messageByteLenght, 0, messageByteLenght.Length);
                        _stream.Read(_wait1, 0, _wait1.Length);
                        _stream.Write(messageByte, 0, messageByte.Length);
                    }
                    else
                    {
                        string otvet = "1";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    }
                }

                /*foreach (var item in users)
                {
                    byte[] lognameByte = Encoding.Unicode.GetBytes(item.Login + " " + item.Name + " " + item.Password + " " + item.Count.ToString() + item.CreatedAt);
                    byte[] lognameLenght = Encoding.Unicode.GetBytes(lognameByte.Length.ToString());
                    _stream.Write(lognameLenght, 0, lognameLenght.Length);
                    _stream.Read(_wait1, 0, _wait1.Length);
                    _stream.Write(lognameByte, 0, lognameByte.Length);


                    if (item.Message != null)
                    {
                        string otvet = "0";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        _stream.Write(otvetRegistra, 0, otvetRegistra.Length);

                        _stream.Read(_wait1, 0, _wait1.Length);

                        byte[] messageByte = Encoding.Unicode.GetBytes(item.Message);
                        byte[] messageByteLenght = Encoding.Unicode.GetBytes(messageByte.Length.ToString());
                        _stream.Write(messageByteLenght, 0, messageByteLenght.Length);
                        _stream.Read(_wait1, 0, _wait1.Length);
                        _stream.Write(messageByte, 0, messageByte.Length);
                    }
                    else
                    {
                        string otvet = "1";
                        byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                        _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    }
                }*/
            }
            if (methodType == "ButtonDelete")
            {
                #region Получаем логин пользователя которого нужно удалить из базы
                byte[] selectedByteLenght = new byte[Buffersize];
                _stream.Read(selectedByteLenght, 0, selectedByteLenght.Length);
                string selectedLenght = Encoding.Unicode.GetString(selectedByteLenght);
                int selectedBuffLenght = int.Parse(selectedLenght);
                byte[] selectedByte = new byte[selectedBuffLenght];

                _stream.Write(_wait1, 0, _wait1.Length);

                _stream.Read(selectedByte, 0, selectedByte.Length);
                var selectedLogin = Encoding.Unicode.GetString(selectedByte);
                #endregion

                User p = users.First(x => x.Login == selectedLogin);
                users.Remove(p);
                storage.Save(users);
            }
        }

        public bool YesNoRemimdPas()
        {
            _dateMas = _data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            _login = _dateMas[0];
            _loginSearch = false;
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();

            foreach (var item in users)
            {
                if (item.Login == _login)
                {
                    _loginSearch = true;
                }
            }
            return _loginSearch;
        }

        public void PageRemimdPas(bool b)
        {
            var storage = new XmlStorage("UserBase.xml");
            var users = storage.Load();


            #region Прием названия кнопки которая нажата в "Remind"
            //stream.Write(wait1, 0, wait1.Length);
            byte[] methodTypeByteLenght = new byte[Buffersize];
            _stream.Read(methodTypeByteLenght, 0, methodTypeByteLenght.Length);
            string methodTypeLenght = Encoding.Unicode.GetString(methodTypeByteLenght);
            int methodTypeBuffLenght = int.Parse(methodTypeLenght);
            byte[] methodTypeByte = new byte[methodTypeBuffLenght];
            _stream.Write(_wait1, 0, _wait1.Length);

            _stream.Read(methodTypeByte, 0, methodTypeByte.Length);
            var methodType = Encoding.Unicode.GetString(methodTypeByte);
            //stream.Write(wait1, 0, wait1.Length);

            #endregion

            if (methodType == "btOK_Click")
            {
                if (b)
                {
                    int random = _rnd.Next(10000, 99999);

                    foreach (var item in users)
                    {
                        if (item.Login == _login)
                        {
                            _mail = item.Mail;
                        }
                    }
                    string otvet = "0";
                    byte[] otvetRegistra = Encoding.Unicode.GetBytes(otvet);
                    _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                    Console.WriteLine($"Система:  Такой логин для восстановления есть");
                    _stream.Read(_wait1, 0, _wait1.Length);

                    string userMail = _mail + " " + random;
                    byte[] userMailByte = Encoding.Unicode.GetBytes(userMail);
                    string userMailLenght = userMailByte.Length.ToString();
                    byte[] userMailLenghtByte = Encoding.Unicode.GetBytes(userMailLenght);
                    _stream.Write(userMailLenghtByte, 0, userMailLenghtByte.Length);
                    _stream.Read(_wait1, 0, _wait1.Length);
                    _stream.Write(userMailByte, 0, userMailByte.Length);

                    MailAddress from = new MailAddress("remind-password-bot@mail.ru");
                    MailAddress to = new MailAddress(_mail);
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
                    _stream.Write(otvetRegistra, 0, otvetRegistra.Length);
                }
            }
            if (methodType == "btChangePas_Click")
            {
                #region Прием пароля для изменения 
                byte[] pasByteLenght = new byte[Buffersize];
                _stream.Read(pasByteLenght, 0, pasByteLenght.Length);
                string pasLenght = Encoding.Unicode.GetString(pasByteLenght);
                int pasBuffLenght = int.Parse(pasLenght);
                byte[] pasByte = new byte[pasBuffLenght];
                _stream.Write(_wait1, 0, _wait1.Length);
                _stream.Read(pasByte, 0, pasByte.Length);
                var pas_time = Encoding.Unicode.GetString(pasByte);
                var pasTime = pas_time.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                #endregion

                User p = users.First(x => x.Login == _login);
                p.Password = pasTime[0];
                p.ModifiedAt = pasTime[1] + " " + pasTime[2];
                users.Add(p);
                storage.Save(users);
                List<User> listUser = new List<User>();

                foreach (var item in users)
                {
                    if (item.Login == _login)
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
