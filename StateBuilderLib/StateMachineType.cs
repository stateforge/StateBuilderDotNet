#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateMachineType.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public partial class StateMachineType
    {
        String Name { get; set; }
        Dictionary<string, StateType> stateMap = new Dictionary<string, StateType>();
        Dictionary<string, EventType> eventMap = new Dictionary<string, EventType>();
        Dictionary<string, TimerType> timerMap = new Dictionary<string, TimerType>();

        List<EventType> eventList = new List<EventType>();
        List<TimerType> timers = new List<TimerType>();

        Dictionary<string, List<EventType>> feedersMap = new Dictionary<string, List<EventType>>();

        Dictionary<string, string> eventInterfaceMap = new Dictionary<string, string>();

        private List<EventType> GetFeederEvents(string feeder)
        {
            if ((string.IsNullOrEmpty(feeder) == true))
            {
                return null;
            }

            if (feedersMap.ContainsKey(feeder) == false)
            {
                feedersMap[feeder] = new List<EventType>();
            }
            return feedersMap[feeder];
        }

        public void AddEventToFeeder(string feeder, string eventId)
        {
            if ((string.IsNullOrEmpty(feeder) == false) && (string.IsNullOrEmpty(feeder) == false))
            {
                GetFeederEvents(feeder).Add(GetEventFromId(eventId));
            }
        }

        /// <summary>
        /// The list of object that feeds events to the context.
        /// </summary>
        internal Dictionary<string, List<EventType>> FeedersMap
        {
            get
            {
                return this.feedersMap;
            }
            set
            {
                this.feedersMap = value;
            }
        }

        /// <summary>
        /// The map eventInterface/file
        /// </summary>
        internal Dictionary<string, string> EventInterfaceMap
        {
            get
            {
                return this.eventInterfaceMap;
            }
            set
            {
                this.eventInterfaceMap = value;
            }
        }

        static TraceSource ts = new TraceSource("StateBuilder");
        /// <summary>
        /// Get the state from its name.
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public StateType GetStateType(string stateName)
        {
            StateType state = null;

            if (stateName == null)
            {
                return null;
            }

            if (stateMap.ContainsKey(stateName) == true)
            {
                state = stateMap[stateName];
            }
            return state;
        }

        /// <summary>
        /// Add a state to the state map.
        /// </summary>
        /// <param name="state"></param>
        public void AddState(StateType state)
        {
            stateMap[state.name] = state;
        }

        /// <summary>
        /// Get the leaf state. This is a recursive function.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateType GetStateLeaf(StateType state)
        {
            if (state == null)
            {
                return null;
            }

            if (state.Type.HasFlag(StateType.TypeFlags.LEAF))
            {
                return state;
            }

            return GetStateLeaf(state.state[0]);
        }

        /// <summary>
        /// Returns the context depth between the state and the next state.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="stateNext">The next state.</param>
        /// <returns></returns>
        public int ContextDepth(StateType state, StateType stateNext)
        {
            int depth = 0;
            return ContextDepth(state, stateNext, depth);
        }

        /// <summary>
        /// Returns the context depth between the state and the next state.
        /// The next state must in the current context or in an higher context.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="stateNext">The next state.</param>
        /// <param name="depth">The context depth between the state and the next state.</param>
        /// <returns></returns>
        private int ContextDepth(StateType state, StateType stateNext, int depth)
        {
            //TODO what if stateNext is below state ?
            if ((state == null) || (stateNext == null))
            {
                return 0;
            }

            if (state.StateParallel == stateNext.StateParallel)
            {
                return depth;
            }
            else
            {
                depth++;
                return ContextDepth(state.StateParallel, stateNext, depth);
            }
        }

        /// <summary>
        /// Get the top state of the given state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateType GetStateTop(StateType state)
        {
            if (state.Parent == null)
            {
                return state;
            }
            else
            {
                return GetStateTop(state.Parent);
            }
        }

        /// <summary>
        /// Is this state the root state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsRoot(StateType state)
        {
            return state.Type.HasFlag(StateType.TypeFlags.ROOT);
        }

        /// <summary>
        /// Get the event from its id.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public EventType GetEventFromId(string eventId)
        {
            EventType evt = null;
            if (this.eventMap.ContainsKey(eventId) == true)
            {
                evt = this.eventMap[eventId];
            }
            return evt;
        }

        /// <summary>
        /// Get the timer from its name
        /// </summary>
        /// <param name="timerName"></param>
        /// <returns></returns>
        public TimerType GetTimerFromName(string timerName)
        {
            TimerType timer = null;
            if (this.timerMap.ContainsKey(timerName) == true)
            {
                timer = this.timerMap[timerName];
            }
            return timer;
        }

        /// <summary>
        /// Get events for the given state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<EventType> GetEventsForState(StateType state)
        {
            if ((state.Type.HasFlag(StateType.TypeFlags.TOP)) ||
                (state.Type.HasFlag(StateType.TypeFlags.FINAL)) ||
                (state.Type.HasFlag(StateType.TypeFlags.PARALLEL)))
            {
                return GetEventsAll();
            }

            List<EventType> eventList = new List<EventType>();

            if (state.transition != null)
            {
                foreach (TransitionType transition in state.transition)
                {
                    EventType evt = GetEventFromId(transition.@event);
                    if (eventList.IndexOf(evt, 0) == -1)
                    {
                        eventList.Add(evt);
                    }
                }
            }

            return eventList;
        }

        /// <summary>
        /// Get all events.
        /// </summary>
        /// <returns></returns>
        public List<EventType> GetEventsAll()
        {
            return this.eventList;
        }

        /// <summary>
        /// Get all timers
        /// </summary>
        /// <returns></returns>
        public List<TimerType> GetTimersAll()
        {
            return this.timers;
        }

        /// <summary>
        /// Get the list of transition given an event. Transitions with the same event must have a different condition attribute or element.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public List<TransitionType> GetTransitionList(StateType state, string eventId)
        {
            List<TransitionType> transitions = new List<TransitionType>();
            if (state.transition != null)
            {
                foreach (TransitionType transition in state.transition)
                {
                    if (transition.@event == eventId)
                    {
                        transitions.Add(transition);
                    }
                }
            }

            return transitions;
        }

        /// <summary>
        /// Get the transition given the event id and the condition.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="eventId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TransitionType GetTransition(StateType state, string eventId, string condition)
        {
            if (state.transition != null)
            {
                foreach (TransitionType transition in state.transition)
                {
                    if (transition.@event == eventId)
                    {
                        if ((condition == null) && (GetCondition(transition) == null))
                        {
                            return transition;
                        }

                        if ((condition != null) && (condition == GetCondition(transition)))
                        {
                            return transition;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the transition name with is made up the event and the condition: eventName[condition] or eventName for condition less transition.
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public string GetTransitionName(TransitionType transition)
        {
            string condition = GetCondition(transition);
            if (condition != null)
            {
                return transition.@event + "[" + condition + "]";
            }
            else
            {
                return transition.@event;
            }
        }
        /// <summary>
        /// Gets the condition from the transition. the condition can be either set as an attribute or an element .
        /// </summary>
        /// <param name="transition">The transition</param>
        /// <returns>The eventual condition associated with this transition.</returns>
        public string GetCondition(TransitionType transition)
        {
            if (string.IsNullOrEmpty(transition.condition) == false)
            {
                return transition.condition;
            }
            else if (string.IsNullOrEmpty(transition.condition1) == false)
            {
                return transition.condition1;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Fill the model.
        /// </summary>
        public void FillModelPost()
        {
            FillEventContainer();
            FillState(this.state, null, null);
            this.state.Type = this.state.Type | StateType.TypeFlags.ROOT;
        }

        private void FillEventContainer()
        {
            EventSourceType[] eventSources = this.events;
            foreach (EventSourceType eventSource in eventSources)
            {
                if (string.IsNullOrEmpty(eventSource.file) == false)
                {
                    this.eventInterfaceMap[eventSource.name] = eventSource.file;
                }

                if (eventSource.@event != null)
                {
                    foreach (EventType evt in eventSource.@event)
                    {
                        this.eventMap[evt.id] = evt;
                        this.eventList.Add(evt);
                    }
                }
                if (eventSource.timer != null)
                {
                    foreach (TimerType timer in eventSource.timer)
                    {

                        // Add parameter (object source, ElapsedEventArgs e)
                        ParameterType source = new ParameterType() { name = "source", type = "Object" };
                        //ParameterType elapsedEventArgs = new ParameterType() { name = "elapsedEventArgs", type = "ElapsedEventArgs" };
                        //timer.parameter = new ParameterType[] { source, elapsedEventArgs };
                        timer.parameter = new ParameterType[] { source };
                        this.eventMap[timer.id] = timer;
                        this.eventList.Add(timer);
                        this.timerMap[timer.name] = timer;
                        this.timers.Add(timer);
                    }
                }
            }
        }




        /// <summary>
        /// Fill the state parent and Type, add the state to the map state.
        /// This is a recursive function.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="stateParent"></param>
        private void FillState(StateType state, StateType stateParent, StateType stateParallel)
        {
            if (stateMap.ContainsKey(state.name) == true)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "FillState:  {0} already exist", state.name);
            }
            else
            {
                stateMap.Add(state.name, state);
            }

            state.Parent = stateParent;
            state.StateParallel = stateParallel;


            

            ts.TraceEvent(TraceEventType.Verbose, 1, "FillState:  {0}", state.name);

            if (state.kindSpecified == true)
            {
                switch (state.kind)
                {
                    case StateKindType.final:
                        state.Type = state.Type | StateType.TypeFlags.FINAL;
                        break;
                    case StateKindType.history:
                        state.Type = state.Type | StateType.TypeFlags.HISTORY;
                        break;
                    default:
                        //Should not happen
                        break;
                }
            }

            StateType stateTop = GetStateTop(state);
            if ((stateParent != null) && (state.Type.HasFlag(StateType.TypeFlags.HISTORY) == false))
            {
                stateTop.ChildAdd(state);
            }

            if (state.parallel != null)
            {
                // Parallel region
                state.Type = state.Type | StateType.TypeFlags.PARALLEL | StateType.TypeFlags.LEAF;
               
                stateTop.ParallelList.Add(state);
                
                if (state.parallel.state != null)
                {
                    foreach (StateType stateOrthogonal in state.parallel.state)
                    {
                        FillState(stateOrthogonal, null, state);
                    }
                }
            }
            else if (state.state == null)
            {
                // No child
                state.Type = state.Type | StateType.TypeFlags.LEAF;
            }
            else
            {
                // Childs
                state.Type = state.Type | StateType.TypeFlags.COMPOSITE;

                if (state.HasChildHistory())
                {
                    state.Type = state.Type | StateType.TypeFlags.HAS_HISTORY;
                }

                if (stateParent == null)
                {
                    state.Type = state.Type | StateType.TypeFlags.TOP;
                    //For top states in a parallel state machines, set the flag to HAS_HISTORY, it saves the current state upon OnExit
                    if ((state.StateParallel != null) && state.StateParallel.HasParentStateHistory())
                    {
                        state.Type = state.Type | StateType.TypeFlags.HAS_HISTORY;
                    }
                }

                if ((state.kindSpecified == true) && (state.kind == StateKindType.final))
                {
                    // TODO  ERROR Composite state cannot have error or final atttribute
                }

                foreach (StateType stateChild in state.state)
                {
                    
                    FillState(stateChild, state, stateParallel);
                }
            }
        }

        internal void AddEvent(EventType evt)
        {
            if (this.eventMap.ContainsKey(evt.id) == false)
            {
                this.eventMap[evt.id] = evt;
                this.eventList.Add(evt);
            }
            else
            {
                //TODO
            }
        }
    }
}
