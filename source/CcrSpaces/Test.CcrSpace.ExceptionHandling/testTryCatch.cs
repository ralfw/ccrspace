using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.ExceptionHandling;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.ExceptionHandling
{
    [TestFixture]
    public class testTryCatch : TestFixtureBase
    {
        [Test]
        public void Catch_exception()
        {
            Exception exCaught=null;
            var p = new Port<int>();

            new CcrsTryCatch(() =>
                             {
                                 Arbiter.Activate(
                                     new DispatcherQueue(),
                                     p.Receive(n =>{
                                                       throw new ApplicationException("error!");
                                                   })
                                     );

                                 p.Post(1);
                             })
                .Catch(ex =>
                           {
                               exCaught = ex;
                               base.are.Set();
                           });

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.IsInstanceOf<ApplicationException>(exCaught);
        }
    }
}
