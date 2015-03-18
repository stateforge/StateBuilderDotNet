
namespace StateForge.Examples.Microwave
{
    using System;
    using StateForge.StateMachine;

    /// <summary>
    /// IMicrowaveActuator is an interface that is used by the Microwave class to perform actions.
    /// This interface is set in the state machine description through the attribute //StateMachine/settings/object@class
    /// </summary>
    public interface IMicrowaveActuator
    {
        void DoOn();
        void DoOff();
    }

    /// <summary>
    /// MicrowaveActuator is a concrete implementation of the IMicrowaveActuator interface.
    /// </summary>
    public class MicrowaveActuator : IMicrowaveActuator
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
    /// Microwave is a container class which hold instances of a MicrowaveActuator and a generated MicrowaveContext class.
    /// </summary>
    public partial class Microwave
    {
        private MicrowaveContext context;

        public Microwave()
        {
            this.context = new MicrowaveContext(this);
            this.context.Observer = ObserverConsole.Instance;
            this.context.EnterInitialState();
        }

        public IObserver Observer
        {
            get
            {
                return this.context.Observer;
            }
            set
            {
                this.context.Observer = value;
            }
        }

        static void Main(string[] args)
        {
            Microwave microwave = new Microwave();
            microwave.EvStart();
            microwave.EvDoorOpened();
            microwave.EvTurnOn();
            microwave.EvDoorClosed();
            microwave.EvDoorClosed();
            microwave.EvCookingDone();
            microwave.EvDoorOpened();
            microwave.EvTurnOff();
            microwave.EvDoorOpened();
            microwave.EvTurnOn();
            microwave.EvDoorClosed();

            Environment.Exit(0);
        }
    }
}
