

namespace StateForge.Examples.CallManager
{
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System;

    public partial class CallScenario
    {
        public event EventHandler<EventArgs> EndHandler;

        private CallBase01Config config;
        private CallBase01Context context;

        private static readonly string traceName = "CallScenario";
        private static TraceSource ts = new TraceSource(traceName);

        public CallScenario(CallControl callControl)
        {
            this.config = new CallBase01Config();
            this.context = new CallBase01Context(callControl, this, this.config);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("CallScenario State machine has ended");
            if (EndHandler != null) {
                EndHandler(this, EventArgs.Empty);
            }   
        }
    }

    public class CallBase01Config
    {
        public long TestMaxDuration { get; set; }
        public long RingDuration { get; set; }
        public long CallDuration { get; set; }

        public CallBase01Config()
        {
            TestMaxDuration = 30000;
            RingDuration = 2000;
            CallDuration = 5000;
        }
    }

}
