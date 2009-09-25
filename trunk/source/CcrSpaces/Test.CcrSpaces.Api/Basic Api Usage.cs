using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class ApiUsage
    {
        [Test]
        public void Create_CcrSpace()
        {
            using(new CcrSpace())
            {}
        }


        [Test]
        public void Create_one_way_listener()
        {
            using(var space = new CcrSpace())
            {
                var listener = space.CreateListener<string>(Console.WriteLine);
                listener.Post("hello");
            }
        }

        [Test]
        public void Create_request_response_listener()
        {
            using (var space = new CcrSpace())
            {
                var returnListener = space.CreateListener<string, int>(s => 
                                                                          { 
                                                                            Console.WriteLine("request: {0}", s);
                                                                            return s.Length; 
                                                                          });
                returnListener.Post("hello", n => Console.WriteLine("response: {0}", n));


                var paramListener = space.CreateListener<string, int>((s, ch) =>
                                                                          {
                                                                              Console.WriteLine("request2: {0}", s);
                                                                              ch.Post(s.Length);
                                                                          });
                paramListener.Post("world", n => Console.WriteLine("response2: {0}", n));

                var listener = space.CreateListener<int>(n => Console.WriteLine("final: {0}", n));
                paramListener.Post("the quick brown fox", listener);
            }
        }


        [Test]
        public void Exception_handling()
        {
            using (var space = new CcrSpace())
            {
                var listener = space.CreateListener<string>(s =>
                                                                {
                                                                    if (s.IndexOf("x") >= 0)
                                                                        throw new ApplicationException("aaargghhh!");
                                                                    Console.WriteLine("no exception for: {0}", s);
                                                                });

                using (space.TryCatch(ex => Console.WriteLine("*** {0}", ex.Message)))
                {
                    listener.Post("hello");
                    listener.Post("xray");
                }


                var exListener = space.CreateListener<Exception>(ex => Console.WriteLine("*** {0}", ex.Message));
                using (space.TryCatch(exListener))
                {
                    listener.Post("xray");
                }
            }
        }


        [Test]
        public void Concatenate_listeners()
        {
            using(var space = new CcrSpace())
            {
                var stage1 = space.CreateListener<string, int>(s => s.Length);
                var stage2 = space.CreateListener<int>(Console.WriteLine);
                var stage3 = space.CreateListener<int, bool>(n => n%2==0);
                var stage4 = space.CreateListener<bool>(b => Console.WriteLine("out: {0}", b));
                var stage5 = space.CreateListener<bool, bool>(b => !b);

                var flow = stage1.Concat(stage2);
                flow.Post("hello");

                var flowWithOutput = stage1.Concat<bool>(stage3);
                flowWithOutput.Post("world", b => Console.WriteLine("even number of chars: {0}", b));

                var extendedFlow = flowWithOutput.Concat(stage4);
                extendedFlow.Post("the quick brown fox");

                var extendedFlowWithOutput = flowWithOutput.Concat<bool>(stage5);
                extendedFlowWithOutput.Post("jumps over", b => Console.WriteLine("not even: {0}"));
            }
        }


        [Test, Ignore]
        public void Create_named_listeners()
        {}


        [Test, Ignore]
        public void Config_ccr_space()
        {
            using(var space = CcrSpaces.Space()
                            .RunningDispatcher("mydispatcher", 10)
                            .SchedulingWithDispatcherQueue("mydispatcher")
                            .CatchingUnhandledExceptionAt(ex => Console.WriteLine(ex.Message))
                            .Create())
            {}
        }


        [Test, Ignore]
        public void Config_listeners()
        {
            using(var space = new CcrSpace())
            {
                var listener = space.Listener<string>()
                                    .WithName("dumptext")
                                    .ProcessWith(s => Console.WriteLine(s))
                                    .Sequentially(false)
                                    .InCurrentSyncContext(false)
                                    .ScheduledByDispatcherQueue()
                                    .Create();
            }
        }
    }
}
