//
// * Change the namespace Company.Product in MyObject.cs and in MyObject.fsmcs.
// * Rename MyObject.cs and MyObject.fsmcs by right clicking on the file in the solution explorer.
// 
//


namespace StateForge.Examples.Persistence
{
    using System;
    using System.IO;
    using StateForge.StateMachine;

    /// <summary>
    /// IPeristenceActuator is an interface that is used by the Persistence class to perform actions.
    /// This interface is set in the state machine description through the attribute //StateMachine/settings/object@class
    /// </summary>
    public interface IPeristenceActuator
    {
        void DoOn();
        void DoOff();
    }

    /// <summary>
    /// PeristenceActuator is a concrete implementation of the IPeristenceActuator interface.
    /// </summary>
    public class PeristenceActuator : IPeristenceActuator
    {
        public void DoOn()
        {
            Console.WriteLine("DoOn");
        }

        public void DoOff()
        {
            Console.WriteLine("DoOff");
        }
    }

    /// <summary>
    /// Persistence is a container class which hold instances of a PeristenceActuator and a generated PeristenceContext class.
    /// </summary>
    public class Persistence
    {
        private PeristenceActuator actuator;
        private PersistenceContext context;

        public Persistence()
        {
            this.actuator = new PeristenceActuator();
            this.context = new PersistenceContext(actuator);
            this.context.Observer = ObserverConsole.Instance;
            this.context.EnterInitialState();
        }

        public void On()
        {
            this.context.EvOn();
        }

        public void Off()
        {
            this.context.EvOff();
        }

        static void Main(string[] args)
        {
            string fileName = "statemachine.dat";
            {
                PeristenceActuator actuator = new PeristenceActuator();
                PersistenceContext context = new PersistenceContext(actuator);
                context.EvOn();
                Console.WriteLine(context.StateCurrent.Name);
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    context.Serialize(streamWriter);
                }
            }
            {
                PeristenceActuator actuator = new PeristenceActuator();
                PersistenceContext context = new PersistenceContext(actuator);
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    context.DeSerialize(streamReader);
                }

                Console.WriteLine(context.StateCurrent.Name);
               
            }

            Environment.Exit(0);
        }
    }
}
