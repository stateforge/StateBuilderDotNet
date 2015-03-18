//
// * Change the namespace Company.Product in MyObject.cs and in MyObject.fsmcs.
// * Rename MyObject.cs and MyObject.fsmcs by right clicking on the file in the solution explorer.
// 
//


namespace Company.Product
{
    using System;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Threading;
    
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
	private static readonly string traceName = "$safeprojectname$";
        private static TraceSource ts = new TraceSource(traceName);
	
        public void DoOn()
        {
            ts.TraceInformation("DoOn");
        }

        public void DoOff()
        {
            ts.TraceInformation("DoOff");
        }
    }

    /// <summary>
    /// $safeprojectname$ is a container class which hold instances of a $safeprojectname$Actuator and a generated $safeprojectname$Context class.
    /// </summary>
    public class $safeprojectname$ {
        private $safeprojectname$Actuator actuator;
        private $safeprojectname$Context context;
        private static readonly string traceName = "$safeprojectname$";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
	
        public $safeprojectname$()
        {
            this.actuator = new $safeprojectname$Actuator();
            this.context = new $safeprojectname$Context(actuator);
	    this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EndEventHandler(StateMachineEnd);
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

	private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("State machine has ended");
            autoResetEvent.Set();
        }
	
        static void Main(string[] args)
        {
            $safeprojectname$ my$safeprojectname$ = new $safeprojectname$();
            my$safeprojectname$.On();
            my$safeprojectname$.Off();
	    autoResetEvent.WaitOne();
            Environment.Exit(0);
        }
    }
}
