#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ObserverNull.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    
    /// <summary>
    /// A singleton observer that does nothing. 
    /// </summary>
    public class ObserverNull : IObserver
    {
        /// <summary>
        /// The ObserverNull instance.
        /// </summary>
        private static ObserverNull instance = new ObserverNull();

        /// <summary>
        /// Prevents a default instance of the ObserverNull class from being created.
        /// </summary>
        private ObserverNull()
        { 
        }

        /// <summary>
        /// Gets the ObserverNull instance.
        /// </summary>
        public static ObserverNull Instance
        {
            get
            {
                return instance;
            }
        }
        
        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is entered.</param>
        public override void OnEntry(
            string context,
            string state)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is left.</param>
        public override void OnExit(
            string context,
            string state)
        {
        }

        /// <summary>
        /// Do nothing.
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
        }

        /// <summary>
        /// Do nothing.
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
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">the timer name.</param>
        /// <param name="duration">The timer duration.</param>
        public override void OnTimerStart(
            string context,
            string timerName,
            double duration)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">The timer name.</param>
        public override void OnTimerStop(
            string context,
            string timerName)
        {
        }
    }
}
