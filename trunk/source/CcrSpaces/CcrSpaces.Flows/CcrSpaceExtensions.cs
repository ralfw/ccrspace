using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows
{
    public static class CcrSpaceExtensions
    {
        public static CcrsFlow<TInput, TOutput> StartFlow<TInput, TOutput>(this ICcrSpace space)
        {
            //TODO: how to allow for taskqueue and handler mode config of each stage? how to set defaults?
            //return new CcrsFlow<TInput, TOutput>()
            return null;
        }
    }
}
