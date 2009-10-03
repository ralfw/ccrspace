using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsTry
    {
        private readonly Action tryThis;


        internal CcrsTry(Action tryThis)
        {
            this.tryThis = tryThis;
        }


        public void Catch(Action<Exception> exceptionHandler)
        {
            var cfg = new CcrsOneWayChannelConfig<Exception>
                          {
                              MessageHandler = exceptionHandler,
                              TaskQueue = new DispatcherQueue(),
                              ProcessSequentially = true
                          };
            var exListener = new CcrsOneWayChannel<Exception>(cfg);
            Catch(exListener);
        }

        public void Catch(ICcrsSimplexChannel<Exception> exceptionListener)
        {
            var c = new Causality("TryCatch", exceptionListener);
            Dispatcher.AddCausality(c);
            try
            {
                this.tryThis();
            }
            finally
            {
                Dispatcher.RemoveCausality(c);
            }
        }
    }
}
