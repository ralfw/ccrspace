using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api
{
    public class CcrsPublisher<TBroadcastMessage> : ICcrsSimplexChannel<TBroadcastMessage>
    {
        #region Implementation of ICcrsSimplexChannel<TBroadcastMessage>
        public void Post(TBroadcastMessage message)
        {}
        #endregion


        public void Subscribe(Action<TBroadcastMessage> publicationHandler)
        {}

        public void Subscribe(ICcrsSimplexChannel<TBroadcastMessage> publicationListener)
        {}


        public void Unsubscribe(Action<TBroadcastMessage> publicationHandler)
        { }

        public void Unsubscribe(ICcrsSimplexChannel<TBroadcastMessage> publicationListener)
        { }
    }
}
