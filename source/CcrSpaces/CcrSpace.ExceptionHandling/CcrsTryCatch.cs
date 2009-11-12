using System;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.ExceptionHandling;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.ExceptionHandling
{
    public class CcrsTryCatch
    {
        private readonly Action tryThis;


        public CcrsTryCatch(Action tryThis)
        {
            this.tryThis = tryThis;
        }


        public void Catch(Action<Exception> exceptionHandler)
        { Catch(CausalityFactory.CreateExceptionHandlingCausality(exceptionHandler)); }
        public void Catch(Port<Exception> exceptionPort)
        { Catch(CausalityFactory.CreateExceptionHandlingCausality(exceptionPort)); }
        private void Catch(ICausality causality)
        {
            Dispatcher.AddCausality(causality);

            this.tryThis();

            Dispatcher.RemoveCausality(causality);
        }
    }
}
