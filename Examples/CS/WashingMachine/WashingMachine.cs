

namespace StateForge.Examples.WashingMachine
{
    using System;
    using StateForge.StateMachine;

    public partial class WashingMachine
    {
        private WashingMachineContext context;

        public WashingMachine()
        {
            this.context = new WashingMachineContext(this);
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
            WashingMachine washingMachine = new WashingMachine();
            washingMachine.Start();
            washingMachine.WashingDone();
            washingMachine.Fault();
            washingMachine.DiagnoseSuccess();
            washingMachine.RinsingDone();
            washingMachine.SpinningDone();
            Environment.Exit(0);
        }
    }
}
