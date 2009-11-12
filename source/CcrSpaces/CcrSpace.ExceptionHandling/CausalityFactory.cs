using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.ExceptionHandling
{
    internal static class CausalityFactory
    {
        public static ICausality CreateExceptionHandlingCausality(Action<Exception> exceptionHandler)
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

            return CreateExceptionHandlingCausality(pEx);
        }

        public static ICausality CreateExceptionHandlingCausality(Port<Exception> exceptionPort)
        {
            ICausality cEx = new Causality("CCR Space Exception Handler", exceptionPort);
            return cEx;
        }
    }
}
