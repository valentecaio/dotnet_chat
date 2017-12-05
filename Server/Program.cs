using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace remoteServer
{
	public class Server : MarshalByRefObject, RemotingInterface.IRemoteString
	{
		static void Main()
		{
			// Create a TCP channel to transfer data
			TcpChannel canal = new TcpChannel(12345);

			// register channel
			ChannelServices.RegisterChannel(canal);

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
		

		#region members of Interface

		public string Hello()
		{
			// TODO : implement
			return "The message was received by the server" ;
		}

		#endregion
	}
}
