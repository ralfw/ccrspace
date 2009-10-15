using System;
using CcrSpaces.ExceptionHandling;
using CcrSpaces.Core;
using CcrSpaces.ExceptionHandling;

namespace CcrSpaces.ExceptionHandling
{
    public static class CcrsTryCatchExtensions
    {
        public static CcrsTryCatch Try(this ICcrSpace space, Action tryThis)
        {
            return new CcrsTryCatch(tryThis);
        }


        public static CcrsCausality SpanCausality(this ICcrSpace space, Action<Exception> exceptionHandler)
        {
            return new CcrsCausality(exceptionHandler);
        }
    }
}
