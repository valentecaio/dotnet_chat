using Common;
using RemotingInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interface
{
    public class EventProxy : MarshalByRefObject
    {

        #region Event Declarations

        public event MessageArrivedEvent MessageArrived;

        #endregion

        #region Lifetime Services

        public override object InitializeLifetimeService()
        {
            return null;
            //Returning null holds the object alive
            //until it is explicitly destroyed
        }

        #endregion

        #region Local Handlers

        public void LocallyHandleMessageArrived(Message Message)
        {
            if (MessageArrived != null)
                MessageArrived(Message);
        }

        #endregion

    }
}