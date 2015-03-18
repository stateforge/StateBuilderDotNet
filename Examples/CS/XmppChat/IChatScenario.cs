namespace StateForge.Examples.XmppChat
{
    using agsXMPP;
    using agsXMPP.protocol.client;
    using agsXMPP.protocol.iq.roster;
    using agsXMPP.Xml.Dom;

    public interface IChatScenario
    {
        void Start();
        void Error(Jid jid, System.Exception exception);

        void LoggedIn(Jid jid);
        void Closed(Jid jid);
        void AuthError(Jid jid, Element element);
        void MessageRx(Jid jid, Message message);
        void PresenceRx(Jid jid, Presence presence);
        void RosterItem(Jid jid, RosterItem item);
        void RosterEnd(Jid jid);
        void SubscribeRequest(Jid jid, Jid from);
        void SubscribeConfirm(Jid jid, Jid from);
        void IqError(Jid jid, Error error);
        void LoggedInAll();
        void ClosedAll();
    }
}
