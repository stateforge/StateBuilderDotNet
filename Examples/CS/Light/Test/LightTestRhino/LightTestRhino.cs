

namespace StateForge.Examples.Light
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    public class LightTestRhino
    {
        private LightContext lightContext;
        private MockRepository repository;
        private ILightActuator lightActuator;
        private IObserver observer;
        private static String contextName = "LightContext";
        private string current, next, transition;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            lightActuator = repository.StrictMock<ILightActuator>();
            observer = repository.StrictMock<IObserver>();
            lightContext = new LightContext(lightActuator);
            lightContext.Observer = observer;
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
                    current = "Off"; next = "On"; transition = "EvOn";
                    Expect.Call(() => observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => observer.OnExit(contextName, current));
                    Expect.Call(() => observer.OnEntry(contextName, next));
                    Expect.Call(() => lightActuator.DoOn());
                    Expect.Call(() => observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                lightContext.EvOn();
            }
        }

        [Test]
        public void Off()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                }
            }

            using (repository.Playback())
            {
                lightContext.EvOff();
            }
        }
        [Test]
        public void OnOff()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // lightContext.EvOn();
                    current = "Off"; next = "On"; transition = "EvOn";
                    Expect.Call(() => observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => observer.OnExit(contextName, current));
                    Expect.Call(() => observer.OnEntry(contextName, next));
                    Expect.Call(() => lightActuator.DoOn());
                    Expect.Call(() => observer.OnTransitionEnd(contextName, current, next, transition));

                    // lightContext.EvOff();
                    current = "On"; next = "Off"; transition = "EvOff";
                    Expect.Call(() => observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => observer.OnExit(contextName, current));
                    Expect.Call(() => observer.OnEntry(contextName, next));
                    Expect.Call(() => lightActuator.DoOff());
                    Expect.Call(() => observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                lightContext.EvOn();
                lightContext.EvOff();
            }
        }

        [Test]
        public void OnOn()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // lightContext.EvOn();
                    current = "Off"; next = "On"; transition = "EvOn";
                    Expect.Call(() => observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => observer.OnExit(contextName, current));
                    Expect.Call(() => observer.OnEntry(contextName, next));
                    Expect.Call(() => lightActuator.DoOn());
                    Expect.Call(() => observer.OnTransitionEnd(contextName, current, next, transition));
                }
            }

            using (repository.Playback())
            {
                lightContext.EvOn();
                lightContext.EvOn();
            }
        }
    }
}
