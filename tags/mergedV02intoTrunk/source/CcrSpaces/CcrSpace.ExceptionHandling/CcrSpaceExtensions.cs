﻿using System;
using CcrSpaces.Core;

namespace CcrSpaces.ExceptionHandling
{
    public static class CcrSpaceExtensions
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