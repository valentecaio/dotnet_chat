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

        static Server server = new Server();

        static void Main()
        {
            server.StartServer();

            // keep console alive
            Console.Title = "Server";
            Console.WriteLine("Server started");
            Console.ReadLine();
        }

        #endregion

        class Server : MarshalByRefObject, IServerObject
        {
            #region Variables

            private TcpServerChannel serverChannel;
            private int tcpPort = 12345;
            private ObjRef internalRef;

            private bool serverActive = false;

            private List<string> usersList = new List<string>();

            #endregion

            #region IServerObject Members

            public event MessageArrivedEvent MessageArrived;

            public void PublishMessage(Message msg)
            {
                SafeInvokeMessageArrived(msg);
            }

            public List<string> PublishNewSubscriber(string username)
            {
                this.usersList.Add(username);
                PublishMessage(new Message { type = Message.TYPE_CONNECT, content = username });
                return this.usersList;
            }

            #endregion

            #region server managing

            public void StartServer()
            {
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

            public void StopServer()
            {
                if (!serverActive)
                    return;

                // unmarshal and unregister channel of this server
                RemotingServices.Unmarshal(internalRef);
                ChannelServices.UnregisterChannel(serverChannel);
            }

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