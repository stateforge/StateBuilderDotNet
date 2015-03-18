namespace StateForge.Examples.Light
{
    using System;
    using StateForge.StateMachine;

    public interface ILightActuator
    {
        void DoOn();
        void DoOff();
    }

    public class LightActuator : ILightActuator
    {
        public void DoOn()
        {
            Console.WriteLine("DoOn");
        }

        public void DoOff()
        {
            Console.WriteLine("DoOff");
        }
    }

    public partial class Light {
        private LightActuator lightActuator;
        private LightContext context;

        public Light()
        {
            this.lightActuator = new LightActuator();
            this.context = new LightContext(lightActuator);
            this.context.Observer = ObserverConsole.Instance;
            this.context.EnterInitialState();
        }
        
        static void Main(string[] args)
        {
            Light light = new Light();
            light.EvOn();
            //light.On();
            //light.On();
            //light.Off();
            Environment.Exit(0);
        }
    }
}
