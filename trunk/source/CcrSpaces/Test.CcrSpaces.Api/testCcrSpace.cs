using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testCcrSpace
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Create()
        {
            using(new CcrSpace())
            {}
        }

        [Test]
        public void Create_oneway_listener()
        {
            using(var s = new CcrSpace())
            {
                var l = s.CreateListener<int>(n => this.are.Set());

                l.Post(1);

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Create_reqSingleResp_listener()
        {
            using (var s = new CcrSpace())
            {
                var l = s.CreateListener<int, bool>(n => true);

                l.Post(1, r => this.are.Set());

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Create_reqMultiResp_listener()
        {
            using (var s = new CcrSpace())
            {
                var l = s.CreateListener<int, int>((n,p) => { p.Post(n + 1); p.Post(n + 2);});

                l.Post(1, r => this.are.Set());

                Assert.IsTrue(this.are.WaitOne(500));
                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Try_catch()
        {
            using(var s = new CcrSpace())
            {
                s.Try(() =>
                          {
                              var l = s.CreateListener<bool>(b =>
                                         {
                                             throw new ApplicationException("extest");
                                         });
                              l.Post(true);
                          })
                    .Catch(ex =>
                               {
                                   Assert.AreEqual("extest", ex.Message);
                                   this.are.Set();
                               });

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }
    }
}
