#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ContextAsync.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.IO;

    /// <summary>
    /// A context used for asynchronous machine.
    /// </summary>
    /// <typeparam name="TState">The top state.</typeparam>
    /// <typeparam name="TContextParent">The parent context.</typeparam>
    public abstract class ContextAsync<TState, TContextParent> : Context<TState, TContextParent> where TContextParent : ContextBase where TState : StateBase
    {
        /// <summary>
        /// Indicate if ProcessEvent() is scheduled or not finished yet.
        /// </summary>
        private bool processing = false;
        
        /// <summary>
        /// Initializes a new instance of the ContextAsync class.
        /// </summary>
        protected ContextAsync()
            : this(null)
        {
            this.EventQueue = new Queue<EventBase>();
        }

        /// <summary>
        /// Initializes a new instance of the ContextAsync class.
        /// </summary>
        /// <param name="contextParent">The parent context.</param>
        protected ContextAsync(TContextParent contextParent)
            : base(contextParent)
        {
            this.MaxEventProcessed = 1024;
        }

        /// <summary>
        /// Gets or sets the maximum number of event processed in the method ProcessEvent()
        /// </summary>
        public int MaxEventProcessed { get; set; }

        /// <summary>
        /// Gets or set the queue where events are stored for being processed later.
        /// </summary>
        protected Queue<EventBase> EventQueue { get; private set; }
        
        /// <summary>
        /// Dequeue the event from the queue and execute its delegate.
        /// </summary>
        public void ProcessEvents(object val)
        {
            int eventsProcessed = 0;
            EventBase evt;

            while (true)
            {
                lock (this.EventQueue)
                {
                    if ((this.EventQueue.Count == 0) || (eventsProcessed >= this.MaxEventProcessed))
                    {
                        this.processing = false;
                        break;
                    }

                    evt = this.EventQueue.Dequeue();

                    eventsProcessed++;
                }
                
                // Execute the delegate associated with this event outside the lock.
                // Be careful here because events may be queued while executing.
                evt.Dispatch();
            }
        }

        /// <summary>
        /// Invoke ProcessEvents() asynchronously if necessary.
        /// </summary>
        public void ScheduleEvent()
        {
            if (this.processing == false)
            {
                this.processing = true;

                WaitCallback w = new WaitCallback(this.ProcessEvents);
                ThreadPool.QueueUserWorkItem(w);
            }
        }

        /// <summary>
        /// Serialize the context.
        /// </summary>
        /// <param name="streamWriter"></param>
        public override void Serialize(StreamWriter streamWriter)
        {
            SerializeState(streamWriter);
            SerializeEvents(streamWriter);
            SerializeParallel(streamWriter);
        }

        /// <summary>
        /// Serialize the events.
        /// </summary>
        /// <param name="streamWriter"></param>
        public void SerializeEvents(StreamWriter streamWriter)
        {
            int eventListSize = EventQueue.Count;
            streamWriter.WriteLine(eventListSize);
            foreach (EventBase eventBase in this.EventQueue)
            {
                int eventId = eventBase.EventId;
                if (eventId == 0)
                {
                    // TODO throw exception
                }

                streamWriter.WriteLine(eventId);
            }
        }

        /// <summary>
        /// DeSerialize the context.
        /// </summary>
        /// <param name="streamReader"></param>
        public override void DeSerialize(StreamReader streamReader)
        {
            DeSerializeState(streamReader);
            DeSerializeEvents(streamReader);
            DeSerializeParallel(streamReader);
        }

        /// <summary>
        /// DeSerialize the events.
        /// </summary>
        /// <param name="streamReader"></param>
        public void DeSerializeEvents(StreamReader streamReader)
        {
            string eventListSizeString = streamReader.ReadLine();
            int eventListSize = Convert.ToInt32(eventListSizeString);
            for(int i = 0; i < eventListSize; i++)
            {
                string eventIdString = streamReader.ReadLine();
                int eventId = Convert.ToInt32(eventIdString);
                if (eventId == 0) { 
                    // TODO throw exception
                }

                PushBackEvent(eventId);
            }
        }

        /// <summary>
        /// Push back events on the queue
        /// </summary>
        /// <param name="eventId"></param>
        protected virtual void PushBackEvent(int eventId)
        {
            //throw new NotImplementedException();
        }
    }
}
