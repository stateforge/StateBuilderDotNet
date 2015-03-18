#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderBase.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    using System.Diagnostics;

    public abstract class CoderBase
    {
        protected static TraceSource ts = new TraceSource("StateBuilder");
        protected StateMachineType Model { get; private set; }
        public CodeNamespace CodeNamespace { get; private set; }
        public StateBuilderOptions Options { get; set; }

        protected CoderBase(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
        {
            Model = model;
            Options = options;
            CodeNamespace = codeNamespace;
        }

        public abstract void WriteCode();

        /// <summary>
        /// Get the state class name: DoorXxxState
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetStateClassName(StateType state)
        {
            return Model.settings.name + state.name + "State";
        }

        /// <summary>
        /// Get the base context class name, either Context or ContextAsync.
        /// </summary>
        /// <returns></returns>
        protected string GetContextBaseClassName(StateType state)
        {
            if ((Model.settings.asynchronous == true) && ((state.Type.HasFlag(StateType.TypeFlags.ROOT) == true)))
            {
                return "ContextAsync";
            }
            else
            {
                return "Context";
            }
        }

        /// <summary>
        /// Get the name of the synchromous method.
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        protected string GetMethodNameSync(EventType evt)
        {
            return evt.id + "Sync";
        }

        /// <summary>
        /// Gets the context class name: MicrowaveContext,  MicrowaveEngineContext, MicrowaveDoorContext ...
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetContextClassName(StateType state)
        {
            StateType stateTop = Model.GetStateTop(state);
            if (Model.IsRoot(stateTop) == true)
            {
                return Model.settings.context.@class;
            }
            else
            {
                return Model.settings.name + stateTop.name + "Context";
            }
        }

        /// <summary>
        /// Gets the parent context class name.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetContextParentClassName(StateType state)
        {
            if (state.StateParallel == null)
            {
                return GetContextClassName(state);
            }
            else 
            {
                return GetContextClassName(state.StateParallel);
            }
        }

        protected string GetParallelClassName(StateType state)
        {
            return Model.settings.name + state.name +  "Parallel";
        }

        protected string GetObjectFieldName(string obj)
        {
            return "_" + obj;
        }

        protected string GetParallelLocalVariableName(StateType state)
        {
            return "parallel" + state.name;
        }

        protected string GetParallelFieldName(StateType state)
        {
            return "_parallel" + state.name;
        }

        protected StateType GetStateInitial(StateType state) 
        {
            if ((state.state == null) || (state.state.Length == 0)) {
                return state;
            } else {
                return GetStateInitial(state.state[0]);
            }
        }

        protected string GetTimerFieldName(TimerType timer)
        {
            return "timer" + timer.name;
        }

        protected string GetTimerStartName(TimerType timer)
        {
            return "TimerStart" + timer.name;
        }

        protected string GetTimerStopName(TimerType timer)
        {
            return "TimerStop" + timer.name;
        }

        /// <summary>
        /// Get the property name: Client_bob
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string GetPropertyName(ObjectType obj)
        {
            return obj.@class + "_" + obj.instance;
        }

        //}
        // StateMachineHelper.ProcessTransitionBegin<DoorContext, DoorContext, StateTop>(context, StateNext.Instance);
        // or
        // StateMachineHelper.ProcessTransitionEnd<DoorContext, DoorContext, StateTop>(context, StateNext.Instance);
        protected void WriteProcessTransition(CodeStatementCollection statements, StateType state, string nextStateName, string suffix)
        {
            CodeExpression[] paramsRef;

            StateType stateNext = Model.GetStateType(nextStateName);
            
            string stateTopClassName;
            string contextClassName;
            string contextParentClassName;
            // context or context.ContextParent or context.ContextParent.ContextParent .... depending where is stateNext compared to state.
            var contextExpression = GetContextParentExpression(state, stateNext);

            if (stateNext == null)
            {
                //Internal transition
                paramsRef = new CodeExpression[] { 
                                      contextExpression, 
                                      new CodePrimitiveExpression(null)};

                stateTopClassName = GetStateClassName(Model.GetStateTop(state));
                contextClassName = GetContextClassName(state);
                contextParentClassName = GetContextParentClassName(state);
            }
            else
            {
                CodeExpression nextStateExpression;
                
                if (stateNext.Type.HasFlag(StateType.TypeFlags.HISTORY))
                {
                    // context.StateHistory
                    nextStateExpression = new CodeFieldReferenceExpression(
                                             new CodeVariableReferenceExpression(Model.settings.context.instance),
                                             "StateHistory");
                    
                }
                else 
                {
                    StateType stateLeaf = Model.GetStateLeaf(stateNext);
                    // NextState.Instance
                    nextStateExpression = new CodeFieldReferenceExpression(
                        new CodeVariableReferenceExpression(GetStateClassName(stateLeaf)),
                        "Instance");
                }

                stateTopClassName = GetStateClassName(Model.GetStateTop(stateNext));
                contextClassName = GetContextClassName(stateNext);
                contextParentClassName = GetContextParentClassName(stateNext);
                paramsRef = new CodeExpression[] { 
                                      contextExpression, 
                                      nextStateExpression};
            }

            // param StateRunning.Instance
            var onMethodInvoke = new CodeMethodInvokeExpression(
                      new CodeMethodReferenceExpression(
                         new CodeVariableReferenceExpression("StateMachineHelper"),
                         "ProcessTransition" + suffix,
                         new CodeTypeReference[] {
                                    new CodeTypeReference(contextClassName),
                                    new CodeTypeReference(contextParentClassName),
                                    new CodeTypeReference((stateTopClassName))}),
                         paramsRef);

            statements.Add(onMethodInvoke);
        }

        /// <summary>
        /// Depth = 0: context
        /// Depth = 1: context.ContextParent
        /// Depth = 2: context.ContextParent.ContextParent
        /// </summary>
        /// <param name="state"></param>
        /// <param name="stateNext"></param>
        /// <returns></returns>
        protected CodeExpression GetContextParentExpression(StateType state, StateType stateNext)
        {
            CodeExpression contextExpression = new CodeVariableReferenceExpression(Model.settings.context.instance);
            if (stateNext != null)
            {
                int contextDepth = Model.ContextDepth(state, stateNext);
                for (int i = 1; i <= contextDepth; i++)
                {
                    contextExpression = new CodeFieldReferenceExpression(contextExpression, "ContextParent");
                }
            }
            return contextExpression;
        }

        // If next state is final, invoke:
        // context.OnEnd();
        // or 
        // context.ContextParent.OnEnd();
        protected void WriteContextOnEnd(CodeStatementCollection statements, StateType state, StateType stateNext)
        {
            //Special case when the next state is a final state.
            if ((stateNext != null) && (stateNext.Type.HasFlag(StateType.TypeFlags.FINAL)))
            {
                statements.Add(new CodeCommentStatement("Notify the end of this state machine."));

                // context or context.ContextParent or context.ContextParent.ContextParent .... depending where is stateNext compared to state.
                var contextExpression = GetContextParentExpression(state, stateNext);

                // context.OnEnd();
                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                      contextExpression,
                      "OnEnd");
                statements.Add(methodInvoke);
            }
        }

        protected CodeTypeReference GetTypeReference(string type) {
            if (type == "byte")
            {
                return new CodeTypeReference(typeof(byte));
            }
            else if (type == "sbyte")
            {
                return new CodeTypeReference(typeof(sbyte));
            }
            else if (type == "bool")
            {
                return new CodeTypeReference(typeof(bool));
            }
            else if (type == "char")
            {
                return new CodeTypeReference(typeof(char));
            }
            else if (type == "short")
            {
                return new CodeTypeReference(typeof(short));
            }
            else if (type == "ushort")
            {
                return new CodeTypeReference(typeof(ushort));
            }
            else if (type == "decimal")
            {
                return new CodeTypeReference(typeof(decimal));
            }
            else if (type == "double")
            {
                return new CodeTypeReference(typeof(double));
            }
            else if (type == "float")
            {
                return new CodeTypeReference(typeof(float));
            }
            else if (type == "int")
            {
                return new CodeTypeReference(typeof(int));
            }
            else if (type == "uint")
            {
                return new CodeTypeReference(typeof(uint));
            }
            else if (type == "long")
            {
                return new CodeTypeReference(typeof(long));
            }
            else if (type == "ulong")
            {
                return new CodeTypeReference(typeof(ulong));
            }

            else if (type == "string")
            {
                return new CodeTypeReference(typeof(string));
            }
            else
            {
                return new CodeTypeReference(type);
            }
        }
    }
}
