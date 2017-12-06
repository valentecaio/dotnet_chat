using System;
namespace Common
{
    [Serializable()]
    public class Message
    {
        public string text { get; set; }
        public string sender { get; set; }
    }
}