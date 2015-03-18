#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ObserverConsole.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;

    /// <summary>
    /// ObserverConsole logs all messages to the console.
    /// </summary>
    public class ObserverConsole : IObserver
    {
        /// <summary>
        /// The ObserverConsole instance.
        /// </summary>
        private static ObserverConsole instance = new ObserverConsole();

        /// <summary>
        /// Prevents a default instance of the ObserverConsole class from being created.
        /// </summary>
        private ObserverConsole()
        { 
        }

        /// <summary>
        /// Gets the ObserverConsole instance.
        /// </summary>
        public static ObserverConsole Instance
        {
            get
            {
                return instance;
            }
        }
        
        /// <summary>
        /// Inform the outside world when entering a state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is entered.</param>
        public override void OnEntry(
            string context,
            string state)
        {
            Console.WriteLine("{0}: entering in state {1}", context, state);
        }

        /// <summary>
        /// Inform the outside world when leaving a state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is left.</param>
        public override void OnExit(
            string context,
            string state)
        {
            Console.WriteLine("{0}: leaving from state {1}", context, state);
        }

        /// <summary>
        /// Inform the outside world when a transition begins.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <param name="statePrevious">The previous state name.</param>
        /// <param name="stateNext">The next state name.</param>
        /// <param name="transition">The transition name.</param>
        public override void OnTransitionBegin(
            string context,
            string statePrevious,
            string stateNext,
            string transition)
        {
            Console.WriteLine("{0}: transition begins from state {1} to {2}, event {3}", context, statePrevious, stateNext, transition);
        }

        /// <summary>
        /// Inform the outside world when a transition ends.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <param name="statePrevious">The previous state name.</param>
        /// <param name="stateNext">The next state name.</param>
        /// <param name="transition">The transition name.</param>
        public override void OnTransitionEnd(
            string context,
            string statePrevious,
            string stateNext,
            string transition)
        {
            Console.WriteLine("{0}: transition has ended from state {1} to {2}, event {3}", context, statePrevious, stateNext, transition);
        }

        /// <summary>
        /// Inform the outside world a timer is started.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">The timer name.</param>
        /// <param name="duration">The timer duration.</param>
        public override void OnTimerStart(
            string context,
            string timerName,
            double duration)
        {
            Console.WriteLine("{0}: start timer {1} for {2} msec", context, timerName, duration);
        }

        /// <summary>
        /// Inform the outside world a timer is stopped.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">The timer name.</param>
        public override void OnTimerStop(
            string context,
            string timerName)
        {
            Console.WriteLine("{0}: stop timer {1}", context, timerName);
        }
    }
}
