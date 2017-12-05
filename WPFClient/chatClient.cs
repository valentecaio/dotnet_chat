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
        #region variables

        public string confServerHost = "localhost";
        public string confClientPort;
        public string confUsername;
        public List<Interface.User> usersList = new List<Interface.User>();
        public RemotingInterface.IRemoteString strRemote;

        #endregion

        public chatClient()
        {
            // create a receptor TCP channel
            TcpChannel canal = new TcpChannel();

            // register channnel
            ChannelServices.RegisterChannel(canal);

            // get server objet reference
            // needs: server URL (tcp://<server ip>:<server port>/<server class>) and interface name
            this.strRemote = (RemotingInterface.IRemoteString)Activator.GetObject(
                typeof(RemotingInterface.IRemoteString), "tcp://localhost:12345/Server");
        }

        #region Interface

        public string TextMessage()
        {
            // TODO : implement
            return "The message was received by the server";
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
