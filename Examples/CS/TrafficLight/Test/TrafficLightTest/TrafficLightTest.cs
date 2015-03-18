
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

namespace StateForge.Examples.TrafficLight
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;
    using System.Threading;
    using System.Diagnostics;

    [TestFixture]
    class TrafficLightTest
    {
        private TrafficLightContext context;  
        private Light light;
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "TrafficLightContext";
        private static TraceSource ts = new TraceSource("TrafficLight");
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private string current, next, transition;

        public TrafficLightTest()
        {
            Setup();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            Console.WriteLine("State machine has ended");
            autoResetEvent.Set();
        }

        static void Main(string[] args)
        {
            TrafficLightTest trafficLightTest = new TrafficLightTest();
            trafficLightTest.NormalOperation();
            autoResetEvent.WaitOne(2000);
            ts.TraceInformation("Test has ended");
            Environment.Exit(0);
        }

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            this.light = new Light();
            this.observer = repository.StrictMock<IObserver>();
            this.context = new TrafficLightContext(this.light);
            this.context.Observer = this.observer;
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void NormalOperation()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EnterInitialState();
                    Expect.Call(() => this.observer.OnEntry(contextName, "TrafficLight"));
                    Expect.Call(() => this.observer.OnTimerStart(contextName, "MaxDuration", light.TimerMaxDuration));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Red"));
                    Expect.Call(() => this.observer.OnTimerStart(contextName, "TimerRed", light.TimerRedDuration));
                    // this.context.EvTimerRed();
                    current = "Red"; next = "Green"; transition = "EvTimerRed";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnTimerStop(contextName, "TimerRed"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTimerStart(contextName, "TimerGreen", light.TimerGreenDuration));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvTimerGreen();
                    current = "Green"; next = "Yellow"; transition = "EvTimerGreen";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTimerStart(contextName, "TimerYellow", light.TimerYellowDuration));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvTimerYellow();
                    current = "Yellow"; next = "Red"; transition = "EvTimerYellow";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTimerStart(contextName, "TimerRed", light.TimerRedDuration));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvTimerMaxDuration();
                    current = "Red"; next = "End"; transition = "EvTimerMaxDuration";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnTimerStop(contextName, "TimerRed"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                Console.WriteLine("Entering state machine");
                this.context.EnterInitialState();
                Console.WriteLine("Wait for end of state machine");
                autoResetEvent.WaitOne(2000);
            }
        }

        
    }
}
