

namespace StateForge.Examples.XmppChat
{
    using System;
    using agsXMPP;
    using agsXMPP.protocol.client;
    using System.Diagnostics;
    using agsXMPP.protocol.iq.roster;
    using agsXMPP.Xml.Dom;
    using agsXMPP.sasl;

    public delegate void LoginEventHandler(object sender, Jid jid);
    public delegate void CloseEventHandler(object sender, Jid jid);
    public delegate void MessageEventHandler(object sender, Jid jid, Message message);
    public delegate void PresenceEventHandler(object sender, Jid jid, Presence presence);
    public delegate void SubscribeRequestEventHandler(object sender, Jid jid, Jid from);
    public delegate void SubscribeConfirmEventHandler(object sender, Jid jid, Jid from);
    public delegate void ErrorEventHandler(object sender, Jid jid, Exception exception);
    public delegate void RosterItemEventHandler(object sender, Jid jid, RosterItem item);
    public delegate void RosterEndEventHandler(object sender, Jid jid);

    public delegate void SaslStartEventHandler(object sender, Jid jid, SaslEventArgs e);
    public delegate void AuthErrorEventHandler(object sender, Jid jid, Element element);

    public delegate void IqErrorEventHandler(object sender, Jid jid, Error error);

    public class Client
    {
        public event LoginEventHandler LoginHandler;
        public event CloseEventHandler CloseHandler;
        public event MessageEventHandler MessageHandler;
        public event PresenceEventHandler PresenceHandler;
        public event SubscribeRequestEventHandler SubscribeRequestHandler;
        public event SubscribeConfirmEventHandler SubscribeConfirmHandler;
        public event ErrorEventHandler ErrorHandler;
        public event RosterItemEventHandler RosterItemHandler;
        public event RosterEndEventHandler RosterEndHandler;
        public event SaslStartEventHandler SaslStartHandler;
        public event AuthErrorEventHandler AuthErrorHandler;
        public event IqErrorEventHandler IqErrorHandler;

        public XmppClientConnection xmppClientConnection { get; set; }
        public Jid Jid { get; set; }
        private string password;

        private static readonly string traceName = "Client";
        private static TraceSource ts = new TraceSource(traceName);

        public Client(string user, string password, string server)
        {
            this.Jid = new Jid(user, server, null);
            this.password = password;

            InitializeConnection();
        }

        public void Open()
        {
            ts.TraceInformation("Start {0}", this.Jid.ToString());
            try
            {
                this.xmppClientConnection.Open();
            }
            catch (System.Exception exception)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Start:  {0}", exception.Message);
            }
        }

        public void Close()
        {
            ts.TraceInformation("Close {0}", this.Jid.ToString());
            this.xmppClientConnection.Close();
        }

        public void RegisterNewAccount()
        {
            this.xmppClientConnection.RegisterAccount = true;
            Open();
        }

        public void SendMyPresence()
        {
            ts.TraceInformation("SendMyPresence {0}", this.Jid.ToString());
            this.xmppClientConnection.SendMyPresence();
        }

