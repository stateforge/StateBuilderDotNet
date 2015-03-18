
//
// This class is a basic framework to test a state machine with NUnit and RhinoMock.
//

namespace StateForge.Examples.Turnstile
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    public class TurnstileTest
    {
        private TurnstileContext context;
        private Barrier barrier;
        private Alarm alarm;
        private CoinMachine coinMachine;
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "TurnstileContext";
        private string current, next, transition;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            this.barrier = repository.StrictMock<Barrier>();
            this.alarm = repository.StrictMock<Alarm>();
            this.coinMachine = repository.StrictMock<CoinMachine>();
            observer = repository.StrictMock<IObserver>();
            this.context = new TurnstileContext(this.barrier, this.alarm, this.coinMachine);
            this.context.Observer = observer;
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Coin()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EnterInitialState();
                    Expect.Call(() => this.observer.OnEntry(contextName, "Turnstile"));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Locked"));
                    Expect.Call(() => this.barrier.DoLock());

                    // this.context.Coin();
                    current = "Locked"; next = "Unlocked"; transition = "Coin";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.coinMachine.DoAccept());
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.barrier.DoUnlock());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.context.EnterInitialState();
                this.context.Coin();
            }
        }

        [Test]
        public void Pass()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    current = "Locked"; next = "Locked"; transition = "Pass";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, next, next, transition));
                    Expect.Call(() => this.alarm.DoAlertStaff());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, next, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.context.Pass();
            }
        }
        [Test]
        public void CoinPass()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    //this.context.Coin();
                    current = "Locked"; next = "Unlocked"; transition = "Coin";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.coinMachine.DoAccept());
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.barrier.DoUnlock());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    //this.context.Pass();
                    current = "Unlocked"; next = "Locked"; transition = "Pass";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Locked"));
                    Expect.Call(() => this.barrier.DoLock());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.context.Coin();
                this.context.Pass();
            }
        }

        [Test]
        public void CoinCoin()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.Coin();
                    current = "Locked"; next = "Unlocked"; transition = "Coin";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.coinMachine.DoAccept());
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.barrier.DoUnlock());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.Coin();
                    current = "Unlocked"; next = "Unlocked"; transition = "Coin";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.coinMachine.DoReject());
                    Expect.Call(() => this.alarm.DoRing());
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                this.context.Coin();
                this.context.Coin();
            }
        }
    }
}
