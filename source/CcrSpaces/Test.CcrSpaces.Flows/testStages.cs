using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.Channels.Extensions;
using CcrSpaces.Flows;
using CcrSpaces.Flows.Stages;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Flows
{
    [TestFixture]
    public class testStages : TestFixtureBase
    {
        [Test]
        public void Terminating_stage_consumes_message()
        {
            var sut = new TerminalStage<string>(new CcrsOneWayChannelConfig<string>
                                                    {
                                                        MessageHandler= s => base.are.Set()
                                                    });

            sut.Post(new StageMessage{Message="hello"});

            Assert.IsTrue(base.are.WaitOne(500));
        }


        [Test]
        public void Intermediate_stage_passes_message_on_to_next_stage()
        {
            string result = "";

            var sut = new IntermediateStage<string, string>(new CcrsIntermediateFlowStageConfig<string, string>
                                                                {
                                                                    InputMessageHandler=(s, ps) => ps.Post(s)
                                                                });
            sut.Next = new TerminalStage<string>(new CcrsOneWayChannelConfig<string>
                                                     {
                                                         MessageHandler = s => { result = s; base.are.Set(); }
                                                     });

            sut.Post(new StageMessage {Message = "hello"});

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.AreEqual("hello", result);
        }


        [Test]
        public void Intermediate_stage_throws_exception_if_next_stage_is_missing()
        {
            var sut = new IntermediateStage<string, string>(new CcrsIntermediateFlowStageConfig<string, string>
                                                                {
                                                                    InputMessageHandler = (s, ps) => ps.Post(s)
                                                                });

            var pEx = new Port<Exception>();
            pEx.RegisterGenericSyncReceiver(ex => base.are.Set());
            ICausality c = new Causality("ex", pEx);
            Dispatcher.AddCausality(c);

            sut.Post(new StageMessage { Message = "hello" });

            Assert.IsTrue(base.are.WaitOne(1000));
            Dispatcher.RemoveCausality(c);
        }


        [Test]
        public void Terminating_stage_returns_result()
        {
            string result = "";

            var sut = new IntermediateStage<string, string>(new CcrsIntermediateFlowStageConfig<string, string>
                                                                {
                                                                    InputMessageHandler = (s, ps) => ps.Post(s)
                                                                });

            var pResult = new Port<string>();
            pResult.RegisterGenericSyncReceiver(s => { result = (string)s; base.are.Set(); });
            
            sut.Post(new StageMessage { Message = "hello", ResponsePort=pResult });

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.AreEqual("hello", result);
        }
    }
}
