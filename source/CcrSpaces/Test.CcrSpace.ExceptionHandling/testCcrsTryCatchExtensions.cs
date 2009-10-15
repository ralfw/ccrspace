using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using Rhino.Mocks;
using CcrSpace.ExceptionHandling;

namespace Test.CcrSpace.ExceptionHandling
{
    [TestFixture]
    public class testCcrsTryCatchExtensions : TestFixtureBase
    {
        [Test]
        public void Try_and_catch()
        {
            mocks.ReplayAll();

            base.mockSpace.Try(()=>
                                   {
                                       var p = new Port<int>();
                                       Arbiter.Activate(
                                           new DispatcherQueue(),
                                           p.Receive(n => { throw new ApplicationException("error!");  })
                                           );
                                       p.Post(1);
                                   })
                .Catch(ex => base.are.Set());

            Assert.IsTrue(base.are.WaitOne(500));
        }
    }
}