        /// <summary>
        /// <presence to='juliet@example.com' type='subscribe'/>
        /// </summary>
        /// <param name="to"></param>
        public void SubscriptionRequest(Jid to)
        {
            ts.TraceInformation("SubscriptionRequest {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.RosterManager.AddRosterItem(to);
            this.xmppClientConnection.PresenceManager.Subscribe(to);
        }

        /// <summary>
        /// <presence to='romeo@example.net' type='subscribed'/>
        /// </summary>
        /// <param name="to"></param>
        public void SubscriptionConfirm(Jid to)
        {
            ts.TraceInformation("SubscriptionConfirm {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.RosterManager.AddRosterItem(to);
            this.xmppClientConnection.PresenceManager.ApproveSubscriptionRequest(to);
        }

        /// <summary>
        /// <presence to='juliet@example.com' type='unsubscribe'/>
        /// </summary>
        /// <param name="to"></param>
        public void UnSubscribe(Jid to)
        {
            ts.TraceInformation("UnSubscribe {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.PresenceManager.Unsubscribe(to);
        }

        /// <summary>
        /// <presence to='romeo@example.net' type='unsubscribed'/>
        /// </summary>
        /// <param name="to"></param>
        public void SubscriptionRemove(Jid to)
        {
            ts.TraceInformation("SubscriptionRemove {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.PresenceManager.RefuseSubscriptionRequest(to);
        }

        /// <summary>
        /// <iq id="agsXMPP_4" type="set"><Query xmlns="jabber:iq:roster"><item jid="bob@example.net" /></Query></iq>
        /// </summary>
        /// <param name="to"></param>
        public void RosterAddItem(Jid to)
        {
            ts.TraceInformation("RosterAddItem {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.RosterManager.AddRosterItem(to);
        }

        /// <summary>
        /// <iq id="agsXMPP_5" type="set"><Query xmlns="jabber:iq:roster"><item jid="bob@xmpp-server" subscription="remove" /></Query></iq>
        /// </summary>
        /// <param name="to"></param>
        public void RosterRemoveItem(Jid to)
        {
            ts.TraceInformation("RosterRemoveItem {0} to {1}", this.Jid.ToString(), to.ToString());
            this.xmppClientConnection.RosterManager.RemoveRosterItem(to);
        }

        public void RosterRequest()
        {
            ts.TraceInformation("RosterRequest {0}", this.Jid.ToString());
            this.xmppClientConnection.RequestRoster();
        }

        public void AddRosterItem(Jid jid)
        {
            ts.TraceInformation("AddRosterItem {0} to {1}", this.Jid.ToString(), jid);
            this.xmppClientConnection.RosterManager.AddRosterItem(jid);
        }

        public void SendChat(Jid to, string messageText)
        {
            ts.TraceInformation("SendChat from {0} to {1}, msg: {2}", this.Jid.ToString(), to.ToString(), messageText);
            var message = new Message(to, MessageType.chat, messageText);
            this.xmppClientConnection.Send(message);
        }

        private void InitializeConnection()
        {
            this.xmppClientConnection = new XmppClientConnection();
            this.xmppClientConnection.AutoAgents = false;
            this.xmppClientConnection.AutoPresence = true;
            this.xmppClientConnection.AutoRoster = true;
            this.xmppClientConnection.AutoResolveConnectServer = false;

            this.xmppClientConnection.Password = this.password;
            this.xmppClientConnection.Username = Jid.User;
            this.xmppClientConnection.Server = Jid.Server;
            this.xmppClientConnection.ConnectServer = Jid.Server;
            //this.xmppClientConnection.UseSSL = false;
            //this.xmppClientConnection.UseStartTLS = false;
            //this.xmppClientConnection.UseCompression = false;
            this.xmppClientConnection.OnRosterStart += new ObjectHandler(OnRosterStart);
            this.xmppClientConnection.OnRosterItem += new XmppClientConnection.RosterHandler(OnRosterItem);
            this.xmppClientConnection.OnRosterEnd += new ObjectHandler(OnRosterEnd);
            this.xmppClientConnection.OnPresence += new PresenceHandler(OnPresence);
            this.xmppClientConnection.OnMessage += new MessageHandler(OnMessage);
            this.xmppClientConnection.OnLogin += new ObjectHandler(OnLogin);
            this.xmppClientConnection.OnClose += new ObjectHandler(OnClose);
            this.xmppClientConnection.OnError += new ErrorHandler(OnError);
            this.xmppClientConnection.OnAuthError += new XmppElementHandler(OnAuthError);
            //this.xmppClientConnection.OnSaslStart += new SaslEventHandler(OnSaslStart);
            this.xmppClientConnection.OnIq += new IqHandler(OnIq);
            this.xmppClientConnection.OnReadXml += new XmlHandler(OnReadXml);
            this.xmppClientConnection.OnWriteXml += new XmlHandler(OnWriteXml);
            this.xmppClientConnection.ClientSocket.OnValidateCertificate += new System.Net.Security.RemoteCertificateValidationCallback(OnValidateCertificate);
            this.xmppClientConnection.OnSocketError += new ErrorHandler(OnSocketError);
            this.xmppClientConnection.OnStreamError += new XmppElementHandler(OnStreamError);
        }


        private void OnReadXml(object sender, string xml)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "OnReadXml {0}", this.Jid.ToString());
            ts.TraceEvent(TraceEventType.Verbose, 1, "{0}", xml);
        }

        private void OnWriteXml(object sender, string xml)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "OnWriteXml {0}", this.Jid.ToString());
            ts.TraceEvent(TraceEventType.Verbose, 1, "{0}", xml);
        }

        void OnStreamError(object sender, Element e)
        {
            ts.TraceEvent(TraceEventType.Error, 1, "OnStreamError {0}, error: {1}",
                this.Jid.ToString(),
                e.ToString());
        }

        private void OnSocketError(object sender, Exception exception)
        {
            ts.TraceEvent(TraceEventType.Error, 1, "OnSocketError {0}, exception: {1}",
                this.Jid.ToString(),
                exception.Message);
        }

        private bool OnValidateCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            ts.TraceInformation("OnValidateCertificate {0}", this.Jid.ToString(), certificate.ToString());
            return true;
        }

        private void OnIq(object sender, agsXMPP.protocol.client.IQ iq)
        {
            if (iq != null)
            {
                if (iq.Error != null)
                {
                    OnIqError(iq);
                }

                // No Iq with Query
                if (iq.HasTag(typeof(agsXMPP.protocol.extensions.si.SI)))
                {
                    if (iq.Type == IqType.set)
                    {
                        agsXMPP.protocol.extensions.si.SI si = iq.SelectSingleElement(typeof(agsXMPP.protocol.extensions.si.SI)) as agsXMPP.protocol.extensions.si.SI;

                        agsXMPP.protocol.extensions.filetransfer.File file = si.File;
                        if (file != null)
                        {
                            // somebody wants to send a file to us
                            Console.WriteLine(file.Size.ToString());
                            Console.WriteLine(file.Name);
                        }
                    }
                }
                else
                {
                    Element query = iq.Query;

                    if (query != null)
                    {
                        if (query.GetType() == typeof(agsXMPP.protocol.iq.version.Version))
                        {
                            OnIqVersion(iq, query);
                        }
                        else if (query.GetType() == typeof(agsXMPP.protocol.iq.register.Register))
                        {
                            OnIqRegister(iq, query);
                        }
                    }
                }
            }
        }

