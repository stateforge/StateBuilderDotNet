

namespace StateForge.Examples.CallManager
{

    public partial class EndpointTest : Endpoint
    {
        EndpointTestContext context;
        public EndpointTestConfig Config {get;set; }

        public EndpointTest()
        {
            this.Config = new EndpointTestConfig();
            this.context = new EndpointTestContext(this, this.Config);
        }

        public override void Open()
        {
            ts.TraceInformation("Open {0}", Name);
            this.context.Open();
        }

        public override void Close()
        {
            ts.TraceInformation("Close {0}", Name);
            this.context.Close();
        }

        internal void Opened()
        {
            ts.TraceInformation("Opened {0}", Name);
            EndpointEvent.EndpointOpened();
        }

        internal void Closed()
        {
            ts.TraceInformation("Closed {0}", Name);
            EndpointEvent.EndpointClosed();
        }

        public override void Setup(string to, string token)
        {
            ts.TraceInformation("Setup from {0} to {1}", Name, to);
            ConnectionTest connectionTest = CreateConnection();
            
        }

        protected ConnectionTest CreateConnection()
        {
            return new ConnectionTest();
        }
    }

    public class EndpointTestConfig
    {
        public long DurationOpen { get; set; }
        public long DurationClose { get; set; }

        public EndpointTestConfig()
        {
            DurationOpen = 100;
            DurationClose = 100;
        }
    }
}
