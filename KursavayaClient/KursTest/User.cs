using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursTest
{
    [Serializable]
    public class User
    {
        public string Login { get; set; }
        public string Mail{ get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
        public string Count { get; set; }
    }
}
