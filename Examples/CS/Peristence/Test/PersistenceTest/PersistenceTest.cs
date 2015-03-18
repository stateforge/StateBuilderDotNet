
namespace StateForge.Examples.Persistence
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using StateForge.StateMachine;

    [TestFixture]
    class PersistenceTest
    {
        private PeristenceActuator actuator;

        private void Serialize(ContextBase context, string fileName)
        {
            using (StreamWriter streamWriter = File.CreateText(fileName))
            {
                context.Serialize(streamWriter);
            }
        }

        private void DeSerialize(ContextBase context, string fileName)
        {
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                context.DeSerialize(streamReader);
            }
        }

        [SetUp]
        public void Setup()
        {
            this.actuator = new PeristenceActuator();
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void PersistenceOn()
        {
            // Create, change state and Serialize
            PersistenceContext context = new PersistenceContext(this.actuator);
            Assert.AreEqual(context.StateCurrent.Name, "Off");
            context.EvOn();
            Assert.AreEqual(context.StateCurrent.Name, "On");
            string fileName = context.Name + "On.dat";
            Serialize(context, fileName);

            // Create new , DeSerialize and check state
            PersistenceContext contextSerialized = new PersistenceContext(actuator);
            Assert.AreEqual(contextSerialized.StateCurrent.Name, "Off");
            DeSerialize(contextSerialized, fileName);
            Assert.AreEqual(contextSerialized.StateCurrent.Name, "On");
        }

        [Test]
        public void PersistenceOff()
        {
            // Create and Serialize
            PersistenceContext context = new PersistenceContext(this.actuator);
            context.EnterInitialState();
            Assert.AreEqual(context.StateCurrent.Name, "Off");
           
            string fileName = context.Name + "Off.dat";
            Serialize(context, fileName);

            // Create new , DeSerialize and check state
            PersistenceContext contextSerialized = new PersistenceContext(actuator);
            DeSerialize(contextSerialized, fileName);
            contextSerialized.EnterInitialState();
            Assert.AreEqual(contextSerialized.StateCurrent.Name, "Off");
        }

        [Test]
        public void ParallelPersistence01()
        {
            // Create and Serialize
            PersistenceParallelContext context = new PersistenceParallelContext(this.actuator);
            context.Observer = ObserverConsole.Instance;
            context.EnterInitialState();

            Assert.AreEqual(context.StateCurrent.Name, "Running");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A1");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B1");

            context.Ev12();

            Assert.AreEqual(context.StateCurrent.Name, "Running");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A2");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B2");

            string fileName = context.Name + "O1.dat";
            Serialize(context, fileName);

            // Create new , DeSerialize and check state
            PersistenceParallelContext contextSerialized = new PersistenceParallelContext(actuator);
            DeSerialize(contextSerialized, fileName);
            contextSerialized.EnterInitialState();

            Assert.AreEqual(contextSerialized.StateCurrent.Name, "Running");
            Assert.AreEqual(contextSerialized.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A2");
            Assert.AreEqual(contextSerialized.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B2");

            contextSerialized.Ev23();
            Assert.AreEqual(contextSerialized.StateCurrent.Name, "Idle");
        }

        [Test]
        public void Parallel01()
        {
            // Create and Serialize
            PersistenceParallelContext context = new PersistenceParallelContext(this.actuator);
            context.Observer = ObserverConsole.Instance;
            context.EnterInitialState(); // always call  EnterInitialState for state machine which have parallel state as the first state

            Assert.AreEqual(context.StateCurrent.Name, "Running");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A1");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B1");

            context.Ev12();

            Assert.AreEqual(context.StateCurrent.Name, "Running");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A2");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B2");

            context.Ev23();
            Assert.AreEqual(context.StateCurrent.Name, "Idle");

            // Go back to Running
            context.EvRunning();
            Assert.AreEqual(context.StateCurrent.Name, "Running");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessAContext.StateCurrent.Name, "A1");
            Assert.AreEqual(context.PersistenceParallelRunningParallel.PersistenceParallelProcessBContext.StateCurrent.Name, "B1");

            //Stop
            context.EvStop();
            Assert.AreEqual(context.StateCurrent.Name, "End");

            //End state is a final state, impossible to get out of it.
            context.Ev12();
            Assert.AreEqual(context.StateCurrent.Name, "End");
            context.Ev23();
            Assert.AreEqual(context.StateCurrent.Name, "End");
            context.EvRunning();
            Assert.AreEqual(context.StateCurrent.Name, "End");
        }

        static void Main(string[] args)
        {
            var test = new PersistenceTest();
            test.Setup();
            test.Parallel01();
        }
    }
}
