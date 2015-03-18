#region Copyright
//------------------------------------------------------------------------------
// <copyright file="Event.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    
    /// <summary>
    /// Delegate with no argeter.
    /// </summary>
    public delegate void Handler0();

    /// <summary>
    /// Delegate with one argeter.
    /// </summary>

    public delegate void Handler1<TArg1>(TArg1 arg1);

    /// <summary>
    /// Delegate with two arguments.
    /// </summary>
    public delegate void Handler2<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);

    /// <summary>
    /// Delegate with three arguments.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public delegate void Handler3<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);

    /// <summary>
    /// Delegate with four arguments.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public delegate void Handler4<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    ///// <summary>
    ///// The event interface. 
    ///// </summary>
    //public interface IEvent
    //{
    //    /// <summary>
    //    /// Invoke the delegate.
    //    /// </summary>
    //    void Dispatch();
    //}

    /// <summary>
    /// Event Base
    /// </summary>
    public abstract class EventBase
    {
        /// <summary>
        /// Default EventBase constructor.
        /// </summary>
        public EventBase(int eventId)
        {
            EventId = eventId; 
        }

        /// <summary>
        /// The event Id.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Invoke the delegate.
        /// </summary>
        abstract public void Dispatch();
    }

    /// <summary>
    /// Event without parameter.
    /// </summary>
    internal class Event0 : EventBase
    {
        private Handler0 handler;

        /// <summary>
        /// 
        /// </summary>
        /// <arg name="handler"></arg>
        public Event0(int evenId, Handler0 handler) : base(evenId)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Invoke the delegate without parameter.
        /// </summary>
        public override void Dispatch()
        {
            this.handler();
        }
    }

    /// <summary>
    /// Event with one parameter
    /// </summary>
    /// <typearg name="TArg1"></typearg>
    internal class Event1<TArg1> : EventBase
    {
        private Handler1<TArg1> handler;
        private TArg1 arg1;

        /// <summary>
        /// Initializes a new instance of the Event1 class.
        /// </summary>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        public Event1(int evenId, Handler1<TArg1> handler, TArg1 arg1)
            : base(evenId)
        {
            this.handler = handler;
            this.arg1 = arg1;
        }

        /// <summary>
        /// Invoke the delegate with one parameter.
        /// </summary>
        public override void Dispatch()
        {
            this.handler(this.arg1);
        }
    }

    /// <summary>
    /// Event with 2 arguments
    /// </summary>
    /// <typearg name="TArg1"></typearg>
    /// <typearg name="TArg2"></typearg>
    internal class Event2<TArg1, TArg2> : EventBase
    {
        private Handler2<TArg1, TArg2> handler;
        private TArg1 arg1;
        private TArg2 arg2;
        /// <summary>
        /// Initializes a new instance of the Event2 class.
        /// </summary>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        public Event2(int evenId, Handler2<TArg1, TArg2> handler, TArg1 arg1, TArg2 arg2)
            : base(evenId)
        {
            this.handler = handler;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        /// <summary>
        /// Invoke the delegate with 2 arguments.
        /// </summary>
        public override void Dispatch()
        {
            this.handler(this.arg1, this.arg2);
        }
    }

    /// <summary>
    /// Event with 3 arguments
    /// </summary>
    /// <typearg name="TArg1"></typearg>
    /// <typearg name="TArg2"></typearg>
    /// <typearg name="TArg3"></typearg>
    internal class Event3<TArg1, TArg2, TArg3> : EventBase
    {
        private Handler3<TArg1, TArg2, TArg3> handler;
        private TArg1 arg1;
        private TArg2 arg2;
        private TArg3 arg3;

        /// <summary>
        /// Initializes a new instance of the Event3 class.
        /// </summary>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        /// <arg name="arg3"></arg>
        public Event3(int evenId, Handler3<TArg1, TArg2, TArg3> handler, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            : base(evenId)
        {
            this.handler = handler;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
        }

        /// <summary>
        /// Invoke the delegate with 3 arguments.
        /// </summary>
        public override void Dispatch()
        {
            this.handler(this.arg1, this.arg2, this.arg3);
        }
    }

    /// <summary>
    /// Event with 4 arguments
    /// </summary>
    /// <typearg name="TArg1"></typearg>
    /// <typearg name="TArg2"></typearg>
    /// <typearg name="TArg3"></typearg>
    /// <typearg name="TArg4"></typearg>
    internal class Event4<TArg1, TArg2, TArg3, TArg4> : EventBase
    {
        private Handler4<TArg1, TArg2, TArg3, TArg4> handler;
        private TArg1 arg1;
        private TArg2 arg2;
        private TArg3 arg3;
        private TArg4 arg4;

        /// <summary>
        /// Initializes a new instance of the Event3 class.
        /// </summary>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        /// <arg name="arg3"></arg>
        /// <arg name="arg4"></arg>
        public Event4(int evenId, Handler4<TArg1, TArg2, TArg3, TArg4> handler, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
            : base(evenId)
        {
            this.handler = handler;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
        }

        /// <summary>
        /// Invoke the delegate with 4 arguments.
        /// </summary>
        public override void Dispatch()
        {
            this.handler(this.arg1, this.arg2, this.arg3, this.arg4);
        }
    }

    /// <summary>
    /// EventFactory creates events with various number arguments.
    /// </summary>
    public sealed class EventFactory
    {
        
        /// <summary>
        /// 
        /// </summary>
        EventFactory()
        {
        }
        
        /// <summary>
        /// Create an event without argeter.
        /// </summary>
        /// <arg name="handler"></arg>
        /// <returns></returns>
        public static EventBase CreateEvent(int eventId, Handler0 handler)
        {
            return new Event0(eventId, handler);
        }

        /// <summary>
        /// Create an event with one argeter.
        /// </summary>
        /// <typearg name="TArg1"></typearg>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <returns></returns>
        public static EventBase CreateEvent<TArg1>(int eventId, Handler1<TArg1> handler, TArg1 arg1)
        {
            return new Event1<TArg1>(eventId, handler, arg1);
        }

        /// <summary>
        /// Create an event with 2 arguments.
        /// </summary>
        /// <typearg name="TArg1"></typearg>
        /// <typearg name="TArg2"></typearg>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        /// <returns></returns>
        public static EventBase CreateEvent<TArg1, TArg2>(int eventId, Handler2<TArg1, TArg2> handler, TArg1 arg1, TArg2 arg2)
        {
            return new Event2<TArg1, TArg2>(eventId, handler, arg1, arg2);
        }

        /// <summary>
        /// Create an event with 3 arguments.
        /// </summary>
        /// <typearg name="TArg1"></typearg>
        /// <typearg name="TArg2"></typearg>
        /// <typearg name="TArg3"></typearg>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        /// <arg name="arg3"></arg>
        /// <returns></returns>
        public static EventBase CreateEvent<TArg1, TArg2, TArg3>(int eventId, Handler3<TArg1, TArg2, TArg3> handler, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return new Event3<TArg1, TArg2, TArg3>(eventId, handler, arg1, arg2, arg3);
        }

        /// <summary>
        /// Create an event with 4 arguments.
        /// </summary>
        /// <typearg name="TArg1"></typearg>
        /// <typearg name="TArg2"></typearg>
        /// <typearg name="TArg3"></typearg>
        /// <typearg name="TArg4"></typearg>
        /// <arg name="handler"></arg>
        /// <arg name="arg1"></arg>
        /// <arg name="arg2"></arg>
        /// <arg name="arg3"></arg>
        /// <arg name="arg4"></arg>
        /// <returns></returns>
        public static EventBase CreateEvent<TArg1, TArg2, TArg3, TArg4>(int eventId, Handler4<TArg1, TArg2, TArg3, TArg4> handler, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return new Event4<TArg1, TArg2, TArg3, TArg4>(eventId, handler, arg1, arg2, arg3, arg4);
        }
    }
}
