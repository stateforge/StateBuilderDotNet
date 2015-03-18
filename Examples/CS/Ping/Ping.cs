
namespace StateForge.Examples.Ping
{
    using System;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Threading;
    using System.Text;
    using System.Net.NetworkInformation;

    /// <summary>
    /// Ping 
    /// </summary>
    public partial class Ping
    {
        public long Timeout { get; set; }
        public String Target { get; set; }
        public int Count { get; set; }
        public int Rx { get; set; }
        public int Tx { get; set; }

        private PingContext context;
        private static readonly string traceName = "Ping";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        PingOptions packetOptions = new PingOptions(50, true);
        byte[] packetData = Encoding.ASCII.GetBytes("................................");

        public Ping()
        {
            pingSender.PingCompleted += new PingCompletedEventHandler(PingComplete);
            this.context = new PingContext(this);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
            Reset();
        }

        //TODO HEEFRE
        public void DoCancel()
        {
            this.pingSender.SendAsyncCancel();
        }

        internal void Send()
        {
            ts.TraceInformation("Ping: Send to {0}", this.Target);
            pingSender.SendAsyncCancel();
            pingSender.SendAsync(this.Target, (int)this.Timeout, this);
        }

        internal void PingComplete(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ts.TraceInformation("Ping was canceled");

            }

            else if (e.Error != null)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "An error occured: {0}", e.Error);
                this.context.EvError();
            }

            else
            {
                PingReply pingResponse = e.Reply;
                ShowPingResults(pingResponse);
            }
        }

        public void ShowPingResults(PingReply pingResponse)
        {
            if (pingResponse == null)
            {
                ts.TraceEvent(TraceEventType.Warning, 1, "There was no response");
                return;
            }
            else if (pingResponse.Status == IPStatus.Success)
            {
                ts.TraceEvent(TraceEventType.Information, 1, "Reply from {0}  RTT {1}",
                    pingResponse.Address.ToString(),
                    pingResponse.RoundtripTime);
                this.context.EvPingReply();
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, 1, "Ping was unsuccessful, ip status {0}", pingResponse.Status);
                this.context.EvError();
            }
        }

        private void Reset()
        {
            this.Target = "127.0.0.1";
            this.Timeout = 1000; /* msec */
            this.Count = 5;
            this.Rx = 0;
            this.Tx = 0;
        }

        public void PrintStatistics()
        {
            Console.WriteLine("Ping to {0}  attempt {1} of {2}, error: {3}", this.Target, this.Tx, this.Count, this.Tx - this.Rx);
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("State machine has ended");
            autoResetEvent.Set();
        }

        static void Main(string[] args)
        {
            Ping ping = new Ping();
            ping.Target = "127.0.0.1";
            ping.StartPing();
            autoResetEvent.WaitOne();
            ts.TraceInformation("Ping has ended");
            Environment.Exit(0);
        }
    }
}
