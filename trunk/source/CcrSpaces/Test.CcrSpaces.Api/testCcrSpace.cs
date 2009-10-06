using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Flows;
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
        public void Create_oneway_channel()
        {
            using(var s = new CcrSpace())
            {
                var ch = s.CreateChannel<int>(n => this.are.Set());

                ch.Post(1);

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Create_reqSingleResp_channel()
        {
            using (var s = new CcrSpace())
            {
                var ch = s.CreateChannel<int, bool>(n => true);

                ch.Post(1, r => this.are.Set());

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Create_reqMultiResp_channel()
        {
            using (var s = new CcrSpace())
            {
                var ch = s.CreateChannel<int, int>((n,p) => { p.Post(n + 1); p.Post(n + 2);});

                ch.Post(1, r => this.are.Set());

                Assert.IsTrue(this.are.WaitOne(500));
                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Fluent_creation_of_oneWayChannel()
        {
            using(var space = new CcrSpace())
            {
                CcrsOneWayChannel<int> ch = space.CreateChannel<int>()
                    .Process(n => this.are.Set())
                    .Sequentially()
                    .WithOwnTaskQueue();

                ch.Post(1);

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Create_publisher()
        {
            using(var space = new CcrSpace())
            {
                var pub = space.CreatePublisher<int>();
                pub.Subscribe(n => this.are.Set());

                pub.Post(1);

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
                              var ch = s.CreateChannel<bool>(b =>
                                         {
                                             throw new ApplicationException("extest");
                                         });
                              ch.Post(true);
                          })
                    .Catch(ex =>
                               {
                                   Assert.AreEqual("extest", ex.Message);
                                   this.are.Set();
                               });

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }


        [Test]
        public void Fluent_creation_of_onewayflow()
        {
            using(var space = new CcrSpace())
            {
                CcrsFlow<string> f = space.CreateFlow<string>()
                    .Do(space.CreateChannel<string, int>(s => s.Length))
                    .Do(space.CreateChannel<int>(n => { Console.WriteLine(n); this.are.Set(); }));

                f.Post("hello");

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }
    }
}
