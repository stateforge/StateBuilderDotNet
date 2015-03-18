
namespace StateForge.Examples.Led
{
    using System;
    using StateForge.StateMachine;

    public class Led
    {
        private LedContext context;

        public Led()
        {
            this.context = new LedContext(this);
            this.context.Observer = ObserverConsole.Instance;
            this.context.EnterInitialState();
        }

        #region Public Methods
        // On
        public void On()
        {
            context.On();
        }

        // Off
        public void Off()
        {
            context.Off();
        }

        #endregion

        #region Methods invoked by the context class
        // DoOn
        protected internal void DoOn()
        {
            Console.WriteLine("DoOn");
        }

        // DoOff
        protected internal void DoOff()
        {
            Console.WriteLine("DoOff");
        }

        #endregion
    }

    public class LedApp
    {
        static void Main(string[] args)
        {
            Led led = new Led();
            led.On();
            led.Off();
            
            Environment.Exit(0);
        }
    }
}
