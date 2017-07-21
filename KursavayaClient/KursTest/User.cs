using System;

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
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public static DateTimeOffset GetCurrentDate()
        {
            return DateTimeOffset.Now;
        }

        public User()
        {
            CreatedAt = GetCurrentDate();
            ModifiedAt = GetCurrentDate();

        }
    }
}
