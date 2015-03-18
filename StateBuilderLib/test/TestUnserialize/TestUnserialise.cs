#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using NUnit.Framework;
#endregion

namespace StateForge
{
    [TestFixture]
    public class TestUnserialize
    {
        XmlSerializer Serializer { get; set; }
        string FileName { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("FsmUnserialize hi 5");
            TestUnserialize testUnserialize = new TestUnserialize();
            testUnserialize.UnserializeFile("FsmSamples/HelloWorld.fsm");
            Console.WriteLine("FsmUnserialize arriverderci");
        }


        public TestUnserialize()
        {
            Serializer = new XmlSerializer(typeof(StateMachineType));
        }

        [Test]
        public void test1()
        {
            TestUnserialize testUnserialize = new TestUnserialize();
            testUnserialize.UnserializeFile("Samples/HelloWorld.fsmcs");
        }
        
        public void UnserializeFile(string fileName)
        {
            //try
            //{
                TextReader reader = new StreamReader(fileName);

                StateMachineType fsm = (StateMachineType)Serializer.Deserialize(reader);
                FsmPrint(fsm);
                reader.Close();
            //}
            //catch (IOException ioException)
            //{
            //    Console.WriteLine("Error: " + ioException.Message);
            //}
            //catch (System.InvalidOperationException invalidOperationException)
            //{
            //    Console.WriteLine("Error: " + invalidOperationException.Message);
            //    //Console.WriteLine();
            //}
        }

        public void FsmPrint(StateMachineType fsm)
        {
            FsmSettings(fsm.settings);
            FsmEventList(fsm);
            FsmStatePrint(fsm.state, null);
        }

        public void FsmSettings(SettingsType settings)
        {
            Console.WriteLine("Settings:");
            Console.WriteLine("description: " + settings.description);
            Console.WriteLine("asynchronous: " + settings.asynchronous);
            Console.WriteLine("namespace: " + settings.@namespace);
            Console.WriteLine("using: " + settings.@using);
        }

        public void FsmEventList(StateMachineType fsm)
        {
            foreach (EventSourceType eventSource in fsm.events){
                Console.WriteLine("EventProvider:");
                Console.WriteLine("name: " + eventSource.name);
                Console.WriteLine("description: " + eventSource.description);
                foreach (EventType smEvent in eventSource.@event){
                    Console.WriteLine("Event:");
                    Console.WriteLine("name: " + smEvent.name);
                    Console.WriteLine("id: " + smEvent.id);
                    Console.WriteLine("description: " + smEvent.description);
                }
            }
        }

        public void FsmStatePrint(StateType state, StateType stateParent)
        {
            Console.WriteLine("State name : " + state.name);
            Console.WriteLine("State kind : " + state.kind);

            state.Parent = stateParent;

            if (state.transition != null)
            {
                foreach (TransitionType transition in state.transition)
                {
                    FsmPrintTransition(transition);
                }
            }
            if (state.state != null)
            {
                foreach (StateType stateChild in state.state)
                {
                    FsmStatePrint(stateChild, state);
                }
            }
        }

        public void FsmPrintTransition(TransitionType transition)
        {
            Console.WriteLine("Transition: ");
            Console.WriteLine("description : " + transition.description);

            Console.WriteLine("Transition event : " + transition.@event);
            Console.WriteLine("Transition next state : " + transition.nextState);
            Console.WriteLine("Transition condition : " + transition.condition);
            
 
        }

        public void FsmPrintTimer(TimerType timer)
        {
            Console.WriteLine("Timer name : " + timer.name);
            Console.WriteLine("Timer id : " + timer.id);
            Console.WriteLine("Timer description : " + timer.description);   
        }
    }
}
