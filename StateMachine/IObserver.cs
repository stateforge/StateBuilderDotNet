#region Copyright
//------------------------------------------------------------------------------
// <copyright file="IObserver.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    
    /// <summary>
    /// The IObserver interface, allow to find out what's going on inside the state machine.
    /// </summary>
    public abstract class IObserver
    {
        /// <summary>
        /// Inform the outside world when entering a state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is entered.</param>
        public abstract void OnEntry(
            string context,
            string state);

        /// <summary>
        /// Inform the outside world when leaving a state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state which is left.</param>
        public abstract void OnExit(
            string context,
            string state);

        /// <summary>
        /// Inform the outside world when a transition begins.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <param name="statePrevious">The previous state name.</param>
        /// <param name="stateNext">The next state name.</param>
        /// <param name="transition">The transition name.</param>
        public abstract void OnTransitionBegin(
            string context,
            string statePrevious,
            string stateNext,
            string transition);

        /// <summary>
        /// Inform the outside world when a transition ends.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <param name="statePrevious">The previous state name.</param>
        /// <param name="stateNext">The next state name.</param>
        /// <param name="transition">The transition name.</param>
        public abstract void OnTransitionEnd(
            string context,
            string statePrevious,
            string stateNext,
            string transition);

        /// <summary>
        /// Inform the outside world a timer is started.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">the timer name.</param>
        /// <param name="duration">The timer duration.</param>
        public abstract void OnTimerStart(
            string context,
            string timerName,
            double duration);

        /// <summary>
        /// Inform the outside world a timer is stopped.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerName">The timer name.</param>
        public abstract void OnTimerStop(
            string context,
            string timerName);

    }
}

