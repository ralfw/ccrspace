using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Test.CcrSpace.ExceptionHandling
{
    [TestFixture]
    public class testCcrSpaceExtensions : TestFixtureBase
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


        [Test]
        public void Using_catch()
        {
            using (base.mockSpace.SpanCausality(ex => base.are.Set()))
            {
                var p = new Port<int>();
                Arbiter.Activate(
                    new DispatcherQueue(),
                    Arbiter.Receive(
                        true,
                        p,
                        n =>
                          {
                              Console.WriteLine("received: {0}", n);
                              throw new ApplicationException("error!");
                          })
                    );
                p.Post(1);
                p.Post(2);
            }

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.IsTrue(base.are.WaitOne(1000));
        }
    }
}
