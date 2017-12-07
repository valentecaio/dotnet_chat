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
using System.Windows.Input;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class ClientWindow : Window
    {
        #region variables

        public string serverURI = "tcp://localhost:12345/Server";
        public string username;
        public List<string> usersList = new List<string>();
        public RemotingInterface.IServerObject remoteServer;

        EventProxy eventProxy;
        TcpChannel tcpChan;
        private bool connected = false;

        #endregion

        #region client init

        public ClientWindow()
        {
            InitializeComponent();

            // open a TCP channel to listen
            // this channel may only be closed when finishing app
            openChannel();
        }

        #endregion

        #region UI callbacks

        // get information from configuration text boxes 
        // and regenerate username and server URI
        private void applyConf()
        {
            this.username = this.tbUsername.Text;
            this.serverURI = "tcp://" + this.tbServerIP.Text + ":" + this.tbServerPort.Text + "/Server";
        }

        // callback for btSend; 
        // create a message and broadcast it
        private void callback_sendMessage(object sender, RoutedEventArgs e)
        {
            this.sendMessage(this.tbSend.Text);
        }

        // callback for menuConnect and btConfConnect
        private void callback_connect(object sender, RoutedEventArgs e)
        {
            // it works as a "reconnect"
            // disconnect before connecting with new configurations
            this.disconnect();
            this.applyConf();
            this.connect();
        }

        // callback for menuDisconnect and btConfDisconnect
        private void callback_disconnect(object sender, RoutedEventArgs e)
        {
            this.disconnect();
        }

        // callback for menuQuit;
        // disconnect and unregister channel before leaving
        private void callback_quit(object sender, RoutedEventArgs e)
        {
            disconnect();
            freeChannel();
            Close();
        }

        // callback for menuTestServer and btConfTestServer;
        // use new informations from configuration boxes to test if server is up
        private void callback_testServer(object sender, RoutedEventArgs e)
        {
            applyConf();
            testConnection();
        }

        // callback for keyDown of tbSend;
        // when <return> return is pressed, send message
        private void callback_keyDown_send(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.sendMessage(this.tbSend.Text);
            }
        }

        // callback for X close button;
        // disconnect and unregister channel before leaving
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            disconnect();
            freeChannel();
        }

        #endregion callback

        #region UI changes

        // used by showMessage to update UI element outside main thread
        private delegate void appendMessageListView(Message msg);
        
        // thread-safe method that appends a Message _msg_ to messages listView
        private void appendMessage(Message msg)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new appendMessageListView(appendMessage), new object[] { msg });
                return;
            }
            else
            {
                this.lvMessages.Items.Add(msg);
            }
        }

        // used by updateUsersTable to update UI element outside main thread
        private delegate void updateUserListView(List<string> users);

        // thread-safe method that updates users listView with given _users_ list
        private void updateUsersTable(List<string> users)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new updateUserListView(updateUsersTable), new object[] { users });
                return;
            }
            else
            {
                this.lvUsers.Items.Clear();
                foreach(string user in users)
                {
                    this.lvUsers.Items.Add(new User { username = user });
                }
            }
        }
        
        // log an information _text_ in client's messages board
        private void log(string text)
        {
            appendMessage(new Message { sender = " ", content = text });
        }
        
        #endregion

        #region background

        // broadcast a text message with content _text_
        public void sendMessage(string text)
        {
            // abort if client is disconnected or text is empty
            if (!connected || text.Length==0)
                return;

            // broadcast message
            Message msg = new Message { type = Message.TYPE_TEXT, sender = this.username, content = text };
            remoteServer.PublishMessage(msg);

            // clear message textBox
            tbSend.Text = "";
        }

        // callback for a message arrival
        public void eventProxy_MessageArrived_callback(Message msg)
        {
            // treat msg according to type
            if (msg.type == Message.TYPE_TEXT) {
                // show text messages
                appendMessage(msg);
            } else if (msg.type == Message.TYPE_CONNECT) {
                string user = msg.content;

                // ignore own connect messages
                if (user == this.username)
                    return;

                // add new client to userslist
                this.usersList.Add(user);

                // update users table
                updateUsersTable(this.usersList);

                // log message to user
                log("Client " + user + " connected.");
            } else if (msg.type == Message.TYPE_DISCONNECT) {
                string user = msg.content;

                // remove client from userslist
                this.usersList.Remove(user);

                // update users table
                updateUsersTable(this.usersList);

                // log message to user
                log("Client " + user + " disconnected.");
            }
        }

        // safely open a listenner tcp channel and register it to ChannelServices
        public void openChannel()
        {
            try
            {
                // create and register TCP channel to listen
                // server filter level type must be specified so it can trigger events in the client
                BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                Hashtable props = new Hashtable();
                props["name"] = "Client";
                props["port"] = 0;
                this.tcpChan = new TcpChannel(props, clientProv, serverProv);
                ChannelServices.RegisterChannel(tcpChan);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create TCP channel: " + ex.Message);
                connected = false;
            }
        }

        // safely unregister and destroy tcp channel
        // may only be used when closing app
        public void freeChannel()
        {
            try
            {
                ChannelServices.UnregisterChannel(tcpChan);
                tcpChan = null;
            } catch (Exception ex)
            {
                Console.WriteLine("unable to Unregister Channel: " + ex.Message);
            }
        }

        // connect client to system
        // the tcp listenner channel is not created in this function
        // if the server can't be reached, a MessageBox is the output
        public void connect()
        {
            // if client is already connected, job is done
            // also return if client's username is empty
            if (connected || this.tbUsername.Text == "")
                return;

            try
            {
                // init event proxy and register callback
                eventProxy = new EventProxy();
                eventProxy.MessageArrived += new MessageArrivedEvent(eventProxy_MessageArrived_callback);

                // attach own event handler to remoteServer invoker
                remoteServer = (IServerObject)Activator.GetObject(typeof(IServerObject), serverURI);
                remoteServer.MessageArrived += new MessageArrivedEvent(eventProxy.LocallyHandleMessageArrived);
                
                // broadcast new connection and receive clients list
                this.usersList = remoteServer.Subscribe(this.username);

                // refresh users table
                this.updateUsersTable(this.usersList);

                // change status to connected
                log("Connected as " + this.username + ".");
                this.connected = true;
            }
            catch (Exception ex)
            {
                // can't connect, probably because server is down
                // warn user and return to disconnected status
                MessageBox.Show("Could not connect: " + ex.Message);
                connected = false;
            }
        }

        // disconnect client from server
        // the tcp channel is not closed
        public void disconnect()
        {
            // if client is already disconnected, job is done
            if (!connected)
                return;

            try
            {
                // remove client's event handler
                remoteServer.MessageArrived -= eventProxy.LocallyHandleMessageArrived;

                // warn server about disconnection
                remoteServer.Unsubscribe(this.username);
            } catch (Exception ex)
            {
                Console.WriteLine("error when disconnecting: " + ex.ToString());
            }

            // empty users table
            this.usersList.Clear();
            updateUsersTable(this.usersList);

            // change status
            log("Disconnected.");
            connected = false;
        }

        // try to call a test function (Ping) from the server
        // the output is a MessageBox with serverURI and the result of the request
        public void testConnection()
        {
            try
            {
                IServerObject remoteServer = (IServerObject)Activator.GetObject(typeof(IServerObject), serverURI);
                remoteServer.Ping();
                
                // if client can contact server, server is up
                MessageBox.Show("The server is up! (" + serverURI + ").");
            } catch (Exception ex)
            {
                // if client can't contact server, server is down
                MessageBox.Show("Can't connect to server (" + serverURI + ").");
            }

        }

        #endregion
    }
}
