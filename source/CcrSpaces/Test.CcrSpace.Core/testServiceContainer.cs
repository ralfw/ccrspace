using System;
using System.Linq;
using CcrSpaces.Core.Services;
using NUnit.Framework;

namespace Test.CcrSpace.Core
{
    [TestFixture]
    public class testServiceContainer
    {
        [Test]
        public void Register_service()
        {
            var sut = new ServiceRegistry();

            var s1 = new MyService1();
            sut.Register(s1);

            Assert.AreSame(s1, sut.Resolve<MyService1>());
        }

        [Test]
        public void Exception_on_unregistered_service_type()
        {
            var sut = new ServiceRegistry();

            Assert.Catch<IndexOutOfRangeException>(() => sut.Resolve<MyService1>());
        }


        [Test]
        public void Enum_services()
        {
            var sut = new ServiceRegistry();

            sut.Register(new MyService1());
            sut.Register(new MyService2());
            sut.Register(new MyService1());

            Assert.AreEqual(2, sut.GetAll<MyService1>().Count());
        }


        [Test]
        public void Dispose_resourceful_services()
        {
            var s1 = new MyResourcefulService1();
            var s2 = new MyResourcefulService2();

            using(var sut = new ServiceRegistry())
            {
                sut.Register(new MyService1());
                sut.Register(s1);
                sut.Register(new MyService2());
                sut.Register(s2);
            }

            Assert.IsTrue(s1.HasBeenDisposed);
            Assert.IsTrue(s2.HasBeenDisposed);
        }


        [Test]
        public void Access_named_service()
        {
            var sut = new ServiceRegistry();

            var s1 = new MyService1();
            sut.Register(s1, "s1");
            sut.Register(new MyService1());

            Assert.AreSame(s1, sut.Resolve<MyService1>("s1"));
        }
    }


    class ResourcefulServiceBase : ICcrsResourcefulService
    {
        public bool HasBeenDisposed;

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.HasBeenDisposed = true;
        }

        #endregion
    }

    class MyResourcefulService1 : ResourcefulServiceBase {}
    class MyResourcefulService2 : ResourcefulServiceBase { }

    class MyService1 : ICcrsService{}
    class MyService2 : ICcrsService { }
}
