using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public class CcrSpace : ICcrSpace
    {
        protected readonly Dispatcher defaultDispatcher;
        protected readonly DispatcherQueue defaultTaskQueue;


        public CcrSpace()
        {
            this.defaultDispatcher = new Dispatcher();
            this.defaultTaskQueue = new DispatcherQueue("DefaultTaskQueue", this.defaultDispatcher);
        }


        #region Implementation of ICcrSpace

        public DispatcherQueue DefaultTaskQueue
        {
            get { return this.defaultTaskQueue; }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.defaultDispatcher.Dispose();
        }

        #endregion

    }
}
