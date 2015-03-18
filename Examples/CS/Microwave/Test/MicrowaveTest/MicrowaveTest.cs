
namespace StateForge.Examples.Microwave
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    public class MicrowaveTest
    {
        private Microwave microwave;
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "MicrowaveContext";
        private static String contextEngineName = "MicrowaveEngineContext";
        private static String contextDoorName = "MicrowaveDoorContext";
        private string current, next, transition;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            observer = repository.StrictMock<IObserver>();
            this.microwave = new Microwave();
        }

        [TearDown]
        public void TearDown()
        {

        }
        /// <summary>
        /// Start form Initial.
        /// </summary>
        [Test]
        public void EvStart()
        {
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvStart();
                    current = "Initial"; next = "Operating"; transition = "EvStart";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));

                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));

                    // Entering in sub context engine
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, "Engine"));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, "EngineOff"));

                    //Entering in sub context door
                    Expect.Call(() => this.observer.OnEntry(contextDoorName, "Door"));
                    Expect.Call(() => this.observer.OnEntry(contextDoorName, "DoorClosed"));

                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvStart();
            }
        }
        /// <summary>
        /// Stop after start.
        /// </summary>
        [Test]
        public void EvStart_EvStop()
        {
            this.microwave.EvStart();
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvStop();
                    current = "Operating"; next = "End"; transition = "EvStop";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));

                    // Leaving sub context engine.
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "EngineOff"));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "Engine"));

                    // Leaving sub context door.
                    Expect.Call(() => this.observer.OnExit(contextDoorName, "DoorClosed"));
                    Expect.Call(() => this.observer.OnExit(contextDoorName, "Door"));

                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvStop();
            }
        }

        /// <summary>
        /// Turn on and open the door.
        /// </summary>
        [Test]
        public void EvStartEvTurnOnEvDoorOpened()
        {
            this.microwave.EvStart();
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvTurnOn();
                    current = "EngineOff"; next = "EngineCooking"; transition = "EvTurnOn";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextEngineName, current, next, transition));

                    Expect.Call(() => this.observer.OnExit(contextEngineName, current));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, "EngineOn"));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, next));

                    Expect.Call(() => this.observer.OnTransitionEnd(contextEngineName, current, next, transition));

                    // Engine: this.context.EvDoorOpened(); 
                    current = "EngineCooking"; next = "EngineWaitDoorToClose"; transition = "EvDoorOpened";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextEngineName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, current));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextEngineName, current, next, transition));

                    // Door: this.context.EvDoorOpened(); 
                    current = "DoorClosed"; next = "DoorOpened"; transition = "EvDoorOpened";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextDoorName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextDoorName, current));
                    Expect.Call(() => this.observer.OnEntry(contextDoorName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextDoorName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvTurnOn();
                this.microwave.EvDoorOpened();
            }
        }

        /// <summary>
        /// Turn off when after turn on.
        /// </summary>
        [Test]
        public void EvStart_EvTurnOn_EvTurnOff()
        {
            this.microwave.EvStart();
            this.microwave.EvTurnOn();
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvTurnOn();
                    current = "EngineCooking"; next = "EngineOff"; transition = "EvTurnOff";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextEngineName, current, next, transition));

                    Expect.Call(() => this.observer.OnExit(contextEngineName, current));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "EngineOn"));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, next));

                    Expect.Call(() => this.observer.OnTransitionEnd(contextEngineName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvTurnOff();
            }
        }

        /// <summary>
        /// Stop when after turn on.
        /// </summary>
        [Test]
        public void EvStart_EvTurnOn_EvStop()
        {
            this.microwave.EvStart();
            this.microwave.EvTurnOn();
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvStop();
                    current = "Operating"; next = "End"; transition = "EvStop";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));

                    // Leaving sub context engine
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "EngineCooking"));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "EngineOn"));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, "Engine"));
                    // Leaving sub context door
                    Expect.Call(() => this.observer.OnExit(contextDoorName, "DoorClosed"));
                    Expect.Call(() => this.observer.OnExit(contextDoorName, "Door"));

                    // Leaving context
                    Expect.Call(() => this.observer.OnExit(contextName, current));

                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvStop();
            }
        }

        /// <summary>
        /// Turn on when the door is opened, then close the door.
        /// </summary>
        [Test]
        public void EvStartEvDoorOpenedEvTurnOn()
        {
            this.microwave.EvStart();
            
            this.microwave.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {

                    // Door: this.context.EvDoorOpened(); 
                    current = "DoorClosed"; next = "DoorOpened"; transition = "EvDoorOpened";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextDoorName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextDoorName, current));
                    Expect.Call(() => this.observer.OnEntry(contextDoorName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextDoorName, current, next, transition));
                    
                    // this.context.EvTurnOn();
                    current = "EngineOff"; next = "EngineWaitDoorToClose"; transition = "EvTurnOn[context.ContextParent.MicrowaveOperatingParallel.MicrowaveDoorContext.StateCurrent == MicrowaveDoorOpenedState.Instance]";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextEngineName, current, next, transition));

                    Expect.Call(() => this.observer.OnExit(contextEngineName, current));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, "EngineOn"));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, next));

                    Expect.Call(() => this.observer.OnTransitionEnd(contextEngineName, current, next, transition));

                    // Engine: this.context.EvDoorClosed(); 
                    current = "EngineWaitDoorToClose"; next = "EngineCooking"; transition = "EvDoorClosed";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextEngineName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextEngineName, current));
                    Expect.Call(() => this.observer.OnEntry(contextEngineName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextEngineName, current, next, transition));

                    // Door: this.context.EvDoorClosed(); 
                    current = "DoorOpened"; next = "DoorClosed"; transition = "EvDoorClosed";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextDoorName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextDoorName, current));
                    Expect.Call(() => this.observer.OnEntry(contextDoorName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextDoorName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.microwave.EvDoorOpened();
                this.microwave.EvTurnOn();
                this.microwave.EvDoorClosed();
            }
        }
    }
}
