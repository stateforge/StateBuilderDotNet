
namespace StateForge.Examples.CallManager
{
    public interface ICallEvent
    {
        void AlertingConnection(Connection connection);
        void IncomingConnection(Connection connection);
        void AnsweredConnection(Connection connection);
        void ConnectedConnection(Connection connection);
        void EstablishedCall(Call call);
        void ReleasingConnection(Connection connection);
        void ReleasedConnection(Connection connection);
        void ClearedCall(Call call);
    }
}
