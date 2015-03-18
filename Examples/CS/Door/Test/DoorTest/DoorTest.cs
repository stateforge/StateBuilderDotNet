
namespace StateForge.Examples.Door.Test
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using StateForge.StateMachine;
    using NUnit.Framework;
    using Rhino.Mocks;

    public class EngineMock : IEngine
    {
        public IEngine Context { get; set; }

        public EngineMock()
        {
        }

        public void StartOpen()
        {
            this.Context.StartOpen();
        }

        public void StartClose()
        {
            this.Context.StartClose();
        }

        public void Stop()
        {
            this.Context.Stop();
        }
    }

    /// <summary>
    /// DoorTest tests the Door class.
    /// </summary>
    [TestFixture]
    public class DoorTest
    {
        public int DurationOpening { get; set; }
        public int DurationOpenCosing { get; set; }
        private Door door;
        private EngineMock engineMock;
        private static readonly string traceName = "Test";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public DoorTest()
        {
            Setup();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("Test State machine has ended");
            autoResetEvent.Set();
        }

        static void Main(string[] args)
        {
            DoorTest doorTest = new DoorTest();
            doorTest.TestMethod01();
            ts.TraceInformation("Test has ended");
            Environment.Exit(0);
        }

        private void InitializeContext(ContextBase contextTest)
        {
            contextTest.Observer = ObserverTrace.Instance(traceName);
            contextTest.RegisterEndHandler(new EventHandler<EventArgs>(StateMachineEnd));
            contextTest.EnterInitialState(); 
        }

        [SetUp]
        public void Setup()
        {
            DurationOpening = 2000;
            DurationOpenCosing = 2000;
            this.engineMock = new EngineMock();
            this.door = new Door(engineMock);
        }

        [Test]
        public void TestMethod01()
        {
            var contextTest = new DoorTest01Context(this, door);
            this.engineMock.Context = contextTest;
            InitializeContext(contextTest);
            autoResetEvent.WaitOne();
        }

        [Test]
        public void TestMethod02()
        {
            var contextTest = new DoorTest02Context(this, door);
            this.engineMock.Context = contextTest;
            InitializeContext(contextTest);
            autoResetEvent.WaitOne();
        }
    }
}
