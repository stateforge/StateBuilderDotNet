#region Copyright
//------------------------------------------------------------------------------
// <copyright file="Context.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The Context class, the class is used as the base class for the context generated code.
    /// </summary>
    /// <typeparam name="TState">State class.</typeparam>
    /// <typeparam name="TContextParent">Context parent class.</typeparam>
    public abstract class Context<TState, TContextParent> : ContextBase
        where TContextParent : ContextBase
        where TState : StateBase
    {
        /// <summary>
        /// Initializes a new instance of the Context class.
        /// </summary>
        protected Context()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Context class with the parent context.
        /// </summary>
        /// <param name="contextParent">The parent context.</param>
        protected Context(TContextParent contextParent)
        {
            this.Observer = ObserverNull.Instance;
            this.ContextParent = contextParent;
            if (contextParent != null)
            {
                contextParent.AddChild(this);
            }
        }

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public TState StateCurrent { get; protected internal set; }

        /// <summary>
        /// Gets or sets the previous state.
        /// </summary>
        public TState StatePrevious { get; protected internal set; }

        /// <summary>
        /// Gets or sets the next state.
        /// </summary>
        public TState StateNext { get; protected internal set; }

        /// <summary>
        /// Gets or sets the history state.
        /// </summary>
        public TState StateHistory { get; protected internal set; }

        /// <summary>
        /// Gets or sets the eventual parent context.
        /// </summary>
        public TContextParent ContextParent { get; protected internal set; }
        
        /// <summary>
        /// Save the previous state to the history state.
        /// </summary>
        public void SaveState()
        {
            if (this.StatePrevious != null)
            {
                this.StateHistory = this.StatePrevious;
            }
            else
            {
                this.StateHistory = this.StateCurrent;
            }
        }
        
        /// <summary>
        /// Set the current and previous state.
        /// </summary>
        /// <param name="stateInitial">The initial state.</param>
        public void SetInitialState(TState stateInitial)
        {
            this.StatePrevious = stateInitial;
            this.StateCurrent = stateInitial;
        }

        /// <summary>
        /// Invoke the EndHandler if any.
        /// </summary>
        public override void OnEnd()
        {
            if (this.ContextParent != null)
            {
                ContextParallel contextParallel = this.ContextParent.ContextParallel;
                //TODO check for ActiveState == 0
                contextParallel.ActiveState--;

                if (contextParallel.ActiveState == 0)
                {
                    this.ContextParent.TransitionName = "ContextParallelEnd";
                    contextParallel.TransitionToNextState();
                }
            }
            
            base.OnEnd();
        }

        /// <summary>
        /// Serialize the context.
        /// </summary>
        /// <param name="streamWriter"></param>
        public override void Serialize(StreamWriter streamWriter)
        {
            SerializeState(streamWriter);
            SerializeParallel(streamWriter);
        }

        /// <summary>
        /// Serialize the current state
        /// </summary>
        /// <param name="streamWriter"></param>
        protected override void SerializeState(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(StateCurrent.Name);
        }

        /// <summary>
        /// DeSerialize the context.
        /// </summary>
        /// <param name="streamReader"></param>
        public override void DeSerialize(StreamReader streamReader)
        {
            DeSerializeState(streamReader);
            DeSerializeParallel(streamReader);
        }
    }
}
