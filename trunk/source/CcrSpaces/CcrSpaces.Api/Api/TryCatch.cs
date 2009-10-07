using System;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsCatch : IDisposable
    {
        private readonly ICausality causality;


        public CcrsCatch(Action<Exception> exceptionHandler) : this(exceptionHandler, new DispatcherQueue()) {}
        internal CcrsCatch(Action<Exception> exceptionHandler, DispatcherQueue taskQueue) :
            this(new CcrsOneWayChannel<Exception>(
                        new CcrsOneWayChannelConfig<Exception>
                        {
                            MessageHandler = exceptionHandler,
                            TaskQueue = taskQueue,
                            ProcessSequentially = true
                        }))
        {}

        public CcrsCatch(IPort exceptionListener)
        {
            this.causality = new Causality("TryCatch", exceptionListener);
            Dispatcher.AddCausality(this.causality);
        }


        #region Implementation of IDisposable
        public void Dispose()
        {
            Dispatcher.RemoveCausality(this.causality);
        }
        #endregion
    }


    public class CcrsTry
    {
        private readonly Action tryThis;
        private readonly DispatcherQueue taskQueue;


        public CcrsTry(Action tryThis) : this(tryThis, new DispatcherQueue()) {}
        internal CcrsTry(Action tryThis, DispatcherQueue taskQueue)
        {
            this.tryThis = tryThis;
            this.taskQueue = taskQueue;
        }


        public void Catch(Action<Exception> exceptionHandler)
        {
            using (new CcrsCatch(exceptionHandler, this.taskQueue))
            {
                this.tryThis();
            }
        }

        public void Catch(ICcrsSimplexChannel<Exception> exceptionListener)
        {
            using (new CcrsCatch(exceptionListener))
            {
                this.tryThis();
            }
        }
    }
}
