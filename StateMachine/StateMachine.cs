#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateMachine.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    using System;
    
    /// <summary>
    /// StateMachineHelper is a helper class needed by the StateBuilderDotNet generated code.
    /// </summary>
    public sealed class StateMachineHelper
    {
        /// <summary>
        /// Prevents a default instance of the StateMachineHelper class from being created.
        /// </summary>
        private StateMachineHelper()
        {
        }
        
        /// <summary>
        /// Perform the operations required at the beginning of a transition.
        /// Set the previous state to the current state.
        /// The current state is set to null.
        /// The next state is set.
        /// The OnTransitionBegin() observer method is invoked.
        /// The chain on OnExit() is called.
        /// </summary>
        /// <typeparam name="TContext">A Context class.</typeparam>
        /// <typeparam name="TContextParent">A parent Context class.</typeparam>
        /// <typeparam name="TState">A State class.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="stateNext">The next state, can be null for internal transition where entry and exit functions are not executed.</param>
        public static void ProcessTransitionBegin<TContext, TContextParent, TState>(
            TContext context,
            TState stateNext)
            where TContextParent : ContextBase
            where TContext : Context<TState, TContextParent>
            where TState : State<TContext, TState>
        {
            context.StatePrevious = context.StateCurrent;
            context.StateCurrent = default(TState);
            TState statePrevious = context.StatePrevious;
            context.StateNext = stateNext;

            context.Observer.OnTransitionBegin(
                context.Name,
                statePrevious.Name,
                stateNext != null ? stateNext.Name : statePrevious.Name,
                context.TransitionName);

            if (stateNext != null)
            {
                WalkChainExit<TContext, TState>(context, statePrevious, stateNext);
            }
        }

        /// <summary>
        /// Perform the operations required at the end of a transition.
        /// The chain on OnEntry() is called.
        /// The OnTransitionEnd() observer method is invoked.
        /// The previous state is set to null.
        /// The current state is set to the next state.
        /// The next state is set to null.
        /// </summary>
        /// <typeparam name="TContext">A Context class.</typeparam>
        /// <typeparam name="TContextParent">A parent Context class.</typeparam>
        /// <typeparam name="TState">A State class.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="stateNext">The next state.</param>
        public static void ProcessTransitionEnd<TContext, TContextParent, TState>(
            TContext context,
            TState stateNext)
            where TContextParent : ContextBase
            where TContext : Context<TState, TContextParent>
            where TState : State<TContext, TState>
        {
            TState statePrevious = context.StatePrevious;

            WalkChainEntry<TContext, TState>(context, statePrevious, stateNext);

            context.Observer.OnTransitionEnd(
                context.Name,
                statePrevious.Name,
                stateNext != null ? stateNext.Name : statePrevious.Name,
                context.TransitionName);

            context.StatePrevious = null;
            context.StateCurrent = stateNext != null ? stateNext : statePrevious;
            context.StateNext = null;
        }

        /// <summary>
        /// Walk the OnEntry chain. This is a recursive function.
        /// </summary>
        /// <typeparam name="TContext">A Context class.</typeparam>
        /// <typeparam name="TState">A State class.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="statePrevious">The previous state.</param>
        /// <param name="stateNext">The next state.</param>
        public static void WalkChainEntry<TContext, TState>(TContext context, TState statePrevious, TState stateNext)
             where TState : State<TContext, TState>
        {
            if (stateNext == null) 
            {
                return;
            }

            if (statePrevious == stateNext)
            {
                stateNext.OnEntry(context);
                return;
            } 

            TState stateParent = stateNext.StateParent;
            if (stateParent == null) 
            {
                return;
            }

            if (IsChild<TContext, TState>(statePrevious, stateNext) == false)
            {
                WalkChainEntry<TContext, TState>(context, statePrevious, stateParent);
                stateNext.OnEntry(context);
            }
        }

        /// <summary>
        /// Walk the OnExit chain. This is a recursive function.
        /// </summary>
        /// <typeparam name="TContext">A context class.</typeparam>
        /// <typeparam name="TState">A state class.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="statePrevious">The previous state.</param>
        /// <param name="stateNext">The next state.</param>
        public static void WalkChainExit<TContext, TState>(TContext context, TState statePrevious, TState stateNext)
             where TState : State<TContext, TState>
        {
            if (statePrevious == stateNext)
            {
                stateNext.OnExit(context);
                return;
            } 

            TState stateParent = statePrevious.StateParent;
            if ((stateParent == null) || (stateNext == null))
            {
                return;
            }

            if ((statePrevious == stateNext) || (IsChild<TContext, TState>(stateNext, statePrevious) == false))
            {
                statePrevious.OnExit(context);
                WalkChainExit<TContext, TState>(context, stateParent, stateNext);
            }
        }

        /// <summary>
        /// Is the state a child of the current state ?.
        /// </summary>
        /// <typeparam name="TContext">A context class.</typeparam>
        /// <typeparam name="TState">A state class.</typeparam>
        /// <param name="stateCurrent">The current state.</param>
        /// <param name="state">The eventual child state.</param>
        /// <returns>Returns true if the state is a child of the current state.</returns>
        private static bool IsChild<TContext, TState>(TState stateCurrent, TState state)
             where TState : State<TContext, TState>
        {
            TState stateParent = stateCurrent.StateParent;
            return (stateCurrent == state) || ((stateParent != null) && IsChild<TContext, TState>(stateParent, state));
        }
    }
}
