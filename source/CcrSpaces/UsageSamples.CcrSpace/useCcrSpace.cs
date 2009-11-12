using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using NUnit.Framework;
using CcrSpaces.Core.Core;

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


        [Test]
        public void Exception_handling()
        {
            using (var space = new CcrSpace())
            {
                using (space.SpanCausality(ex => Console.WriteLine(ex.Message)))
                {
                    // do stuff with ports
                    // exceptions are caught by the above exception handler
                }


                space.Try(() =>
                              {
                                  // do stuff with ports
                                  // exceptions are caught by the following exception handler
                              })
                    .Catch(ex => Console.WriteLine(ex.Message));
            }
        }
    }
}
