﻿using System;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useChannels
    {
        [Test]
        public void Create_oneway_channel()
        {
            using(var space = new CcrSpace())
            {
                // all channels are served by threads from the CCR space
                // default thread pool

                // sequential channel
                // it´s persistent, ie. it processes all messages sent to it
                Port<int> p;
                p = space.CreateChannel<int>(Console.WriteLine);
                p.Post(1);

                // parallel channel
                p = space.CreateChannel<int>(Console.WriteLine, 
                                             CcrsChannelHandlerModes.Parallel);
                for (int i = 100; i < 120; i++)
                    p.Post(i);

                // a channel processing messages in the sync context
                // of its creator. use this to access GUI controls during message processing
                p = space.CreateChannel<int>(Console.WriteLine,
                                             CcrsChannelHandlerModes.InCurrentSyncContext);

                Thread.Sleep(2000);
            }
        }


        [Test]
        public void Create_request_response_channel()
        {
            using (var space = new CcrSpace())
            {
                // provide request and response handlers upon creation
                var p = space.CreateChannel<string, int>(s => s.Length, Console.WriteLine);
                p.Post("hello"); // watch the output window for the result

                // defer decision on response handler until request
                p = space.CreateChannel<string, int>(s => s.Length);
                p.Request("hello, world").Receive(Console.WriteLine);

                // return multiple results for each request
                var q = space.CreateChannel<string, string>((s, pw) =>
                                                                {
                                                                    foreach (string w in s.Split(' '))
                                                                        pw.Post(w);
                                                                });
                q.Request("the quick brown fox").Receive(Console.WriteLine);

                Thread.Sleep(2000);
            }
        }
    }
}