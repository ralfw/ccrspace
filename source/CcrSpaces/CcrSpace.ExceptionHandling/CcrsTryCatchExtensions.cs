using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;

namespace CcrSpace.ExceptionHandling
{
    public static class CcrsTryCatchExtensions
    {
        public static CcrsTryCatch Try(this ICcrSpace space, Action tryThis)
        {
            return new CcrsTryCatch(tryThis);
        }
    }
}
