
namespace StateForge.Examples
{
    using System;
    using StateForge.StateMachine;

    public partial class HelloWorld
    {
        private HelloWorldContext context;

        public HelloWorld()
        {
            this.context = new HelloWorldContext(this);
            this.context.Observer = ObserverConsole.Instance;
        }

        protected internal void DoPrint()
        {
            Console.WriteLine("HelloWorld");
        }

        static void Main(string[] args)
        {
            var helloWorld = new HelloWorld();
            helloWorld.EvPrint();
            Environment.Exit(0);
        }
    }
}
