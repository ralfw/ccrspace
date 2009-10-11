using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CcrSpaces.Core;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useCcrSpace
    {
        [Test]
        public void Create_space()
        {
            // create CCR space with a default thread pool (CCR Dispatcher)
            // and a default dispatcher queue
            using(var space = new CcrSpace())
            {
                // do stuff with the CCR Space
            }
        }
    }
}
