using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StateForge.StateMachine;
using Moq;

namespace StateForge.Examples.Light
{
    [TestFixture]
    public class LightTestMoq
    {
        private LightContext lightContext;
        private Mock<ILightActuator> mockLightActuator;
        private Mock<IObserver> mockObserver;
        private String contextName = "LightContext";

        [SetUp]
        public void Setup()
        {
            mockLightActuator = new Mock<ILightActuator>();
            mockObserver = new Mock<IObserver>();
            lightContext = new LightContext(mockLightActuator.Object);
            lightContext.Observer = mockObserver.Object;
        }

        [Test]
        public void Void()
        {
            mockLightActuator.Verify(la => la.DoOn(), Times.Exactly(0));
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(0));
            mockObserver.Verify(observer => observer.OnTransitionBegin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
            mockObserver.Verify(observer => observer.OnTransitionEnd(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
            mockObserver.Verify(observer => observer.OnEntry(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
            mockObserver.Verify(observer => observer.OnExit(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0)); 
        }

        [Test]
        public void On()
        {
            //Needed when Strict mode is used
            //mockLightActuator.Setup(la => la.DoOn());
            //mockObserver.Setup(observer => observer.OnTransitionBegin(contextName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            //mockObserver.Setup(observer => observer.OnExit(contextName, It.IsAny<string>()));
            //mockObserver.Setup(observer => observer.OnEntry(contextName, It.IsAny<string>()));
            //mockObserver.Setup(observer => observer.OnTransitionEnd(contextName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            lightContext.EvOn();
            
            mockLightActuator.Verify(la => la.DoOn(), Times.Exactly(1));
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(0));
            mockObserver.Verify(observer => observer.OnTransitionBegin(contextName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            mockObserver.Verify(observer => observer.OnExit(contextName, It.IsAny<string>()), Times.Exactly(1));
            mockObserver.Verify(observer => observer.OnEntry(contextName, It.IsAny<string>()), Times.Exactly(1));
            mockObserver.Verify(observer => observer.OnTransitionEnd(contextName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void OnOff()
        {
            lightContext.EvOn();
            lightContext.EvOff();
            mockLightActuator.Verify(la => la.DoOn(), Times.Exactly(1));
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(1));
        }

        [Test]
        public void Off()
        {
            lightContext.EvOff();
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(0));
        }

        [Test]
        public void OffOnOnOffOff()
        {
            lightContext.EvOff();
            lightContext.EvOn();
            lightContext.EvOn();
            lightContext.EvOff();
            lightContext.EvOff();
            mockLightActuator.Verify(la => la.DoOn(), Times.Exactly(1));
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(1));
        }

        [Test]
        public void OnOffOnOff()
        {
            lightContext.EvOn();
            lightContext.EvOff();
            lightContext.EvOn();
            lightContext.EvOff();
            mockLightActuator.Verify(la => la.DoOn(), Times.Exactly(2));
            mockLightActuator.Verify(la => la.DoOff(), Times.Exactly(2));
        }
    }
}
