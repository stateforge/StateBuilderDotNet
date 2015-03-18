
namespace StateForge.Examples.FixMachine
{
    using QuickFix;
    using StateForge.StateMachine;
    using System.Diagnostics;

    public class Trader : QuickFix.Application
    {
        public string ConfigFileName { get; set; }
        static TraceSource ts = new TraceSource("Trader");
        private SocketInitiator initiator;
        private TraderContext context;

        public Trader()
        {
            this.context = new TraderContext(this);
            this.context.Observer = ObserverTrace.Instance("Trader");
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
            this.initiator = new SocketInitiator(this, storeFactory, settings, logFactory /*optional*/, messageFactory);
            this.initiator.start();
        }

        public void Stop() 
        {
            this.initiator.stop();
        }
    }
}
