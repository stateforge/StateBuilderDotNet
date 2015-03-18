

namespace StateForge.Examples.XmppChat
{
    using System;
    using System.Diagnostics;
    using agsXMPP;
    using agsXMPP.protocol.client;
    using StateForge.StateMachine;
    using agsXMPP.protocol.iq.roster;
    using agsXMPP.Xml.Dom;
    using System.Collections.Generic;

    public delegate void EndEventHandler(object sender, EventArgs e);

    public class ChatScenario
    {
        public event EndEventHandler EndHandler;

        public IChatScenario Context { get; set; }

        private string error;
        public string Error
        {
            get
            {
                return this.error;
            }
            set
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Error: {0}", value);
                this.error = value;
            }
        }

        public bool HasError()
        {
            return string.IsNullOrEmpty(this.error) == false;
        }

        public int MaxDuration { get; set; }
        public int IdleDuration { get; set; }
        public int SubscribeMaxDuration { get; set; }

        private static readonly string traceName = "ChatScenario";
        private static TraceSource ts = new TraceSource(traceName);

        Dictionary<string, Client> clientMap = new Dictionary<string, Client>();
        private int numberOfClients;
        private int numberOfClientsOpended;

        public ChatScenario()
        {
            MaxDuration = 30000;/* 30 sec */
            IdleDuration = 100;
            SubscribeMaxDuration = 5000;
        }

        public Client CreateClient(string user, string password, string server)
        {
            Client client = new Client(user, password, server);
            InitializeClient(client);
            numberOfClients++;
            this.clientMap.Add(client.Jid.ToString(), client);
            return client;
        }

        private void InitializeClient(Client client)
        {
            client.LoginHandler += new LoginEventHandler(OnLogin);
            client.CloseHandler += new CloseEventHandler(OnClose);
            client.RosterEndHandler += new RosterEndEventHandler(OnRosterEnd);
            client.AuthErrorHandler += new AuthErrorEventHandler(OnAuthError);
            client.MessageHandler += new MessageEventHandler(OnMessage);
            client.PresenceHandler += new PresenceEventHandler(OnPresence);
            client.SubscribeRequestHandler += new SubscribeRequestEventHandler(OnSubscribeRequest);
            client.SubscribeConfirmHandler += new SubscribeConfirmEventHandler(OnSubscribeConfirm);
            client.ErrorHandler += new ErrorEventHandler(OnError);
            client.RosterItemHandler += new RosterItemEventHandler(OnRosterItem);
            client.RosterEndHandler += new RosterEndEventHandler(OnRosterEnd);
            client.IqErrorHandler += new IqErrorEventHandler(OnIqError);
        }

        public void OpenAll()
        {
            foreach (KeyValuePair<string, Client> kvp in this.clientMap)
            {
                kvp.Value.Open();
            }
        }

        public void CloseAll()
        {
            foreach (KeyValuePair<string, Client> kvp in this.clientMap)
            {
                kvp.Value.Close();
            }
        }

        public void Start()
        {
            this.Context.Start();
        }

        private void OnLogin(Object o, Jid jid)
        {
            this.Context.LoggedIn(jid);
            numberOfClientsOpended++;
            if (numberOfClientsOpended == numberOfClients)
            {
                this.Context.LoggedInAll();
            }
        }

        private void OnClose(Object o, Jid jid)
        {
            this.Context.Closed(jid);
            numberOfClientsOpended--;
            if (numberOfClientsOpended == 0)
            {
                this.Context.ClosedAll();
            }
        }

        private void OnIqError(Object o, Jid jid, Error error)
        {
            this.Context.IqError(jid, error);
        }

        private void OnAuthError(Object o, Jid jid, Element element)
        {
            this.Context.AuthError(jid, element);
        }

        private void OnMessage(Object o, Jid jid, Message message)
        {
            this.Context.MessageRx(jid, message);
        }

        private void OnPresence(Object o, Jid jid, Presence presence)
        {
            this.Context.PresenceRx(jid, presence);
        }

        private void OnSubscribeRequest(Object o, Jid jid, Jid from)
        {
            this.Context.SubscribeRequest(jid, from);
        }

        private void OnSubscribeConfirm(Object o, Jid jid, Jid from)
        {
            this.Context.SubscribeConfirm(jid, from);
        }

        private void OnError(Object o, Jid jid, System.Exception exception)
        {
            this.Context.Error(jid, exception);
        }

        private void OnRosterItem(Object o, Jid jid, RosterItem item)
        {
            this.Context.RosterItem(jid, item);
        }

        private void OnRosterEnd(Object o, Jid jid)
        {
            this.Context.RosterEnd(jid);
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            if (EndHandler != null)
            {
                EndHandler(this, EventArgs.Empty);
            }
        }
    }
}
