using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrSpaceConfig
    {
        public static CcrSpaceConfig New()
        {
            return new CcrSpaceConfig();
        }


        public CcrSpaceConfig RunningDispatcher(string name, int numberOfThreads)
        {
            return this;
        }

        public CcrSpaceConfig SchedulingWithDispatcherQueue(string dispatcherName)
        {
            return this;
        }

        public CcrSpaceConfig CatchingUnhandledExceptionAt(Action<Exception> defaultExceptionHandler)
        {
            return this;
        }
                           
        
        public CcrSpace Create()
        {
            return new CcrSpace(this);
        }
    }
}
