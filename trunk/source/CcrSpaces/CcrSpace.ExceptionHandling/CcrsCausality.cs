using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.ExceptionHandling;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.ExceptionHandling
{
    public class CcrsCausality : IDisposable
    {
        private readonly ICausality causality;


        public CcrsCausality(Action<Exception> exceptionHandler)
            : this(CausalityFactory.CreateExceptionHandlingCausality(exceptionHandler)) {}
        public CcrsCausality(Port<Exception> exceptionPort)
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
