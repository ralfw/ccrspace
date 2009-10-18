using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.ExceptionHandling;
using Microsoft.Ccr.Core;

namespace CcrSpaces.ExceptionHandling
{
    public class CcrsCausality : IDisposable
    {
        private readonly ICausality causality;


        internal CcrsCausality(Action<Exception> exceptionHandler)
            : this(CausalityFactory.CreateExceptionHandlingCausality(exceptionHandler)) {}
        internal CcrsCausality(Port<Exception> exceptionPort)
            : this(CausalityFactory.CreateExceptionHandlingCausality(exceptionPort)) {}
        private CcrsCausality(ICausality causality)
        {
            this.causality = causality;
            Dispatcher.AddCausality(this.causality);
        }


        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispatcher.RemoveCausality(this.causality);
        }

        #endregion
    }
}
