using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrSpaceFluent
    {
        public static CcrSpaceFluent New()
        {
            return new CcrSpaceFluent();
        }


        public CcrSpaceFluent RunningDispatcher(string name, int numberOfThreads)
        {
            return this;
        }

        public CcrSpaceFluent SchedulingWithDispatcherQueue(string dispatcherName)
        {
            return this;
        }

        public CcrSpaceFluent CatchingUnhandledExceptionAt(Action<Exception> defaultExceptionHandler)
        {
            return this;
        }
                           
        
        public CcrSpace Create()
        {
            return new CcrSpace(this);
        }
    }
}
