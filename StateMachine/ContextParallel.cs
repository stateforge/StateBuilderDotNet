#region Copyright
//------------------------------------------------------------------------------
// <copyright file="ContextParallel.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
    /// <summary>
    /// ContextParallel is an abstract class for parallel region.
    /// </summary>
    public abstract class ContextParallel
    {
        /// <summary>
        /// Gets or sets the number of active contexts
        /// </summary>
        public int ActiveState { get; set; }

        /// <summary>
        /// Go the next state.
        /// </summary>
        public abstract void TransitionToNextState();
    }
}
