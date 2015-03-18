//
// * Change the namespace Company.Product in MyObject.cs and in MyObject.fsmcs.
// * Rename MyObject.cs and MyObject.fsmcs by right clicking on the file in the solution explorer.
// 
//


namespace Company.Product
{
    using System;
    using StateForge.StateMachine;

    /// <summary>
    /// I$safeprojectname$Actuator is an interface that is used by the $safeprojectname$ class to perform actions.
    /// This interface is set in the state machine description through the attribute //StateMachine/settings/object@class
    /// </summary>
    public interface I$safeprojectname$Actuator
    {
        void DoOn();
        void DoOff();
    }

    /// <summary>
    /// $safeprojectname$Actuator is a concrete implementation of the I$safeprojectname$Actuator interface.
    /// </summary>
    public class $safeprojectname$Actuator : I$safeprojectname$Actuator
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
    /// $safeprojectname$ is a container class which hold instances of a $safeprojectname$Actuator and a generated $safeprojectname$Context class.
    /// </summary>
    public class $safeprojectname$ {
        private $safeprojectname$Actuator actuator;
        private $safeprojectname$Context context;

        public $safeprojectname$()
        {
            this.actuator = new $safeprojectname$Actuator();
            this.context = new $safeprojectname$Context(actuator);
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
            $safeprojectname$ my$safeprojectname$ = new $safeprojectname$();
            my$safeprojectname$.On();
            my$safeprojectname$.On();
            my$safeprojectname$.On();
            my$safeprojectname$.Off();
            Environment.Exit(0);
        }
    }
}
