//
// * Change the namespace StateForge.Examples.FixMachine in MyObject.cs and in MyObject.fsmcs.
// * Rename MyObject.cs and MyObject.fsmcs by right clicking on the file in the solution explorer.
// 
//
using QuickFix;

namespace StateForge.Examples.FixMachine
{
    using System;

    /// <summary>
    /// IFixMachineActuator is an interface that is used by the FixMachine class to perform actions.
    /// This interface is set in the state machine description through the attribute //StateMachine/settings/object@class
    /// </summary>
    public interface IFixMachineActuator
    {
        void DoOn();
        void DoOff();
    }

    /// <summary>
    /// FixMachineActuator is a concrete implementation of the IFixMachineActuator interface.
    /// </summary>
    public class FixMachineActuator : IFixMachineActuator
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


    public class FixMachine
    {

        static void Main(string[] args)
        {
            String executorConfigFileName = "executor.cfg";
            String traderConfigFileName = "tradeclient.cfg";
            try
            {

                Executor executor = new Executor() { ConfigFileName = executorConfigFileName };
                executor.Start();

                Trader trader = new Trader() { ConfigFileName = traderConfigFileName };
                trader.Start();

                Console.WriteLine("FixMachine Press any key to quit");
                Console.ReadLine();

                trader.Stop();
                executor.Stop();
            }
            catch (ConfigError e)
            {
                Console.WriteLine(e);
            }

            Environment.Exit(0);
        }

        

    }
}
