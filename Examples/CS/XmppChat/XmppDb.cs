
namespace StateForge.Examples.XmppChat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Data.Odbc;

    public class XmppDb
    {
        public string ConnectionString { get; set; }
        OdbcConnection connection;
        private static readonly string traceName = "Db";
        private static TraceSource ts = new TraceSource(traceName);

        public XmppDb(string connectionString)
        {
            ts.TraceInformation("connection string {0}", connectionString);
            this.ConnectionString = connectionString;
        }

        public void Open()
        {
            this.connection = new OdbcConnection(this.ConnectionString);
            this.connection.Open();
        }

        public void Close()
        {
            this.connection.Close();
        }

        public void UserRemove(string user)
        {
            string queryString = "DELETE FROM users WHERE username='" + user + "';";
            ts.TraceInformation("UserRemove {0}", queryString);
            Query(queryString);
        }

        public void UserAdd(string user, string password)
        {
            string queryString = "insert into users (username,password) value ('" + user + "','" + password + "') ;";
            ts.TraceInformation("UserAdd {0} ", queryString);
            Query(queryString);
        }

        private void Query(string queryString)
        {
            if (this.connection == null)
            {
                Open();
            }

            try
            {
                var command = new OdbcCommand(queryString, connection);
                int row = command.ExecuteNonQuery();
                ts.TraceEvent(TraceEventType.Error, 1, "row: {0}", row);
            }
            catch (OdbcException exception)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "error: {0}", exception.Message);
            }
        }
    }
}
