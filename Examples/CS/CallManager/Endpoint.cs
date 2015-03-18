
namespace StateForge.Examples.CallManager
{
    using System.Diagnostics;

    public abstract class Endpoint
    {
        private static readonly string traceName = "Endpoint";
        protected static TraceSource ts = new TraceSource(traceName);

        //public CallControl CallControl { get; set; }
        public string Name { get; set; }
        public IEndpointEvent EndpointEvent { get; set; }

        public abstract void Open();
        public abstract void Close();

        public abstract void Setup(string to, string token);
    }
}
