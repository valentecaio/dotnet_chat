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
        private string CONNECT = "CONNECT";

        #region variables

        public string serverHost = "localhost";
        public string serverPort = "12345";
        public string name;
        public List<Interface.User> usersList = new List<Interface.User>();
        public RemotingInterface.IRemoteString server;
        public TcpChannel channel;

        #endregion

        public chatClient()
        {
            // create a receiver TCP channel
            this.channel = new TcpChannel();

            // register channnel
            ChannelServices.RegisterChannel(channel);
        }

        public void sendMessage(string text)
        {
            // encode message
            string msg = this.name + SEPARATOR + text;

            Console.WriteLine(SEND + " msg " + msg);

            // launch remote method
            this.server.TextMessage(msg);
        }

        public void connect()
        {
            // get server objet reference
            // input: server URL (tcp://<server ip>:<server port>/<server class>) and interface name
            string serverURL = "tcp://" + this.serverHost + ":" + this.serverPort + "/Server";
            this.server = (RemotingInterface.IRemoteString)Activator.GetObject(
                typeof(RemotingInterface.IRemoteString), serverURL);

            this.server.Login(this.name);

            Console.WriteLine(CONNECT + " with name " + this.name);
        }

        #region Interface

        public void TextMessage(string msg)
        {
            Console.WriteLine(RECEIVE + " msg " + msg);
        }

        public void Login(string username)
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
