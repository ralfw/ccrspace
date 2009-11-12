using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Core.Core;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneralTestInfrastructure
{
    [TestFixture]
    public class TestFixtureBase
    {
        protected MockRepository mocks;
        protected AutoResetEvent are;

        protected DispatcherQueue dpq;
        protected ICcrSpace mockSpace;


        [SetUp]
        public void GlobalArrange()
        {
            this.are = new AutoResetEvent(false);

            this.mocks = new MockRepository();
            this.mockSpace = mocks.Stub<ICcrSpace>();
            this.dpq = new DispatcherQueue();
            mockSpace.Expect(x => x.DefaultTaskQueue).Return(this.dpq).Repeat.Any();

            FixtureArrange();
        }


        protected virtual void FixtureArrange(){}
    }
}
