
//
// This class is a basic framework to test a state machine with NUnit and RhinoMock.
// Here are the steps to get started:
// * Rename MyObjectTest.cs .
// * Change the namespace.
// * Add a Reference to NUnit which can found in ExternalDlls/NUnit  
// * Add a Reference to Rhino.Mocks which can found in ExternalDlls/RhinoMocks 
// * Add a Reference to your state machine. 
// * Replace MyContext with your context class.
// * Replace MyActuator with your actuator class.

// * Start to change the function with the [Test] attribute to implement your test
// * Compile the test project
// * Open the Nunit Gui runnner and select the test project dll

namespace Company.Product
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    class $safeprojectname$
    {
        private MyContext context;  //TODO change MyContext
        private MyActuator actuator;//TODO change MyActuator
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "MyContext";

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            this.actuator = repository.StrictMock<MyActuator>();//TODO change MyActuator
            observer = repository.StrictMock<IObserver>();
            this.context = new MyContext(this.actuator); //TODO change MyContext
            this.context.Observer = observer;
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void On()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EnterInitialState();
                    Expect.Call(() => this.observer.OnEntry(contextName, "Off"));
                    Expect.Call(() => this.actuator.DoOff());

                    // this.context.EvOn();
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, "Off", "On", "EvOn"));
                    Expect.Call(() => this.observer.OnExit(contextName, "Off"));
                    Expect.Call(() => this.observer.OnEntry(contextName, "On"));
                    Expect.Call(() => this.actuator.DoOn());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, "Off", "On", "EvOn"));
                }
            }

            using (repository.Playback())
            {
                this.context.EnterInitialState();
                this.context.EvOn();
            }
        }
    }
}
