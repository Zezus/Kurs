using System.Net.Sockets;
using System.Text;

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
                byte[] typePageByte = Encoding.Unicode.GetBytes(typePage);
                //размер типа окна
                string typePageLenght = typePageByte.Length.ToString();
                //байтовый размер типа  окна
                byte[] typePageLenghtByte = Encoding.Unicode.GetBytes(typePageLenght);
                //отправляем размер окна
                stream.Write(typePageLenghtByte, 0, typePageLenghtByte.Length);
                #endregion

                #region отправка типа окна
                stream.Read(wait1, 0, wait1.Length);
                stream.Write(typePageByte, 0, typePageByte.Length);
                #endregion
        }

        public void Send(NetworkStream stream, string userData)
        {
            
            byte[] userDataByte = Encoding.Unicode.GetBytes(userData);
            string userDataLenght = userDataByte.Length.ToString();
            byte[] userDataLenghtByte = Encoding.Unicode.GetBytes(userDataLenght);
            stream.Write(userDataLenghtByte, 0, userDataLenghtByte.Length);
            stream.Read(wait1, 0, wait1.Length);
            stream.Write(userDataByte, 0, userDataByte.Length);
        }
    }
}
