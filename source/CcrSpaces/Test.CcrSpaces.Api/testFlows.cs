using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Config;
using CcrSpaces.Api.Flows;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testFlows
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }



        [Test]
        public void Create_config()
        {
            var cfg = new CcrsOneWayFlowConfig();
            cfg.AddStage(new CcrsRequestSingleResponseChannel<string, int>(s => s.Length));
            cfg.AddStage(new CcrsRequestSingleResponseChannel<int, bool>(n => n % 2 == 0));
            cfg.AddStage(new CcrsOneWayChannel<bool>(Console.WriteLine));

            Assert.AreEqual(2, cfg.IntermediateStages.Count);
            Assert.IsNotNull(cfg.FinalStage);
        }


        [Test]
        public void Create_onewayflow_from_config()
        {
            var cfg = new CcrsOneWayFlowConfig();
            cfg.AddStage(new CcrsRequestSingleResponseChannel<string, int>(s => s.Length));
            cfg.AddStage(new CcrsRequestSingleResponseChannel<int, int>(n => n+1));
            cfg.AddStage(new CcrsRequestSingleResponseChannel<int, bool>(n => n % 2 == 0));
            cfg.AddStage(new CcrsOneWayChannel<bool>(b => { Console.WriteLine(b); this.are.Set(); }));

            var flow = new CcrsFlow<string>(cfg);
            flow.Post("hello");

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Create_reqrespflow_from_config()
        {
            using (var space = new CcrSpace())
            {
                var cfg = new CcrsRequestResponseFlowConfig();
                cfg.AddStage(new CcrsRequestSingleResponseChannel<string, int>(s => s.Length));

                var flow = new CcrsFlow<string, int>(cfg);

                flow.Post("hello", n =>{
                                           Console.WriteLine(n);
                                           this.are.Set();
                                       });

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }
    }


    internal static class IEnumExtensions
    {
        public static void Foreach<T>(this IEnumerable<T> collecton, Action<T> command)
        {
            foreach (T item in collecton)
                command(item);
        }
    }
}
