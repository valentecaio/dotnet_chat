using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    class chatClient: RemotingInterface.IRemoteString
    {
        private string SEPARATOR = "_$_";
        private string RECEIVE = "RECEIVE";
        private string SEND = "SEND";

        #region variables

        public string serverHost = "localhost";
        public string serverPort = "12345";
        public string name;
        public List<Interface.User> usersList = new List<Interface.User>();
        public RemotingInterface.IRemoteString server;

        #endregion

        public chatClient()
        {
            // create a receptor TCP channel
            TcpChannel canal = new TcpChannel();

            // register channnel
            ChannelServices.RegisterChannel(canal);

            // get server objet reference
            // input: server URL (tcp://<server ip>:<server port>/<server class>) and interface name
            this.server = (RemotingInterface.IRemoteString)Activator.GetObject(
                typeof(RemotingInterface.IRemoteString), "tcp://localhost:12345/Server");
        }

        public void sendMessage(string text)
        {
            // encode message
            string msg = this.name + SEPARATOR + text;

            Console.WriteLine(SEND + " msg " + msg);

            // launch remote method
            this.server.TextMessage(msg);
        }

        #region Interface

        public void TextMessage(string msg)
        {
            Console.WriteLine(RECEIVE + " msg " + msg);
        }

        public string Login()
        {
            throw new NotImplementedException();
        }

        public string Logout()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
