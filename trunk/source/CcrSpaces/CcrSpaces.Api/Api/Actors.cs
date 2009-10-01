using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsActorContext
    {
        public ITask Receive<T>()
        {
            return null;
        }

        public ITask Receive<T0, T1>()
        {
            return null;
        }


        public object ReceivedValue
        { 
            get
            {
                return null;
            }
        }


        public void Reply(object message)
        {}
    }


    public class CcrsActor : ICcrsSimplexChannel<object>
    {
        public void Post(object message)
        {}

        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        public bool TryPostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