        private void OnIqError(agsXMPP.protocol.client.IQ iq)
        {
            ts.TraceInformation("OnIqError {0} error type {1}, code: {2}",
                                this.Jid.ToString(), iq.Error.Type, iq.Error.Code);
            if (IqErrorHandler != null)
            {
                IqErrorHandler(this, this.Jid, iq.Error);
            }
        }

        private void OnIqRegister(agsXMPP.protocol.client.IQ iq, Element query)
        {
            ts.TraceInformation("OnIqRegister {0} Query: {1}", this.Jid.ToString(), query.ToString());

        }

        private void OnIqVersion(agsXMPP.protocol.client.IQ iq, Element query)
        {
            ts.TraceInformation("OnIqVersion {0}", this.Jid.ToString());
            // its a version IQ VersionIQ
            agsXMPP.protocol.iq.version.Version version = query as agsXMPP.protocol.iq.version.Version;
            if (iq.Type == IqType.get)
            {
                iq.SwitchDirection();
                iq.Type = IqType.result;

                version.Name = "StateForgeClient";
                version.Ver = "0.1";
                version.Os = Environment.OSVersion.ToString();

                this.xmppClientConnection.Send(iq);
            }
        }
        private void OnRosterStart(object sender)
        {
            ts.TraceInformation("OnRosterStart {0}", this.Jid.ToString());
        }

        private void OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {

            ts.TraceInformation("OnRosterItem {0} ask {1}, from {2}", this.Jid.ToString(), item.Ask, item.Jid.ToString());

            if (item.Ask == AskType.subscribe)
            {
                //ts.TraceInformation("OnRosterItem {0} ask=subscribe", this.Jid.ToString(), item.ToString());
                if (SubscribeConfirmHandler != null)
                {
                    SubscribeConfirmHandler(this, this.Jid, item.Jid);
                }
            }

            if (RosterItemHandler != null)
            {
                RosterItemHandler(this, this.Jid, item);
            }
        }

        private void OnRosterEnd(object sender)
        {
            ts.TraceInformation("OnRosterEnd {0}", this.Jid.ToString());
            if (RosterEndHandler != null)
            {
                RosterEndHandler(this, this.Jid);
            }
        }

        private void OnPresence(object sender, Presence presence)
        {
            ts.TraceInformation("OnPresence {0}, type: {1}, error: {2}",
                this.Jid.ToString(), presence.Type, presence.Error);

            if (presence.Type == PresenceType.subscribe)
            {
                if (SubscribeRequestHandler != null)
                {
                    SubscribeRequestHandler(this, this.Jid, presence.From);
                }
            }

            else
            {
                if (PresenceHandler != null)
                {
                    PresenceHandler(this, this.Jid, presence);
                }
            }
        }

        private void OnMessage(object sender, Message message)
        {
            if (message.Error != null)
            {
                ts.TraceInformation("OnMessage {0} from {1} to {2}, error: {3}, body: {4}",
                                    this.Jid.ToString(), message.From, message.To, message.Error.Code, message.Body);
            }
            else
            {
                ts.TraceInformation("OnMessage {0} from {1} to {2}, body: {3}",
                                    this.Jid.ToString(), message.From, message.To, message.Body);
            }

            if (MessageHandler != null)
            {
                MessageHandler(this, this.Jid, message);
            }
        }
        private void OnError(object sender, System.Exception exception)
        {
            ts.TraceInformation("OnError {0}, exception:  {1}",
                this.Jid.ToString(), exception.Message);
            ts.TraceInformation("{0}", exception.StackTrace);

            if (ErrorHandler != null)
            {
                ErrorHandler(this, this.Jid, exception);
            }
        }

        private void OnLogin(object sender)
        {
            ts.TraceInformation("OnLogin {0}", this.Jid.ToString());
            if (LoginHandler != null)
            {
                LoginHandler(this, this.Jid);
            }
        }

        private void OnClose(object sender)
        {
            ts.TraceInformation("OnClose {0}", this.Jid.ToString());
            if (CloseHandler != null)
            {
                CloseHandler(this, this.Jid);
            }
        }

        void OnSaslStart(object sender, SaslEventArgs e)
        {
            ts.TraceInformation("OnSaslStart {0}", this.Jid.ToString());
            e.Auto = false;
            //e.SaslMechanism = Matrix.Xmpp.Sasl.SaslMechanism.NONE;
            if (SaslStartHandler != null)
            {
                SaslStartHandler(this, this.Jid, e);
            }
        }

        private void OnAuthError(object sender, Element element)
        {
            ts.TraceInformation("OnAuthError {0}, error {1}", this.Jid.ToString(), element.ToString());
            if (AuthErrorHandler != null)
            {
                AuthErrorHandler(this, this.Jid, element);
            }
        }
    }
}
