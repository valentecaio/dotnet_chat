using Common;
using System;

namespace RemotingInterface
{
    /// <summary>
    /// this interface contains all the distributed methods
    /// </summary>

    public delegate void MessageArrivedEvent(Message Message);

    public interface IRemoteString
    {
        void TextMessage(string msg);
        void Login(string username);
        string Logout();
    }

    public interface IServerObject
    {
        #region Events

        event MessageArrivedEvent MessageArrived;

        #endregion

        #region Methods

        void PublishMessage(Message msg);

        #endregion
    }
}