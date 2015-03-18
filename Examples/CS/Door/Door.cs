namespace StateForge.Examples.Door
{
    using System;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Threading;

    public interface IEngine
    {
        void StartOpen();
        void StartClose();
        void Stop();
    }

    public class Engine : IEngine
    {
        private static TraceSource ts = new TraceSource("Engine");

        public void StartOpen()
        {
            ts.TraceInformation("StartOpen");
        }

        public void StartClose()
        {
            ts.TraceInformation("StartClose");
        }

        public void Stop()
        {
            ts.TraceInformation("Stop");
        }
    }

    public partial class Door
    {
        public IEngine Engine;
    
        public DoorContext context;
        private static readonly string traceName = "Door";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public Door()
        {
            this.Engine = new Engine();
            InitializeContext();
        }

        public Door(IEngine engine)
        {
            this.Engine = engine;
            InitializeContext();
        }

        private void InitializeContext()
        {
            this.context = new DoorContext(Engine);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("State machine has ended");
            autoResetEvent.Set();
        }

        public void Test01()
        {
            this.Start();
            this.OpenRequest();
            this.Quit();
            autoResetEvent.WaitOne();
        }

        static void Main(string[] args)
        {
            var door = new Door();
            door.Test01();
            ts.TraceInformation("Door has ended");
            Environment.Exit(0);
        }
    }
}
