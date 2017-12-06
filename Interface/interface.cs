using Common;

namespace RemotingInterface
{
    /// <summary>
    /// this interface contains all the distributed methods
    /// </summary>

    public delegate void MessageArrivedEvent(Message msg);
    
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