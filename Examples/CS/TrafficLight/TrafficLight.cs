
namespace StateForge.Examples.TrafficLight
{
    using System;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// The Light class knows how to switch on and off the green, yellow and red lights.
    /// </summary>
    public class Light
    {
        /// <summary>
        /// Red timer duration in milliseconds
        /// </summary>
        public long TimerRedDuration { get; set; }

        /// <summary>
        /// Yellow timer duration in milliseconds
        /// </summary>
        public long TimerYellowDuration { get; set; }

        /// <summary>
        /// Green timer duration in milliseconds
        /// </summary>
        public long TimerGreenDuration { get; set; }

        /// <summary>
        /// Maximum operating duration in milliseconds
        /// </summary>
        public long TimerMaxDuration { get; set; }

        public Light()
        {
            TimerRedDuration = 500; /* msec */
            TimerYellowDuration = 500;/* msec */
            TimerGreenDuration = 500;/* msec */

            TimerMaxDuration = 1900; /* msec */
        }

        public void TurnOnGreen()
        {
            Console.WriteLine("TurnOnGreen");
        }

        public void TurnOffGreen()
        {
            Console.WriteLine("TurnOffGreen");
        }

        public void TurnOnYellow()
        {
            Console.WriteLine("TurnOnYellow");
        }

        public void TurnOffYellow()
        {
            Console.WriteLine("TurnOffYellow");
        }
        public void TurnOnRed()
        {
            Console.WriteLine("TurnOnRed");
        }

        public void TurnOffRed()
        {
            Console.WriteLine("TurnOffRed");
        }
    }

    /// <summary>
    /// TrafficLight is a container class which hold instances of a TrafficLightActuator and a generated TrafficLightContext class.
    /// </summary>
    public partial class TrafficLight
    {
        private Light light;
        private TrafficLightContext context;
        private static readonly string traceName = "TrafficLight";
        private static TraceSource ts = new TraceSource(traceName);
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public TrafficLight()
        {
            this.light = new Light();
            this.context = new TrafficLightContext(light);
            this.context.Observer = ObserverTrace.Instance(traceName);
            this.context.EndHandler += new EventHandler<EventArgs>(StateMachineEnd);
            this.context.EnterInitialState();
        }

        private void StateMachineEnd(object sender, EventArgs e)
        {
            ts.TraceInformation("state machine has ended");
            autoResetEvent.Set();
        }

        static void Main(string[] args)
        {
            TrafficLight myTrafficLight = new TrafficLight();
            myTrafficLight.Start("Ciao");
            autoResetEvent.WaitOne();
            ts.TraceInformation("TrafficLight has ended");
            Environment.Exit(0);
        }
    }
}
