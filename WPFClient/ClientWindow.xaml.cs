using System;
using System.Collections.Generic;
using System.Windows;
using Interface;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Remoting;
using RemotingInterface;
using Common;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class ClientWindow : Window
    {
        #region variables

        public string serverHost = "localhost";
        public string serverPort = "12345";
        public string clientName;
        public List<User> usersList = new List<User>();
        public RemotingInterface.IServerObject remoteServer;

        EventProxy eventProxy;
        TcpChannel tcpChan;
        private bool connected = false;

        #endregion

        #region client init

        public ClientWindow()
        {
            InitializeComponent();
            
            // register IServerObject services
            string serverURL = "tcp://" + this.serverHost + ":" + this.serverPort + "/Server";
            RemotingConfiguration.RegisterWellKnownClientType(new WellKnownClientTypeEntry(typeof(IServerObject), serverURL));
        }

        #endregion

        #region UI callbacks
        
        void applyConf()
        {
            this.clientName = this.tbUsername.Text;
            this.serverPort = this.tbServerPort.Text;
            this.serverHost = this.tbServerIP.Text;
        }

        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            this.sendMessage(this.tbSend.Text);
        }
        
        private void btConfConnect_Click(object sender, RoutedEventArgs e)
        {
            // it works as a "reconnect button"
            // disconnect before connecting with new configurations
            this.disconnect();
            this.applyConf();
            this.connect();
        }
        
        private void btConfDisconnect_Click(object sender, RoutedEventArgs e)
        {
            this.disconnect();
        }

        private void menuConnect_Click(object sender, RoutedEventArgs e)
        {
            // it works as a "reconnect button"
            // disconnect before connecting with new configurations
            this.disconnect();
            this.applyConf();
            this.connect();
        }

        private void menuDisconnect_Click(object sender, RoutedEventArgs e)
        {
            this.disconnect();
        }

        private void menuQuit_Click(object sender, RoutedEventArgs e)
        {
            disconnect();
        }

        private void menuTestServer_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion callback

        #region UI changes
        
        private delegate void appendMessageListView(Message msg);

        private void appendMessage(Message msg)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new appendMessageListView(appendMessage), new object[] { msg });
                return;
            }
            else
            {
                this.lvMessages.Items.Add(msg);
            }
        }

        #endregion

        #region background
        
        void eventProxy_MessageArrived_callback(Message Message)
        {
            appendMessage(Message);
        }

        public void sendMessage(string text)
        {
            if (!connected)
                return;

            // broadcast message
            Message msg = new Message { sender = this.clientName, text = text };
            remoteServer.PublishMessage(msg);

            // empty message textBox
            tbSend.Text = "";
        }
        
        public void connect()
        {
            if (connected)
                return;

            try
            {
                // create and register TCP channel (get First available port)
                BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                Hashtable props = new Hashtable();
                props["name"] = "Client";
                props["port"] = 0;
                this.tcpChan = new TcpChannel(props, clientProv, serverProv);
                ChannelServices.RegisterChannel(tcpChan);

                // init client event proxy and register callback
                eventProxy = new EventProxy();
                eventProxy.MessageArrived += new MessageArrivedEvent(eventProxy_MessageArrived_callback);

                // server URL (tcp://<server ip>:<server port>/<server class>) and interface name
                string serverURI = "tcp://" + this.serverHost + ":" + this.serverPort + "/Server";

                // broadcast new connection
                remoteServer = (IServerObject)Activator.GetObject(typeof(IServerObject), serverURI);
                Message msg = new Message { sender = this.clientName, text = "Client " + this.clientName + " connected" };
                remoteServer.PublishMessage(msg);

                // attach the events
                remoteServer.MessageArrived += new MessageArrivedEvent(eventProxy.LocallyHandleMessageArrived);
                connected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect: " + ex.Message);
                connected = false;
            }
        }

        public void disconnect()
        {
            if (!connected)
                return;

            // First remove the event
            remoteServer.MessageArrived -= eventProxy.LocallyHandleMessageArrived;

            // Now we can close it out
            ChannelServices.UnregisterChannel(tcpChan);

            connected = false;
        }

        #endregion
    }
}
