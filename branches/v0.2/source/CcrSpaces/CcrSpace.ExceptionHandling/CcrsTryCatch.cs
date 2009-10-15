using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpace.ExceptionHandling
{
    public class CcrsTryCatch
    {
        private readonly Action tryThis;


        public CcrsTryCatch(Action tryThis)
        {
            this.tryThis = tryThis;
        }


        public void Catch(Action<Exception> exceptionHandler)
        {
            var pEx = new Port<Exception>();
            Arbiter.Activate(
                new DispatcherQueue(),
                Arbiter.Receive(
                    true,
                    pEx,
                    new Handler<Exception>(exceptionHandler)
                    )
                );

            ICausality cEx = new Causality("ExceptionHandler", pEx);
            Dispatcher.AddCausality(cEx);

            this.tryThis();

            Dispatcher.RemoveCausality(cEx);
        }
    }
}
