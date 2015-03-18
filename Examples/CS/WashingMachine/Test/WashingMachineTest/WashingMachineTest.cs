
namespace StateForge.Examples.WashingMachine
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    public class WashingMachineTest
    {
        private WashingMachine washingMachine; 
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "WashingMachineContext";
        private string current, next, transition;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            observer = repository.StrictMock<IObserver>();
            this.washingMachine = new WashingMachine();
        }

        [Test]
        public void EvStart_EvFault_EvDiagnoseSuccess_EvWashingDone()
        {
            this.washingMachine.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.washingMachine.Start();
                    current = "Idle"; next = "Washing"; transition = "Start";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.Fault();
                    current = "Washing"; next = "OutOfService"; transition = "Fault";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.DiagnoseSuccess();
                    current = "OutOfService"; next = "Washing"; transition = "DiagnoseSuccess";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.WashingDone();
                    current = "Washing"; next = "Rinsing"; transition = "WashingDone";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.washingMachine.Start();
                this.washingMachine.Fault();
                this.washingMachine.DiagnoseSuccess();
                this.washingMachine.WashingDone();
            }
        }
        [Test]
        public void EvStart_EvWashingDone_EvFault_EvDiagnoseSuccess_EvRinsingDone()
        {
            this.washingMachine.Start();
            this.washingMachine.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    //this.washingMachine.WashingDone();
                    current = "Washing"; next = "Rinsing"; transition = "WashingDone";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.Fault();
                    current = "Rinsing"; next = "OutOfService"; transition = "Fault";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.DiagnoseSuccess();
                    current = "OutOfService"; next = "Rinsing"; transition = "DiagnoseSuccess";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.RinsingDone();
                    current = "Rinsing"; next = "Spinning"; transition = "RinsingDone";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.washingMachine.WashingDone();
                this.washingMachine.Fault();
                this.washingMachine.DiagnoseSuccess();
                this.washingMachine.RinsingDone();
            }
        }

        [Test]
        public void EvStart_EvWashingDone_EvRinsingDone_EvFault_EvDiagnoseSuccess_EvSpinningDone()
        {
            this.washingMachine.Start();
            this.washingMachine.WashingDone();
            this.washingMachine.Observer = observer;

            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    //this.washingMachine.RinsingDone();
                    current = "Rinsing"; next = "Spinning"; transition = "RinsingDone";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.Fault();
                    current = "Spinning"; next = "OutOfService"; transition = "Fault";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.DiagnoseSuccess();
                    current = "OutOfService"; next = "Spinning"; transition = "DiagnoseSuccess";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.washingMachine.SpinningDone();
                    current = "Spinning"; next = "End"; transition = "SpinningDone";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Running"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.washingMachine.RinsingDone();
                this.washingMachine.Fault();
                this.washingMachine.DiagnoseSuccess();
                this.washingMachine.SpinningDone();
            }
        }
    }
}
