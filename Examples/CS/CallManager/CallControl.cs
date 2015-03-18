
namespace StateForge.Examples.CallManager
{
    using System;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.Generic;

    public delegate void StartedEventHandler(object sender, EventArgs e);
    public delegate void StoppedEventHandler(object sender, EventArgs e);

    public partial class CallControl : ICallAction, IEndpointEvent
    {
        public event StartedEventHandler StartedHandler;
        public event StoppedEventHandler StoppedHandler;

        public string Error { get; set; }
        public ICallEvent CallEvent { get; set; }
        public IManagementEvent ManagementEvent { get; set; }
        public Dictionary<string, string> tokens = new Dictionary<string, string>();
        public Dictionary<string, Endpoint> endpointMap = new Dictionary<string, Endpoint>();

        private CallControlContext context;
        private CallControlConfig config;
        private static readonly string traceName = "CallControl";
        private static TraceSource ts = new TraceSource(traceName);

        public int EndpointActive { get; internal set; }

        public CallControl()
        {
            this.config = new CallControlConfig();
            this.context = new CallControlContext(this, this.config);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
        }

        public void EndpointAdd(Endpoint endpoint)
        {
            ts.TraceInformation("EndpointAdd {0}", endpoint.Name);
            endpoint.EndpointEvent = this;
            if (endpointMap.ContainsKey(endpoint.Name) == false)
            {
                endpointMap[endpoint.Name] = endpoint;
            }
        }

        public void EndpointRemove(Endpoint endpoint)
        {
            ts.TraceInformation("EndpointRemove {0}", endpoint.Name);
            endpointMap.Remove(endpoint.Name);
        }

        public void SetToken(string name, string value)
        {
            this.tokens[name] = value;
        }

        internal void Started()
        {
            ts.TraceInformation("Started");
            if (this.StartedHandler != null)
            {
                StartedHandler(this, EventArgs.Empty);
            }
        }

        internal void Stopped()
        {
            ts.TraceInformation("Stopped");
            if (this.StoppedHandler != null)
            {
                StoppedHandler(this, EventArgs.Empty);
            }
        }

        internal void DoEndpointStart()
        {
            ts.TraceInformation("DoEndpointStart");
            this.EndpointActive = 0;
            foreach (KeyValuePair<string, Endpoint> kvp in this.endpointMap)
            {
                kvp.Value.Open();
            }
        }

        internal void DoEndpointStop()
        {
            ts.TraceInformation("DoEndpointStop");
            foreach (KeyValuePair<string, Endpoint> kvp in this.endpointMap)
            {
                kvp.Value.Close();
            }
        }

        protected Endpoint GetEndpoint(string name)
        {
            return endpointMap[name];
        }

        internal bool IsLastToOpen()
        {
            if (this.EndpointActive == this.endpointMap.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool IsLastToClose()
        {
            if (this.EndpointActive == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EndpointOpened()
        {
            this.EndpointActive++;
            ts.TraceInformation("EndpointOpened {0}/{1}", this.EndpointActive, this.endpointMap.Count);
            this.context.EndpointOpened();
        }

        public void EndpointClosed()
        {
            this.EndpointActive--;
            ts.TraceInformation("EndpointClosed {0}/{1}", this.EndpointActive, this.endpointMap.Count);
            this.context.EndpointClosed();
        }

        #region Actions
        public void Setup(string from, string to, string token)
        {
            ts.TraceInformation("Setup from {0} to {1}, token {2}", from, to, token);
            Endpoint endpointFrom = GetEndpoint(from);
            if (endpointFrom != null)
            {
                endpointFrom.Setup(to, token);
            }
            else 
            {

            }
        }

        public void Answer(string token)
        {
            ts.TraceInformation("Answer token {0}", token);
        }

        public void Connect(string token)
        {
            ts.TraceInformation("Connect token {0}", token);
        }

        public void Release(string token)
        {
            ts.TraceInformation("Release token {0}", token);
        }
        #endregion Actions

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("CallControl State machine has ended");
        }
    }

    public class CallControlConfig
    {
        public CallControlConfig()
        {
        }
    }
}
