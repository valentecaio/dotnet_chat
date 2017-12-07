using System;

namespace Common
{
    /// <summary>
    /// 
    /// This class defines the messages that a connected client can send or receive.
    /// Each Message object has a type, which defines the context of the Message.
    /// 
    /// TYPE_TEXT: a text Message;
    /// content is the text; sender is the username of the client who created the message;
    /// 
    /// TYPE_CONNECT: a connection Message. May be sent only by the server;
    /// content is the username of the new client; sender is empty;
    /// 
    /// TYPE_DISCONNECT: a disconnection Message. May be sent only by the server;
    /// content is the username of the client who left the system; sender is empty;
    /// 
    /// </summary>

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