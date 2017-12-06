using System;
namespace Common
{
    [Serializable()]
    public class Message
    {
        public string msg { get; set; }
        public string sender { get; set; }
    }
}