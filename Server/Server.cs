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
        static Server server = new Server();

        static void Main()
        {
            server.StartServer();

            // keep console alive
            Console.Title = "Server";
            Console.WriteLine("Server started");
            Console.ReadLine();
        }

        public class Server : MarshalByRefObject, IServerObject
        {
            #region Variables

            private TcpServerChannel serverChannel;
            private int tcpPort = 12345;
            private ObjRef internalRef;
            private bool serverActive = false;
            private static string serverURI = "Server";

            private string SEPARATOR = "_$_";
            private string SERVER = "SERVER";
            private string RECEIVE = "RECEIVE";
            private string CONNECT = "CONNECT";
            private string SEND = "SEND";

            private List<string> usersList = new List<string>();

            #endregion

            #region IServerObject Members

            public event MessageArrivedEvent MessageArrived;

            public void PublishMessage(Message Message)
            {
                SafeInvokeMessageArrived(Message);
            }

            #endregion

            #region remote

            public void StartServer()
            {
                if (serverActive)
                    return;

                Hashtable props = new Hashtable();
                props["port"] = this.tcpPort;
                props["name"] = serverURI;

                //Set up for remoting events properly
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                serverChannel = new TcpServerChannel(props, serverProv);

                try
                {
                    ChannelServices.RegisterChannel(serverChannel, false);
                    internalRef = RemotingServices.Marshal(this, props["name"].ToString());
                    serverActive = true;
                }
                catch (RemotingException re)
                {
                    //Could not start the server because of a remoting exception
                }
                catch (Exception ex)
                {
                    //Could not start the server because of some other exception
                }
            }

            public void StopServer()
            {
                if (!serverActive)
                    return;

                RemotingServices.Unmarshal(internalRef);

                try
                {
                    ChannelServices.UnregisterChannel(serverChannel);
                }
                catch (Exception ex)
                {

                }
            }

            private void SafeInvokeMessageArrived(Message Message)
            {
                if (!serverActive)
                    return;

                if (MessageArrived == null)
                    return;         //No Listeners

                MessageArrivedEvent listener = null;
                Delegate[] dels = MessageArrived.GetInvocationList();

                foreach (Delegate del in dels)
                {
                    try
                    {
                        listener = (MessageArrivedEvent)del;
                        listener.Invoke(Message);
                    }
                    catch (Exception ex)
                    {
                        //Could not reach the destination, so remove it
                        //from the list
                        MessageArrived -= listener;
                    }
                }
            }

            #endregion

            // Remove server timeout
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}