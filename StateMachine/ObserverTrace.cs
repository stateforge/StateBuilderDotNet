#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ObserverTrace.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// ObserverTrace logs all messages through the System.Diagnostics.Trace system
    /// </summary>
    public class ObserverTrace : IObserver
    {
        /// <summary>
        /// The trace source.
        /// </summary>
        private TraceSource traceSource;

        /// <summary>
        ///  Initializes a new instance of the ObserverTrace class.
        /// </summary>
        /// <param name="name">The trace name.</param>
        protected ObserverTrace(string name)
        {
            this.traceSource = new TraceSource(name);
        }

        /// <summary>
        /// Prevents a default instance of the ObserverTrace class from being created.
        /// </summary>
        private ObserverTrace()
        {
        }
        
        /// <summary>
        /// Gets the ObserverTrace instance.
        /// </summary>
        public static ObserverTrace Instance(string name)
        {
            ObserverTrace trace = new ObserverTrace(name);
            return trace;
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: entering in state {1}", context, state);
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: leaving from state {1}", context, state);
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: transion begins from state {1} to {2} by transition {3}", context, statePrevious, stateNext, transition);
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: transion has ended from state {1} to {2} by transition {3}", context, statePrevious, stateNext, transition);
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: start timer {1} for {2} msec", context, timerName, duration);
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
            this.traceSource.TraceEvent(TraceEventType.Verbose, 1, "{0}: stop timer {1}", context, timerName);
        }
    }
}

