using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using StateForge.StateMachine;

namespace StateForge.Examples.CallManager
{
    public partial class CallDirector
    {
        private CallControl callControl;
        private CallScenario callScenario;
        private CallDirectorContext context;
        private static readonly string traceName = "CallDirector";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public CallDirector()
        {
            InitializeCallControl();
            InitializeEndpoint(this.callControl);
            InitializeScenario(this.callControl);
            InitializeContext();
        }

        private void InitializeScenario(CallControl callControl)
        {
            this.callScenario = new CallScenario(this.callControl);
            this.callScenario.EndHandler += new EventHandler<EventArgs>(CallScenarioEnd);
        }

        private void InitializeContext()
        {
            this.context = new CallDirectorContext(this.callControl, this.callScenario);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
        }

        private void InitializeCallControl()
        {
            this.callControl = new CallControl();
            this.callControl.StartedHandler += new StartedEventHandler(CallControlStarted);
            this.callControl.StoppedHandler += new StoppedEventHandler(CallControlStopped);
        }

        private void InitializeEndpoint(CallControl callControl)
        {
            EndpointTest endpointAlice = new EndpointTest();
            endpointAlice.Name = "Alice";
            EndpointTest endpointBeatrice = new EndpointTest();
            endpointBeatrice.Name = "Bob";

            callControl.EndpointAdd(endpointAlice);
            callControl.EndpointAdd(endpointBeatrice);
        }

        private void CallControlStarted(object sender, EventArgs e)
        {
            ts.TraceInformation("CallControlStarted");
            this.context.CallControlStarted();
        }

        private void CallControlStopped(object sender, EventArgs e)
        {
            ts.TraceInformation("CallControlStopped");
            this.context.CallControlStopped();
        }

        private void CallScenarioEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("CallScenarioEnd");
            this.context.CallScenarioEnd();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("CallDirector State machine has ended");
            autoResetEvent.Set();
        }

        static void Main(string[] args)
        {
            CallDirector director = new CallDirector();
            director.StartExecution();

            autoResetEvent.WaitOne();
            Environment.Exit(0);
        }
    }
}
