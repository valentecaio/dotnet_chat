using Common;
using System.Collections.Generic;

namespace RemotingInterface
{
    /// <summary>
    /// this interface contains all the distributed methods
    /// </summary>

    public delegate void MessageArrivedEvent(Message msg);
    
    public interface IServerObject
    {
        #region Events

        // event triggered to notify client that a message arrived
        event MessageArrivedEvent MessageArrived;

        #endregion

        #region Methods

        // broadcast message to subscribed clients
        void PublishMessage(Message msg);

        // subscribe a client with name username
        // return the updated list of clients of the server
        List<string> Subscribe(string username);

        // unsubscribe a client with name username
        void Unsubscribe(string username);

        // do nothing, but return
        // used to test server availability
        void Ping();

        #endregion
    }
}