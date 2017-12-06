using System;
namespace Common
{
    [Serializable()]
    public class Message
    {
        public static int TYPE_TEXT = 1;
        public static int TYPE_CONNECT = 2;
        public static int TYPE_DISCONNECT = 3;

        public int type { get; set; }
        public string content { get; set; }
        public string sender { get; set; }
    }
}