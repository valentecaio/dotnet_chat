using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace remoteServer
{
	public class Server : MarshalByRefObject, RemotingInterface.IRemoteString
    {
        private string SEPARATOR = "_$_";
        private string SERVER = "SERVER";
        private string RECEIVE = "RECEIVE";
        private string CONNECT = "CONNECT";
        private string SEND = "SEND";

        private List<string> usersList = new List<string>();

		static void Main()
		{
			// Create a TCP channel to transfer data
			TcpChannel channel = new TcpChannel(12345);

            // register channel
            ChannelServices.RegisterChannel(channel);

			// Start server listenning in a Singleton object
			RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server), "Server",  WellKnownObjectMode.Singleton);

			// keep console alive
			Console.WriteLine("Server started");
            Console.ReadLine();	
		}

		// Remove server timeout
		public override object  InitializeLifetimeService()
		{
			return null;
		}

        public void broadcast(string msg)
        {

        }

        public void addUser(string username)
        {
            // add user to own list
            this.usersList.Add(username);

            // warn all users about new user
            this.TextMessage(SERVER + SEPARATOR + username);

            Console.WriteLine(this.usersList.ToString());
        }

        #region members of Interface

        public void TextMessage(string msg)
		{
            Console.WriteLine(RECEIVE + " msg " + msg + SERVER);
            broadcast(msg);
		}

        public void Login(string username)
        {
            Console.WriteLine(CONNECT + " with name " + username);
            this.addUser(username);
        }

        public string Logout()
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
