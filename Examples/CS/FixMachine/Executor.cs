
namespace StateForge.Examples.FixMachine
{
    using QuickFix;
    using System.Diagnostics;
    using StateForge.StateMachine;
    /// <summary>
    /// Execute Fix orders
    /// </summary>
    public partial class Executor : QuickFix.Application
    {
        static TraceSource ts = new TraceSource("Executor");
        public string ConfigFileName { get; set; }
        private SocketAcceptor acceptor;
        private FixMachineActuator actuator;
        private ExecutorContext context;

        public Executor()
        {
            this.actuator = new FixMachineActuator();
            this.context = new ExecutorContext(this);
            this.context.Observer = ObserverTrace.Instance("Executor");
            this.context.EnterInitialState();
        }

        public void onCreate(SessionID sessionID)
        {
            ts.TraceInformation("onCreate  sessionID {0} ", sessionID.ToString());
            this.context.EvCreate(sessionID);
        }

        public void onLogon(SessionID sessionID)
        {
            ts.TraceInformation("onLogon  sessionID {0} ", sessionID.ToString());
            this.context.EvLogon(sessionID);
        }

        public void onLogout(SessionID sessionID)
        {
            ts.TraceInformation("onLogout  sessionID {0} ", sessionID.ToString());
            this.context.EvLogout(sessionID);
        }

        public void toAdmin(Message message, SessionID sessionID)
        {
            ts.TraceInformation("toAdmin  sessionID {0} ", sessionID.ToString());
            this.context.EvToAdmin(message, sessionID);
        }

        public void toApp(Message message, SessionID sessionID)
        {
            ts.TraceInformation("toApp  sessionID {0} ", sessionID.ToString());
            this.context.EvToApp(message, sessionID);
        }

        public void fromAdmin(Message message, SessionID sessionID)
        {
            ts.TraceInformation("fromAdmin  sessionID {0} ", sessionID.ToString());
            this.context.EvFromAdmin(message, sessionID);
        }

        public void fromApp(Message message, SessionID sessionID)
        {
            ts.TraceInformation("fromApp  sessionID {0} ", sessionID.ToString());
            this.context.EvFromApp(message, sessionID);
        }

        public void Start()
        {
            SessionSettings settings = new SessionSettings(ConfigFileName);
            FileStoreFactory storeFactory = new FileStoreFactory(settings);
            FileLogFactory logFactory = new FileLogFactory(settings);
            MessageFactory messageFactory = new DefaultMessageFactory();
            this.acceptor = new SocketAcceptor(this, storeFactory, settings, logFactory /*optional*/, messageFactory);
            this.acceptor.start();
        }

        public void Stop()
        {
            this.acceptor.stop();
        }

    }
}
