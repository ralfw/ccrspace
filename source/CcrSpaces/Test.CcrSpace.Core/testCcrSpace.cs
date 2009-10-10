using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Core
{
    [TestFixture]
    public class testCcrSpace
    {
        [Test]
        public void Init_space()
        {
            using(var space = new SeeThruCcrSpace())
            {
                Assert.IsNotNull(space.TheDispatcher);
                Assert.IsNotNull(space.DefaultTaskQueue);
                Assert.AreSame(space.TheDispatcher, space.DefaultTaskQueue.Dispatcher);
            }
        }
    }

    class SeeThruCcrSpace : global::CcrSpace.Core.CcrSpace
    {
        public Dispatcher TheDispatcher { get { return base.defaultDispatcher;  } }
    }
}
