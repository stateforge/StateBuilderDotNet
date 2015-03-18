#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateType.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.Collections.Generic;

    public partial class StateType
    {
        [Flags]
        public enum TypeFlags
        {
            UNDEFINED = 0,
            ROOT = 1,
            TOP = 2,
            COMPOSITE = 4,
            LEAF = 8,
            FINAL = 16,
            //TODO remove error state
            ERROR = 32,
            HISTORY = 64,
            HAS_HISTORY = 128,
            PARALLEL = 256
        }

        public TypeFlags Type { get; set; }

        public StateType Parent { get; set; }

        public StateType Top { get; set; }

        public StateType StateParallel { get; set; }

        private List<StateType> childList; /// only for root top state

        public List<StateType> ChildList
        {
            get
            {
                if (childList == null) {
                    childList = new List<StateType>();
                }
                return this.childList;
            }
        }

        public void ChildAdd(StateType stateChild){
            childList.Add(stateChild);
        }

        private List<StateType> parallelList;
        public List<StateType> ParallelList
        {
            get
            {
                if (this.parallelList == null)
                {
                    this.parallelList = new List<StateType>();

                }
                return this.parallelList;
            }
            set
            {
                this.parallelList = value;
            }
        }

        public bool HasParentStateHistory()
        {
            return HasParentStateHistory(this);
        }

        private bool HasParentStateHistory(StateType state)
        {
            if (state == null)
            {
                return false;
            }
            else
            {
                if (state.Type.HasFlag(StateType.TypeFlags.HAS_HISTORY))
                {
                    return true;
                }
                else
                {
                    return HasParentStateHistory(state.Parent);
                }
            }
        }

        public bool HasChildHistory()
        {
            return HasChildHistory(this);
        }

        private bool HasChildHistory(StateType state)
        {
            if (state.state == null)
            {
                return false;
            }
            else
            {
                foreach (StateType stateChild in state.state)
                {
                    if (stateChild.kind == StateKindType.history)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }


}
