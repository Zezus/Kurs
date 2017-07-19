using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KursTest
{
    public class Methods
    {
        byte[] wait1 = new byte[1];

        public void TypePage(NetworkStream stream, string typePage)
        {
             #region отправка размера типа окна
                //тип окна
                //string typePage = this.GetType().ToString();
                //байтовый типа окна
                byte[] typePage_byte = Encoding.Unicode.GetBytes(typePage);
                //размер типа окна
                string typePage_lenght = typePage_byte.Length.ToString();
                //байтовый размер типа  окна
                byte[] typePage_lenght_byte = Encoding.Unicode.GetBytes(typePage_lenght);
                //отправляем размер окна
                stream.Write(typePage_lenght_byte, 0, typePage_lenght_byte.Length);
                #endregion

                #region отправка типа окна
                byte[] wait1 = new byte[1];
                stream.Read(wait1, 0, wait1.Length);
                stream.Write(typePage_byte, 0, typePage_byte.Length);
                #endregion
        }

        public void Send(NetworkStream stream, string userData)
        {
            
            byte[] userData_byte = Encoding.Unicode.GetBytes(userData);
            string userData_lenght = userData_byte.Length.ToString();
            byte[] userData_lenght_byte = Encoding.Unicode.GetBytes(userData_lenght);
            stream.Write(userData_lenght_byte, 0, userData_lenght_byte.Length);
            stream.Read(wait1, 0, wait1.Length);
            stream.Write(userData_byte, 0, userData_byte.Length);
        }
    }
}
