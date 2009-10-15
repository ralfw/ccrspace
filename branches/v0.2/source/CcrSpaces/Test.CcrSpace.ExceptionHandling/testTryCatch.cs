using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpace.ExceptionHandling;
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
                                 base.are.Set();

                                 Arbiter.Activate(
                                     new DispatcherQueue(),
                                     p.Receive(n =>
                                                   {
                                                       base.are.Set();
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

            Assert.IsTrue(base.are.WaitOne(500)); // try is executed
            Assert.IsTrue(base.are.WaitOne(500)); // port received msg, throws exception
            Assert.IsTrue(base.are.WaitOne(500)); // exception caught
            Assert.IsInstanceOf<ApplicationException>(exCaught);
        }
    }
}
