#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderState.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    class CoderState : CoderBase
    {
        public CoderState(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
            : base(model, options, codeNamespace)
        {
        }

        public override void WriteCode()
        {
            ts.TraceInformation("WriteCode for states");
            WriteClass(Model.state);
        }

        // public class StateRoot : State<HelloWorldContext, StateRoot>
        private void WriteClass(StateType state)
        {

            if (state.Type.HasFlag(StateType.TypeFlags.HISTORY))
            {
                // Do not generate a state class for pseudo state history.
                return;
            }

            string stateClassName = GetStateClassName(state);
            string contextClassName = GetContextClassName(state);

            // doc
            // CodeNamespace.Comments.Add(new CodeCommentStatement(state.name));

            // Create the state class.
            CodeTypeDeclaration stateCode = new CodeTypeDeclaration(stateClassName);

            // Add the state class to the code.
            CodeNamespace.Types.Add(stateCode);

            // Add state Type in doc
            stateCode.Comments.Add(new CodeCommentStatement("State " + state.name));

            // public
            stateCode.Attributes = MemberAttributes.Public;

            if (state.Parent == null)
            {
                stateCode.BaseTypes.Add(
                    new CodeTypeReference("State",
                        new CodeTypeReference[] { new CodeTypeReference(contextClassName), 
                                                  new CodeTypeReference(stateClassName) }));
            }
            else
            {
                string stateParentClassName = GetStateClassName(state.Parent);
                stateCode.BaseTypes.Add(new CodeTypeReference(stateParentClassName));
            }

            WriteInstanceField(stateCode, state);
            WriteConstructor(stateCode, state);
            WriteGetInstance(stateCode, state);
            WriteOnEntryExit(stateCode, state, state.onEntry, "OnEntry");
            WriteOnEntryExit(stateCode, state, state.onExit, "OnExit");
            WriteEvents(stateCode, state);

            if (state.parallel != null)
            {
                foreach (StateType stateOrthogonal in state.parallel.state)
                {
                    WriteClass(stateOrthogonal);
                }
            }

            if (state.state != null)
            {
                foreach (StateType stateChild in state.state)
                {
                    WriteClass(stateChild);
                }
            }
        }

        //protected StateIdle()
        //{
        //    Name = "Idle";
        //    Kind = StateKind.Leaf;
        //    StateParent = StateTop.Instance;
        //}
        private void WriteConstructor(CodeTypeDeclaration stateCode, StateType state)
        {
            CodeConstructor ctor = new CodeConstructor();
            // protected
            ctor.Attributes = MemberAttributes.Family;
            ctor.Comments.Add(new CodeCommentStatement("Constructor"));

            // Name = "Idle";
            ctor.Statements.Add(
                new CodeAssignStatement(new CodeVariableReferenceExpression("Name"),
                                        new CodePrimitiveExpression(state.name)));

            //StateParent = StateTop.Instance;
            if (state.Parent != null)
            {
                ctor.Statements.Add(
                                new CodeAssignStatement(new CodeVariableReferenceExpression("StateParent"),
                                new CodeFieldReferenceExpression(
                                    new CodeVariableReferenceExpression(GetStateClassName(state.Parent)), "Instance")));
            }
            stateCode.Members.Add(ctor);
        }

        //public override void OnEntry(HelloWorldContext context)
        //{
        //    context.Observer.OnEntry(context.Name, StateTop.Instance.Name);
        //}
        private void WriteOnEntryExit(CodeTypeDeclaration stateCode, StateType state, ActionsType action, string onEntryExit)
        {
            CodeMemberMethod onMethod = new CodeMemberMethod();
            onMethod.Comments.Add(new CodeCommentStatement(onEntryExit));
            onMethod.Name = onEntryExit;
            onMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            onMethod.Parameters.Add(new CodeParameterDeclarationExpression(GetContextClassName(state), Model.settings.context.instance));

            if (onEntryExit == "OnExit")
            {
                // context.SaveState();
                WriteOnExitHistory(state, onMethod);
                WriteOnExitParallel(onMethod, state);
            }

            if (Options.NoObserver == false)
            {
                // context.Observer
                var contextDotObserver = new CodeFieldReferenceExpression(
                    new CodeVariableReferenceExpression(Model.settings.context.instance), "Observer");

                // param context.Name
                var contextDotName = new CodeFieldReferenceExpression(
                    new CodeVariableReferenceExpression(Model.settings.context.instance), "Name");

                // param StateRunning.Instance
                var stateLeafDotInstance = new CodeFieldReferenceExpression(
                    new CodeVariableReferenceExpression(GetStateClassName(state)), "Instance");

                // param StateTop.Instance.Name
                var stateLeafDotInstanceDotName = new CodeFieldReferenceExpression(stateLeafDotInstance, "Name");

                // context.Observer.OnExit(context.Name, StateTop.Instance.Name);
                var onMethodInvoke = new CodeMethodInvokeExpression(
                                      contextDotObserver,
                                      onEntryExit, new CodeExpression[] { contextDotName, stateLeafDotInstanceDotName });

                onMethod.Statements.Add(onMethodInvoke);
            }

            if (action != null)
            {
                WriteActuatorParameterDeclaration(onMethod);

                CodeStatementCollection statements = new CodeStatementCollection();
                WriteActions(statements, state, action.action, action.Items);
                onMethod.Statements.AddRange(statements);
            }

            if (onEntryExit == "OnEntry")
            {
                WriteOnEntryParallel(onMethod, state);
            }

            stateCode.Members.Add(onMethod);
        }

        /// <summary>
        /// When leaving a state which has an history state, save the state into the history state.
        /// context.SaveState
        /// </summary>
        /// <param name="state"></param>
        /// <param name="onMethod"></param>
        private void WriteOnExitHistory(StateType state, CodeMemberMethod onMethod)
        {
            if (state.Type.HasFlag(StateType.TypeFlags.HAS_HISTORY))
            {
                var contextDotSaveHistoryStateInvoke = new CodeMethodInvokeExpression(
                                        new CodeVariableReferenceExpression(Model.settings.context.instance),
                                        "SaveState");
                onMethod.Statements.Add(contextDotSaveHistoryStateInvoke);
            }
        }

        //ParallelRunning parallelRunning = context.ParallelRunning;
        //context.ContextParallel = parallelRunning;
        //parallelRunning.ActiveState = 2;
        //parallelRunning.CallBase01ContextAlice.EnterInitialState();
        //parallelRunning.CallBase01ContextBob.EnterInitialState();

        private void WriteOnEntryParallel(CodeMemberMethod method, StateType state)
        {
            if (state.parallel == null)
            {
                return;
            }
            string parallelClassName = GetParallelClassName(state);
            string parallelLocalVariableName = GetParallelLocalVariableName(state);
            // ParallelRunning parallelRunning = context.ParallelRunning;

            method.Statements.Add(new CodeVariableDeclarationStatement(parallelClassName, parallelLocalVariableName));
            method.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(parallelLocalVariableName),
                  new CodeFieldReferenceExpression(
                      new CodeVariableReferenceExpression(Model.settings.context.instance), parallelClassName)));

            // context.ContextParallel
            var contextDotObserver = new CodeFieldReferenceExpression(
                new CodeVariableReferenceExpression(Model.settings.context.instance),
                "ContextParallel");

            //context.ContextParallel = parallelRunning;
            method.Statements.Add(new CodeAssignStatement(contextDotObserver,
                                                          new CodeVariableReferenceExpression(parallelLocalVariableName)));

            // parallelRunning.ActiveState;
            var parallelRunningDotActiveState = new CodeFieldReferenceExpression(
                                                    new CodeVariableReferenceExpression(parallelLocalVariableName),
                                                                                        "ActiveState");
            //parallelRunning.ActiveState == 0
            CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(                
                parallelRunningDotActiveState,              
                CodeBinaryOperatorType.IdentityEquality,
                new CodePrimitiveExpression(0));    

            CodeConditionStatement conditionalStatement = new CodeConditionStatement(condition);

            

            // parallelRunning.ActiveState = 2;
            CodeStatement parallelRunningActiveStateSet = new CodeAssignStatement(parallelRunningDotActiveState,
                                                             new CodePrimitiveExpression(state.parallel.state.Length));

            if (state.HasParentStateHistory())
            {
                method.Statements.Add(conditionalStatement);
                conditionalStatement.TrueStatements.Add(parallelRunningActiveStateSet);
            }
            else
            {
                method.Statements.Add(parallelRunningActiveStateSet);
            }

            //parallelRunning.CallBase01ContextAlice.EnterInitialState();
            //parallelRunning.CallBase01ContextBob.EnterInitialState();

            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                string contextOrthogonalClassName = GetContextClassName(stateOrthogonal);
                string stateOrthogonalClassName = GetStateClassName(stateOrthogonal);

                // parallelRunning.CallBase01ContextAlice
                var parallelStateNameDotContextOrthogonal = new CodeFieldReferenceExpression(
                     new CodeVariableReferenceExpression(parallelLocalVariableName),
                                                         contextOrthogonalClassName);

                //parallelRunning.CallBase01ContextAlice.EnterInitialState();
                var enterInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                    parallelStateNameDotContextOrthogonal,
                                                                    "EnterInitialState");

                //parallelRunning.CallBase01ContextAlice.EnterHistoryState();
                var enterInitialHistoryStateInvoke = new CodeMethodInvokeExpression(
                                                                    parallelStateNameDotContextOrthogonal,
                                                                    "EnterHistoryState");
                if (state.HasParentStateHistory())
                {
                    conditionalStatement.TrueStatements.Add(enterInitialStateInvoke);
                    conditionalStatement.FalseStatements.Add(enterInitialHistoryStateInvoke);
                }
                else
                {
                    method.Statements.Add(enterInitialStateInvoke);
                }
            }
        }

        //ParallelRunning parallelRunning = context.ParallelRunning;
        //context.ContextParallel = null;
        //parallelRunning.CallBase01ContextAlice.LeavingCurrentState();
        //parallelRunning.CallBase01ContextBob.LeavingCurrentState();
        private void WriteOnExitParallel(CodeMemberMethod method, StateType state)
        {
            if (state.parallel == null)
            {
                return;
            }

            string parallelClassName = GetParallelClassName(state);
            string parallelLocalVariableName = GetParallelLocalVariableName(state);
            // ParallelRunning parallelRunning = context.ParallelRunning;

            method.Statements.Add(new CodeVariableDeclarationStatement(parallelClassName, parallelLocalVariableName));
            method.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(parallelLocalVariableName),
                  new CodeFieldReferenceExpression(
                      new CodeVariableReferenceExpression(Model.settings.context.instance), parallelClassName)));

            // context.ContextParallel
            var contextDotObserver = new CodeFieldReferenceExpression(
                new CodeVariableReferenceExpression(Model.settings.context.instance),
                "ContextParallel");

            //context.ContextParallel = null;
            method.Statements.Add(new CodeAssignStatement(contextDotObserver,
                                                          new CodePrimitiveExpression(null)));

            // parallelRunning.CallBase01ContextAlice.LeavingCurrentState();
            // parallelRunning.CallBase01ContextBob.LeavingCurrentState();
            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                string contextOrthogonalClassName = GetContextClassName(stateOrthogonal);
                string stateOrthogonalClassName = GetStateClassName(stateOrthogonal);

                // parallelRunning.CallBase01ContextAlice
                var parallelStateNameDotContextOrthogonal = new CodeFieldReferenceExpression(
                     new CodeVariableReferenceExpression(parallelLocalVariableName),
                                                         contextOrthogonalClassName);

                //parallelRunning.CallBase01ContextAlice.EnterInitialState();
                var enterInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                    parallelStateNameDotContextOrthogonal,
                                                                    "LeaveCurrentState");

                method.Statements.Add(enterInitialStateInvoke);
            }
        }

        // private static StateIdle instance = new StateIdle();
        private void WriteInstanceField(CodeTypeDeclaration stateCode, StateType state)
        {
            CodeMemberField objectField = new CodeMemberField(GetStateClassName(state), "_instance");
            objectField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
            objectField.InitExpression = new CodeObjectCreateExpression(GetStateClassName(state));
            stateCode.Members.Add(objectField);
        }

        //public static StateRunning Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}
        private void WriteGetInstance(CodeTypeDeclaration stateCode, StateType state)
        {
            CodeMemberProperty instanceProperty = new CodeMemberProperty();
            instanceProperty.Type = new CodeTypeReference(GetStateClassName(state));
            instanceProperty.Name = "Instance";
            instanceProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            if (state.Parent != null)
            {
                instanceProperty.Attributes |= MemberAttributes.New;
            }
            instanceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("_instance")));

            stateCode.Members.Add(instanceProperty);
        }

        private void WriteEvents(CodeTypeDeclaration stateCode, StateType state)
        {
            List<EventType> events = Model.GetEventsForState(state);
            stateCode.Comments.Add(new CodeCommentStatement("State " + state.name + " handles " + events.Count + " event(s)"));
            foreach (EventType evt in events)
            {
                WriteEvent(stateCode, state, evt);
            }
        }

        // public override void EvPrint(HelloWorldContext context)
        // {
        // }
        private void WriteEvent(CodeTypeDeclaration stateCode, StateType state, EventType evt)
        {
            if (evt == null)
            {
                //TODO HEEFRE throw exception
                return;
            }
            CodeMemberMethod eventMethod = new CodeMemberMethod();
            eventMethod.Comments.Add(new CodeCommentStatement("Handle event " + evt.id + " for state " + state.name));
            eventMethod.Name = evt.id;

            if (state.Type.HasFlag(StateType.TypeFlags.TOP))
            {
                eventMethod.Attributes = MemberAttributes.Public;
            }
            else
            {
                eventMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            }




            eventMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(GetContextClassName(state),
                                                       Model.settings.context.instance));

            if (evt.parameter != null)
            {
                foreach (ParameterType param in evt.parameter)
                {
                    eventMethod.Parameters.Add(new CodeParameterDeclarationExpression(GetTypeReference(param.type), param.name));
                }
            }

            stateCode.Members.Add(eventMethod);

            // Body

            if (state.parallel != null)
            {
                WriteEventParallel(eventMethod, state, evt);
            }

            WriteEventLeaf(eventMethod, state, evt);
        }


        //Ping ping = context.Ping;
        //    if (ping.Tx == ping.Count) {
        //        StateMachineHelper.ProcessTransitionBegin<PingContext, PingContext, PingStateRoot>(context, PingStateEnd.Instance");
        //        StateMachineHelper.ProcessTransitionEnd<PingContext, PingContext, PingStateRoot>(context, PingStateEnd.Instance");
        //        // Notify the end of the state machine.
        //        context.OnEnd();
        //        return;
        //    }
        //    StateMachineHelper.ProcessTransitionBegin<PingContext, PingContext, PingStateRoot>(context, PingStateSendPingAndWaitForReply.Instance);
        //    StateMachineHelper.ProcessTransitionEnd<PingContext, PingContext, PingStateRoot>(context, PingStateSendPingAndWaitForReply.Instance);
        //    return;
        private void WriteEventLeaf(CodeMemberMethod eventMethod, StateType state, EventType evt)
        {
            List<TransitionType> transitions = Model.GetTransitionList(state, evt.id);

            if (transitions.Count != 0)
            {
                WriteActuatorParameterDeclaration(eventMethod);
            }

            eventMethod.Comments.Add(new CodeCommentStatement(transitions.Count + " transition(s) for event " + evt.id));

            foreach (TransitionType transition in transitions)
            {
                WriteTransition(eventMethod, state, transition);
            }
        }
        //ParallelRunning parallelRunning = context.ParallelRunning;
        //parallelRunning.CallBase01ContextAlice.EvStart();
        //parallelRunning.CallBase01ContextBob.EvStart();
        private void WriteEventParallel(CodeMemberMethod eventMethod, StateType state, EventType evt)
        {
            eventMethod.Comments.Add(new CodeCommentStatement("State " + state.name + " is parallel"));

            string parallelClassName = GetParallelClassName(state);
            string parallelLocalVariableName = GetParallelLocalVariableName(state);
            // ParallelRunning parallelRunning = context.ParallelRunning;
            eventMethod.Statements.Add(new CodeVariableDeclarationStatement(parallelClassName, parallelLocalVariableName));
            eventMethod.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(parallelLocalVariableName),
                  new CodeFieldReferenceExpression(
                      new CodeVariableReferenceExpression(Model.settings.context.instance), parallelClassName)));

            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                string contextOrthogonalClassName = GetContextClassName(stateOrthogonal);
                string stateOrthogonalClassName = GetStateClassName(stateOrthogonal);

                // parallelRunning.CallBase01ContextAlice
                var parallelStateNameDotContextOrthogonal = new CodeFieldReferenceExpression(
                     new CodeVariableReferenceExpression(parallelLocalVariableName),
                                                         contextOrthogonalClassName);

                //parallelRunning.CallBase01ContextAlice.EvStart("HelloWorld", 2);
                var eventInvoke = new CodeMethodInvokeExpression(
                                      parallelStateNameDotContextOrthogonal,
                                      evt.id);

                // ("HelloWorld", 2)
                if (evt.parameter != null)
                {
                    foreach (ParameterType param in evt.parameter)
                    {
                        eventInvoke.Parameters.Add(new CodeVariableReferenceExpression(param.name));
                    }
                }
                eventMethod.Statements.Add(eventInvoke);
            }

            // Pass the event to the parent state.
            // StateTop.Instance.evStart(context);

            //StateTop.Instance
            StateType stateParent = state.Parent;
            if (stateParent != null)
            {
                // param StateRunning.Instance
                var stateTopDotInstance = new CodeFieldReferenceExpression(
                    new CodeVariableReferenceExpression(GetStateClassName(stateParent)), "Instance");

                // StateTop.Instance.evStart
                var eventParentInvoke = new CodeMethodInvokeExpression(
                                      stateTopDotInstance,
                                      evt.id);

                //(context, 
                eventParentInvoke.Parameters.Add(new CodeVariableReferenceExpression(Model.settings.context.instance));
                // ("HelloWorld", 2)
                if (evt.parameter != null)
                {
                    foreach (ParameterType param in evt.parameter)
                    {
                        eventParentInvoke.Parameters.Add(new CodeVariableReferenceExpression(param.name));
                    }
                }

                eventMethod.Statements.Add(eventParentInvoke);
            }
        }

        //
        // if (connection.Endpoint.Name == "CpeB") {
        //    context.TransitionName = "EvConnectionIncoming[connection.Endpoint.Name == \"CpeB\"]";
        //    StateMachineHelper.ProcessTransitionBegin<CallBase01ContextAlice, CallBase01Context, CallBase01StateAlice>(context, CallBase01StateA_End.Instance);
        //    StateMachineHelper.ProcessTransitionEnd<CallBase01ContextAlice, CallBase01Context, CallBase01StateAlice>(context, CallBase01StateA_End.Instance);
        //    // Notify the end of this state machine.
        //    context.OnEnd();
        //    return;
        //}
        private void WriteTransition(CodeMemberMethod eventMethod, StateType state, TransitionType transition)
        {
            StateType stateNext = Model.GetStateType(transition.nextState);

            if (stateNext == null)
            {
                eventMethod.Comments.Add(
                new CodeCommentStatement(
                    "Internal transition from state " + state.name + " triggered by event " + Model.GetTransitionName(transition)));
            }
            else if (stateNext == state)
            {
                eventMethod.Comments.Add(new CodeCommentStatement("Self transition from state " + state.name +
                    " triggered by event " + Model.GetTransitionName(transition)));
            }
            else
            {
                eventMethod.Comments.Add(new CodeCommentStatement(
                       "Transition from state " + state.name +
                       " to " + transition.nextState +
                       " triggered by event " + Model.GetTransitionName(transition)));

                if (stateNext.StateParallel == state.StateParallel)
                {
                    eventMethod.Comments.Add(
                        new CodeCommentStatement(
                            "The next state " + transition.nextState +
                            " is within the context " + GetContextClassName(state)));

                    if (stateNext.Type.HasFlag(StateType.TypeFlags.HISTORY))
                    {
                        eventMethod.Comments.Add(
                            new CodeCommentStatement(
                                "The next state " + transition.nextState + " is historical"));
                    }
                }
                else
                {
                    eventMethod.Comments.Add(
                        new CodeCommentStatement(
                            "The next state " + transition.nextState + " belonging to context " + GetContextClassName(stateNext) +
                            " is outside the context " + GetContextClassName(state)));
                }
            }

            string condition = Model.GetCondition(transition);
            if (String.IsNullOrEmpty(condition) == false)
            {
                CodeConditionStatement transitionCondition = new CodeConditionStatement(
                    // The condition to test.
                               new CodeSnippetExpression(condition),
                    // The statements to execute if the condition evaluates to true.
                               GetProcessTransitionStatements(state, transition));

                eventMethod.Statements.Add(transitionCondition);
            }
            else
            {
                eventMethod.Statements.AddRange(GetProcessTransitionStatements(state, transition));
            }

        }

        private CodeStatement[] GetProcessTransitionStatements(StateType state, TransitionType transition)
        {
            CodeStatementCollection statements = new CodeStatementCollection();

            string stateNextName = transition.nextState;
            StateType stateNext = Model.GetStateType(stateNextName);

            // context.TransitionName
            var contextDotTransitionName = new CodeFieldReferenceExpression(
                new CodeVariableReferenceExpression(Model.settings.context.instance),
                "TransitionName");

            // context.TransitionName  = "EvFoo[condition]"
            statements.Add(
                new CodeAssignStatement(contextDotTransitionName,
                                        new CodePrimitiveExpression(Model.GetTransitionName(transition))));

            WriteProcessTransition(statements, state, stateNextName, "Begin");
            WriteActionsFromTransition(statements, state, transition);
            WriteProcessTransition(statements, state, stateNextName, "End");

            //Special case when the next state is a final state.
            //context.OnEnd();
            WriteContextOnEnd(statements, state, stateNext);

            statements.Add(new CodeMethodReturnStatement());
            CodeStatement[] statementArray = new CodeStatement[statements.Count];
            statements.CopyTo(statementArray, 0);
            return statementArray;
        }


        private void WriteActionsFromTransition(CodeStatementCollection statements, StateType state, TransitionType transition)
        {
            WriteActions(statements, state, transition.action, transition.Items);
        }

        private void WriteActions(CodeStatementCollection statements, StateType state, String actionAttribute, Object[] actions)
        {
            if (String.IsNullOrEmpty(actionAttribute) == false)
            {
                statements.Add(new CodeSnippetExpression(actionAttribute));
            }

            WriteActionGroup(statements, state, actions);
        }

        private void WriteActionGroup(CodeStatementCollection statements, StateType state, Object[] actions)
        {

            if (actions == null)
            {
                return;
            }

            foreach (Object action in actions)
            {
                if (action is String)
                {
                    statements.Add(new CodeSnippetExpression((String)action));

                }
                else if (action is TimerStartActionType)
                {
                    WriteTimerStart(statements, state, (TimerStartActionType)action);
                }
                else if (action is TimerStopActionType)
                {
                    WriteTimerStop(statements, state, (TimerStopActionType)action);
                }
            }
        }

        /// <summary>
        /// context.TimerStartRedTimer(light.TimerRedDuration);
        /// or 
        /// context.ContextParent.TimerStartRedTimer(light.TimerRedDuration);
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="timerStart"></param>
        private void WriteTimerStart(CodeStatementCollection statements, StateType state, TimerStartActionType timerStart)
        {
            statements.Add(new CodeCommentStatement("Start the timer " + timerStart.timer + ", duration " + timerStart.duration + " msec"));

            // context or context.ContextParent or context.ContextParent.ContextParent ....
            var contextExpression = GetContextParentExpression(state, Model.state.state[0]);

            // context.TimerStartRedTimer(light.TimerRedDuration);
            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                       contextExpression,
                       GetTimerStartName(Model.GetTimerFromName(timerStart.timer)),
                       new CodeVariableReferenceExpression(timerStart.duration));
            statements.Add(methodInvoke);
        }

        /// <summary>
        /// context.TimerStopRedTimer();
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="timerStart"></param>
        private void WriteTimerStop(CodeStatementCollection statements, StateType state, TimerStopActionType timerStop)
        {
            statements.Add(new CodeCommentStatement("Stop the timer " + timerStop.timer));
            // context.TimerStopRedTimer();
            // context or context.ContextParent or context.ContextParent.ContextParent ....
            var contextExpression = GetContextParentExpression(state, Model.state.state[0]);

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                                  contextExpression,
                                  GetTimerStopName(Model.GetTimerFromName(timerStop.timer)));
            statements.Add(methodInvoke);
        }

        // Actuator actuator1;
        // actuator1 = context.Actuator1;
        // Actuator actuator2;
        // actuator2 = context.Actuator2;
        private void WriteActuatorParameterDeclaration(CodeMemberMethod eventMethod)
        {
            foreach (ObjectType obj in Model.settings.@object)
            {
                eventMethod.Statements.Add(new CodeVariableDeclarationStatement(obj.@class, obj.instance));
                eventMethod.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(obj.instance),
                  new CodeFieldReferenceExpression(
                      new CodeVariableReferenceExpression(Model.settings.context.instance), GetPropertyName(obj))));
            }
        }

    }
}
