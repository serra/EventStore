using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EventStore.NUnitTest_issue_143
{
    [TestFixture]
    public class EventsShouldNotBePersistedBetweenTestRuns_Issue143 : FullContextTestBase
    {
        Guid _streamId = new Guid("CC0FB6B5-9400-4070-B613-D90339025142");

        [Test]
        public void One_AddOneStreamAndMakeSureThereIsExactlyOneInStoreWhenDone()
        {
            using (var stream = Store.OpenStream(_streamId, 0, int.MaxValue))
            {
                Assert.AreEqual(0, stream.CommittedEvents.Count);

                var @event = new SomeDomainEvent {Value = "One"};

                stream.Add(new EventMessage {Body = @event});
                stream.CommitChanges(Guid.NewGuid());

                Assert.AreEqual(1, stream.CommittedEvents.Count);
            }
        }

        [Test]
        public void Two_AgainAddOneStreamAndMakeSureThereIsExactlyOneInStoreWhenDone()
        {
            using (var stream = Store.OpenStream(_streamId, 0, int.MaxValue))
            {
                Assert.AreEqual(0, stream.CommittedEvents.Count);
                var @event = new SomeDomainEvent { Value = "One" };

                stream.Add(new EventMessage { Body = @event });
                stream.CommitChanges(Guid.NewGuid());
                Assert.AreEqual(1, stream.CommittedEvents.Count);
            }

        }
    }

    public class FullContextTestBase
    {
        protected IStoreEvents Store;

        [SetUp]
        public void SetUpContext()
        {
            Store = GetInMemoryEventStore();
        }

        [TearDown]
        public void TearDown()
        {
            //_store.Advanced.Purge();
            Store.Dispose();
        }

        private IStoreEvents GetInMemoryEventStore()
        {
            return Wireup.Init()
                         .LogToOutputWindow()
                         .UsingInMemoryPersistence()
                         .UsingSynchronousDispatchScheduler()
                         .Build();
        }
    }

    public class SomeDomainEvent
    {
        public string Value { get; set; }
    }
}
