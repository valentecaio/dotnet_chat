using RemotingInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Common;

namespace remoteServer
{
    public class Program
    {
        #region program

        private static Server server = new Server();

        // available commands
        private static string CMD_HELP = "HELP";
        private static string CMD_START = "START";
        private static string CMD_STOP = "STOP";
        private static string CMD_QUIT = "QUIT";
        private static string CMD_LIST = "LIST";

        static void Main()
        {
            Console.Title = "Server Manager";

            Console.WriteLine("Server Manager launched.");

            string default_msg = "Type " + CMD_HELP + " to list available commands.";
            string cmd = "";
            while (cmd != CMD_QUIT)
            {
                if (cmd == CMD_HELP) {
                    // show hel message
                    Console.WriteLine("That's a server manager interface.\n" +
                        "These are the available commands:\n\n "
                        + CMD_LIST + "\tShow the list of clients of this server.\n "
                        + CMD_START + "\tStart server on localhost.\n "
                        + CMD_STOP + "\tStop active server.\n "
                        + CMD_QUIT + "\tStop server and close manager.\n "
                        + CMD_HELP + "\tShow this message.");
                } else if (cmd == CMD_START) {
                    // start a server
                    if (server.serverActive) {
                        Console.WriteLine("The server is already active.\n" + default_msg);
                    } else {
                        server.StartServer();
                        Console.WriteLine("Server started on port " + server.tcpPort.ToString() + ".");
                    }
                } else if (cmd == CMD_STOP) {
                    // stop a server
                    if (!server.serverActive) {
                        Console.WriteLine("The server is already stopped.\n" + default_msg);
                    } else {
                        server.StopServer();
                        Console.WriteLine("Server stopped.");
                    }
                } else if (cmd == CMD_LIST) {
                    // list connected clients
                    if (!server.serverActive)
                        Console.WriteLine("The server is not active.\n" + default_msg);
                    else {
                        Console.Write("Users: [ ");
                        foreach (string user in server.usersList)
                            Console.Write(user + ", ");
                        Console.WriteLine(" ]");
                    }
                } else {
                    Console.WriteLine(default_msg);
                }

                // get next command
                Console.Write("\n>> ");
                cmd = Console.ReadLine();
            }
        }

        #endregion

        class Server : MarshalByRefObject, IServerObject
        {
            #region Variables

            // tcp channel used to listen to clients messages
            private TcpServerChannel serverChannel;
            private ObjRef internalRef;

            public int tcpPort = 12345;
            public bool serverActive = false;
            public List<string> usersList = new List<string>();

            #endregion

            #region IServerObject Members

            public event MessageArrivedEvent MessageArrived;

            // broadcast message
            public void PublishMessage(Message msg)
            {
                SafeInvokeMessageArrived(msg);
            }

            // subscribe a client by adding it to clients list and warning all users about it
            public List<string> Subscribe(string username)
            {
                this.usersList.Add(username);
                PublishMessage(new Message { type = Message.TYPE_CONNECT, content = username });
                return this.usersList;
            }

            // subscribe a client by removing it from clients list and warning all users about it
            public void Unsubscribe(string username)
            {
                this.usersList.Remove(username);
                PublishMessage(new Message { type = Message.TYPE_DISCONNECT, content = username });
            }
            
            // used by the client to test server availability
            public void Ping() { return; }

            #endregion

            #region server management

            // start server;
            // create a listenner tcp channel at _this.tcpPort_ port
            public void StartServer()
            {
                // if server is already started, job is done
                if (serverActive)
                    return;
                
                // Set up for remoting events properly
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                // create and register tcp channel
                Hashtable props = new Hashtable();
                props["port"] = this.tcpPort;
                props["name"] = "Server";
                this.serverChannel = new TcpServerChannel(props, serverProv);
                ChannelServices.RegisterChannel(serverChannel, false);

                // keep reference to marshalled object
                internalRef = RemotingServices.Marshal(this, props["name"].ToString());

                // server is active
                serverActive = true;
            }

            // stop server;
            // destroy listenner tcp channel and free _this.tcpPort_ port
            public void StopServer()
            {
                // if server is already stopped, job is done
                if (!serverActive)
                    return;

                // unmarshal server from RemotingServices 
                RemotingServices.Unmarshal(internalRef);

                // unregister channel of this server
                this.serverChannel = null;
                ChannelServices.UnregisterChannel(serverChannel);
            }

            // trigger all registered MessageArrived events with Message _msg_
            private void SafeInvokeMessageArrived(Message msg)
            {
                if (!serverActive)
                    return;

                // abort if no one is listenning
                if (MessageArrived == null)
                    return;

                MessageArrivedEvent listener = null;
                Delegate[] dels = MessageArrived.GetInvocationList();

                foreach (Delegate del in dels)
                {
                    try
                    {
                        listener = (MessageArrivedEvent)del;
                        listener.Invoke(msg);
                    }
                    catch (Exception ex)
                    {
                        // Could not reach the destination, so remove it from the list
                        MessageArrived -= listener;
                    }
                }
            }
            
            // Remove server timeout
            public override object InitializeLifetimeService()
            {
                return null;
            }

            #endregion
        }
    }
}