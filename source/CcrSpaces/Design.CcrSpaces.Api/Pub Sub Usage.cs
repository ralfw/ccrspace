using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api;
using NUnit.Framework;

namespace Design.CcrSpaces.Api
{
    [TestFixture]
    public class Pub_Sub_Usage
    {
        [Test]
        public void Pub_and_sub()
        {
            using(var space = new CcrSpace())
            {
                var pub = space.CreatePublisher<string>();

                pub.Subscribe(s => Console.WriteLine("received publication: {0}", s));

                var listener = space.CreateListener<string>(s => Console.WriteLine("also received publication: {0}", s));
                pub.Subscribe(listener);

                pub.Post("hello");

                pub.Unsubscribe(listener);

                pub.Post("world");
            }
        }


        [Test, Ignore]
        public void Subscribe_with_filter()
        {}
    }
}
