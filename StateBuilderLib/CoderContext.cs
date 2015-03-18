#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderContext.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    class CoderContext : CoderBase
    {
        public CoderContext(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
            : base(model, options, codeNamespace)
        {
        }

        public override void WriteCode()
        {
            WriteClass(Model.state);
        }

        // public class DoorContext : Context<StateRoot, DoorContext>

        private void WriteClass(StateType state)
        {
            if (state.Type.HasFlag(StateType.TypeFlags.TOP) == true)
            {
                string contextClassName = GetContextClassName(state);
                string contextParentClassName = GetContextParentClassName(state);
            
                string stateClassName = GetStateClassName(state);
                // class DoorContext
                CodeTypeDeclaration contextCode = new CodeTypeDeclaration(contextClassName);

                // doc
                contextCode.Comments.Add(new CodeCommentStatement(contextClassName));

                // public
                contextCode.Attributes = MemberAttributes.Public;

                // Context<StateTop, DoorContext> or ContextAsync<StateTop, DoorContext>
                contextCode.BaseTypes.Add(new CodeTypeReference(GetContextBaseClassName(state),
                                                                new CodeTypeReference[] { new CodeTypeReference(stateClassName), 
                                                                                          new CodeTypeReference(contextParentClassName) }));

                foreach (KeyValuePair<string, string> kvp in Model.EventInterfaceMap)
                {
                    string interfaceName = kvp.Key;
                    contextCode.BaseTypes.Add(new CodeTypeReference(interfaceName));
                }

                CodeNamespace.Types.Add(contextCode);

                WriteFields(contextCode);
                WriteFieldsParallel(contextCode, state);
                WriteFieldsTimer(contextCode, state);
                WriteConstructor(contextCode, state);
                WriteEvents(contextCode, state);

                WriteMethodSetState(contextCode, state);
                WriteMethodEnterInitialState(contextCode, state);
                WriteMethodLeaveCurrentState(contextCode, state);
                WriteMethodEnterHistoryState(contextCode, state);

                WriteInitializeTimers(contextCode, state);
                WriteTimersStartStop(contextCode, state);
                WriteProperties(contextCode);
                WritePropertiesParallel(contextCode, state);
            }

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

        //private Door door;
        private void WriteFields(CodeTypeDeclaration contextCode)
        {
            foreach (ObjectType obj in Model.settings.@object)
            {
                CodeMemberField objectField = new CodeMemberField(obj.@class, GetObjectFieldName(obj.instance));
                objectField.Attributes = MemberAttributes.Private;
                contextCode.Members.Add(objectField);
            }
        }

        // private ParallelRunning parallelRunning;
        private void WriteFieldsParallel(CodeTypeDeclaration contextCode, StateType state)
        {
            List<StateType> parallelList = state.ParallelList;
            if (parallelList == null)
            {
                return;
            }

            foreach (StateType stateParallel in parallelList)
            {
                CodeMemberField objectField = new CodeMemberField(GetParallelClassName(stateParallel), GetParallelFieldName(stateParallel));
                objectField.Attributes = MemberAttributes.Private;
                contextCode.Members.Add(objectField);
            }
        }

        /// <summary>
        /// Write the timers fields:
        /// private Timer timerTimerRed;
        /// private Timer timerTimerGreen;
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteFieldsTimer(CodeTypeDeclaration contextCode, StateType state)
        {
            if (Model.IsRoot(state) == false)
            {
                return;
            }
            List<TimerType> timers = Model.GetTimersAll();

            foreach (TimerType timer in timers)
            {
                CodeMemberField objectField = new CodeMemberField("System.Threading.Timer", GetTimerFieldName(timer));
                objectField.Attributes = MemberAttributes.Private;
                contextCode.Members.Add(objectField);
            }
        }

        //public Door Door
        //{
        //    get
        //    {
        //        return this.door;
        //    }
        //}
        private void WriteProperties(CodeTypeDeclaration contextCode)
        {
            foreach (ObjectType obj in Model.settings.@object)
            {
                CodeMemberProperty objectProperty = new CodeMemberProperty();
                objectProperty.Type = new CodeTypeReference(obj.@class);
                objectProperty.Name = GetPropertyName(obj);
                objectProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                objectProperty.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(),
                            GetObjectFieldName(obj.instance))));

                contextCode.Members.Add(objectProperty);
            }
        }

        //public ParallelRunning ParallelRunning
        //{
        //    get
        //    {
        //        return this.parallelRunning;
        //    }
        //}
        private void WritePropertiesParallel(CodeTypeDeclaration contextCode, StateType state)
        {
            List<StateType> parallelList = state.ParallelList;
            if (parallelList == null)
            {
                return;
            }

            foreach (StateType stateParallel in parallelList)
            {
                //CodeMemberField objectField = new CodeMemberField(GetParallelClassName(stateParallel), GetParallelFieldName(stateParallel));
                CodeMemberProperty objectProperty = new CodeMemberProperty();
                objectProperty.Type = new CodeTypeReference(GetParallelClassName(stateParallel));
                objectProperty.Name = GetParallelClassName(stateParallel);
                objectProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                objectProperty.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), 
                            GetParallelFieldName(stateParallel))));

                contextCode.Members.Add(objectProperty);
            }
        }
        //
        //public DoorContext(Door door) : base(contextParent)
        //public DoorContext(Door door, DoorContextParent contextParent) : base(contextParent)
        //public MicrowaveContext(Microwave microwave)
        //{
        //    Name = "MicrowaveContext";
        //    this._microwave = microwave;
        //    this.InitializeTimers();
        //    this._parallelOperating = new MicrowaveContextOperatingParallel(microwave, this);
        //    this.SetInitialState(MicrowaveInitialState.Instance);
        //}
        private void WriteConstructor(CodeTypeDeclaration contextCode, StateType state)
        {
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.Comments.Add(new CodeCommentStatement("Constructor"));
            ObjectType[] objects = Model.settings.@object;

            string contextClassName = GetContextClassName(state);
            string contextParentClassName = GetContextParentClassName(state);

            foreach (ObjectType obj in Model.settings.@object)
            {
                ctor.Parameters.Add(new CodeParameterDeclarationExpression(obj.@class, obj.instance));
            }

            if (Model.IsRoot(state) == false)
            {
                ctor.Parameters.Add(new CodeParameterDeclarationExpression(contextParentClassName, "contextParent"));
                ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("contextParent"));
            }

            // Name = "DoorContext";
            ctor.Statements.Add(
                new CodeAssignStatement(new CodeVariableReferenceExpression("Name"),
                                        new CodePrimitiveExpression(contextClassName)));

            // this._door = door;
            foreach (ObjectType obj in Model.settings.@object)
            {
                ctor.Statements.Add(
                            new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(),
                                    GetObjectFieldName(obj.instance)),
                                new CodeVariableReferenceExpression(obj.instance)));
            }

            WriteParallelNew(ctor, state);

            // this.InitializeTimers()
            if ((Model.GetTimersAll().Count != 0) && (Model.IsRoot(state) == true))
            {
                CodeMethodInvokeExpression initializeTimersMethod = new CodeMethodInvokeExpression(
                                                                    new CodeThisReferenceExpression(),
                                                                    "InitializeTimers");
                ctor.Statements.Add(initializeTimersMethod);
            }

            // Get the initial state
            string stateInitialClassName = GetStateClassName(GetStateInitial(state));

            // SetInitialState(StateIdle.Instance);
            CodeMethodInvokeExpression setInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                    new CodeThisReferenceExpression(),
                                                                    "SetInitialState");

            setInitialStateInvoke.Parameters.Add(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateInitialClassName), "Instance"));
            
            ctor.Statements.Add(setInitialStateInvoke);

            contextCode.Members.Add(ctor);
        }

        // this.ParallelRunning = new ParallelRunning(callManager, config, this);
        private void WriteParallelNew(CodeConstructor ctor, StateType state)
        {
            List<StateType> parallelList = state.ParallelList;
            if (parallelList == null)
            {
                return;
            }

            foreach (StateType stateParallel in parallelList)
            {
                
                string parallelClassName = GetParallelClassName(stateParallel);
                string parallelFieldName = GetParallelFieldName(stateParallel);
                // this.parallelRunning
                var thisDotParallerRunning = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(),
                    parallelFieldName);

                // new ParallelRunning(callManager, config, this);
                CodeObjectCreateExpression newParallelCode = new CodeObjectCreateExpression(parallelClassName);
                foreach (ObjectType obj in Model.settings.@object)
                {
                    newParallelCode.Parameters.Add(new CodeVariableReferenceExpression(obj.instance));
                }
                newParallelCode.Parameters.Add(new CodeThisReferenceExpression());

                // this.ParallelRunning = new ParallelRunning(callManager, config, this);
                ctor.Statements.Add(new CodeAssignStatement(thisDotParallerRunning,
                                                            newParallelCode));
            }
        }
        /// <summary>
        /// Write event methods, sync and async.
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteEvents(CodeTypeDeclaration contextCode, StateType state)
        {
            List<EventType> events = Model.GetEventsAll();
            int evenId = 1;
            foreach (EventType evt in events)
            {
                WriteEventAsync(contextCode, state, evt, evenId);
                WriteEventSync(contextCode, state, evt);
                evenId++;
            }
        }

        /// <summary>
        /// public void EvStart(String message, int error)
        /// {
        ///    this.EventQueue.Enqueue(EventFactory.CreateEvent<string, int>(this.EvStartSync, message, error));
        ///    ScheduleEvent();
        /// } 
        /// </summary>
        /// <param name="contextCode"></param>
        /// <param name="evt"></param>
        private void WriteEventAsync(CodeTypeDeclaration contextCode, StateType state, EventType evt, int evenId)
        {
            if ((Model.settings.asynchronous == false) || (state.Type.HasFlag(StateType.TypeFlags.ROOT) == false))
            {
                return;
            }

            // public void EvStart(String message, int error)
            CodeMemberMethod eventMethod = new CodeMemberMethod();
            eventMethod.Comments.Add(new CodeCommentStatement("Event " + evt.id));
            eventMethod.Name = evt.id;
            eventMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            if (evt.parameter != null)
            {
                foreach (ParameterType param in evt.parameter)
                {
                    eventMethod.Parameters.Add(new CodeParameterDeclarationExpression(GetTypeReference(param.type), param.name));
                }
            }

            string lockBegin;
            string lockEnd;
            if (Options.TargetLanguage == StateBuilderOptions.Language.VB)
            {
                lockBegin = "SyncLock Me.EventQueue";
                lockEnd = "End SyncLock";
            }
            else
            {
                lockBegin = "lock (this.EventQueue) {";
                lockEnd = "}";
            }
           
            eventMethod.Statements.Add(new CodeSnippetExpression(lockBegin));

            // this.EventQueue.Enqueue(EventFactory.CreateEvent<string, int>(this.EvStartSync, message, error));
            WriteEnqueue(eventMethod, evt, evenId);

            // ScheduleEvent()
            CodeMethodInvokeExpression scheduleEventInvoke = new CodeMethodInvokeExpression(
                                                    new CodeThisReferenceExpression(),
                                                    "ScheduleEvent");
            eventMethod.Statements.Add(scheduleEventInvoke);

            eventMethod.Statements.Add(new CodeSnippetExpression(lockEnd));
            contextCode.Members.Add(eventMethod);
        }

        /// <summary>
        /// this.EventQueue.Enqueue(EventFactory.CreateEvent<string, int>(this.EvStartSync, message, error));
        /// </summary>
        /// <param name="eventMethod"></param>
        /// <param name="evt"></param>
        private void WriteEnqueue(CodeMemberMethod eventMethod, EventType evt, int evenId)
        {
            //  this.EvStartSync
            CodeExpression thisDotEvStartSync = null;
            if (Options.TargetLanguage == StateBuilderOptions.Language.VB)
            {
                thisDotEvStartSync = new CodeDelegateCreateExpression(
                             new CodeTypeReference("System.EventHandler"), new CodeThisReferenceExpression(), GetMethodNameSync(evt));
            }
            else
            {
                thisDotEvStartSync = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetMethodNameSync(evt));
            }

            // <string, int, bool>
            CodeTypeReference[] typeRef = null;
            CodeExpression[] paramsRef = null;
            if (evt.parameter != null)
            {
                typeRef = new CodeTypeReference[evt.parameter.Length];
                paramsRef = new CodeExpression[evt.parameter.Length];

                for (var i = 0; i < evt.parameter.Length; i++)
                {
                    typeRef[i] = GetTypeReference(evt.parameter[i].type);
                    paramsRef[i] = new CodeVariableReferenceExpression(evt.parameter[i].name);
                }
            }

            //EventFactory.CreateEvent<string, int>(this.EvStartSync, message, error)
            var createInvoke = new CodeMethodInvokeExpression(
                                    new CodeMethodReferenceExpression(
                                      new CodeVariableReferenceExpression("EventFactory"),
                                       "CreateEvent",
                                       typeRef)
                                       );

            //1, 2, 3 etc ...
            createInvoke.Parameters.Add(new CodePrimitiveExpression(evenId));
            //this.EvStartSync
            createInvoke.Parameters.Add(thisDotEvStartSync);

            if (evt.parameter != null)
            {
                foreach (ParameterType param in evt.parameter)
                {
                    createInvoke.Parameters.Add(new CodeVariableReferenceExpression(param.name));
                }
            }

            // this.EventQueue.Enqueue()
            CodeMethodInvokeExpression enqueueInvoke = new CodeMethodInvokeExpression(
                                                          new CodeFieldReferenceExpression(
                                                              new CodeThisReferenceExpression(), "EventQueue"),
                                                          "Enqueue", createInvoke);

            //Add the enqueueInvoke method.
            eventMethod.Statements.Add(enqueueInvoke);
        }

        //public void EvOpenSync()
        //{
        //    StateCurrent.EvOpen(this);
        //}
        private void WriteEventSync(CodeTypeDeclaration contextCode, StateType state, EventType evt)
        {
            CodeMemberMethod eventMethod = new CodeMemberMethod();

            eventMethod.Attributes = MemberAttributes.Final;

            if ((Model.settings.asynchronous == true) && ((state.Type.HasFlag(StateType.TypeFlags.ROOT) == true)))
            {
                eventMethod.Name = GetMethodNameSync(evt);
                eventMethod.Attributes = eventMethod.Attributes | MemberAttributes.Private;
            }
            else
            {
                eventMethod.Name = evt.id;
                eventMethod.Attributes = eventMethod.Attributes | MemberAttributes.Public;
            }

            eventMethod.Comments.Add(new CodeCommentStatement(eventMethod.Name));

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                                              new CodeVariableReferenceExpression("StateCurrent"),
                                              evt.id, new CodeThisReferenceExpression());

            if (evt.parameter != null)
            {
                foreach (ParameterType param in evt.parameter)
                {
                    eventMethod.Parameters.Add(new CodeParameterDeclarationExpression(GetTypeReference(param.type), param.name));
                    methodInvoke.Parameters.Add(new CodeVariableReferenceExpression(param.name));
                }
            }
            // Declare the actuator variable if at least preAction or portAction exists.
            if ((string.IsNullOrEmpty(evt.preAction) == false) || (string.IsNullOrEmpty(evt.postAction) == false))
            {
                WriteActuatorParameterDeclaration(eventMethod);
            }

            WriteEventPrePostAction(eventMethod, evt.preAction);

            // Add StateCurrent.EvOpen(this);
            eventMethod.Statements.Add(methodInvoke);

            WriteEventPrePostAction(eventMethod, evt.postAction);

            contextCode.Members.Add(eventMethod);
        }

        /// <summary>
        /// Generate if action is not null or empty:
        /// </summary>
        /// <param name="eventMethod"></param>
        /// <param name="evt"></param>
        private void WriteEventPrePostAction(CodeMemberMethod eventMethod, string action)
        {
            if (string.IsNullOrEmpty(action) == false)
            {
                eventMethod.Statements.Add(new CodeSnippetExpression(action));
            }
        }

        /// <summary>
        /// Set the current state given its name
        ///public override void SetState(string stateName)
        ///{
        ///    if ((stateName == "Running"))
        ///    {
        ///        this.SetInitialState(PersistenceParallelRunningState.Instance);
        ///        this.ContextParallel = this.PersistenceParallelRunningParallel;
        ///        this.ContextParallel.ActiveState = 2;
        ///        return;
        ///    }
        ///    if ((stateName == "End"))
        ///    {
        ///        this.SetInitialState(PersistenceParallelEndState.Instance);
        ///        return;
        ///    }
        ///    throw new System.ArgumentException();
        ///}
        /// </summary>
        /// <param name="state"></param>
        /// <param name="contextCode"></param>
        /// <summary>
        
        private void WriteMethodSetState(CodeTypeDeclaration contextCode, StateType state)
        {
            CodeMemberMethod setStateMethod = new CodeMemberMethod();
            contextCode.Members.Add(setStateMethod);
            setStateMethod.Comments.Add(new CodeCommentStatement("Set the current state given its name"));
            setStateMethod.Name = "SetState";
            setStateMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            string stateNameVar = "stateName";
            setStateMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("System.String"), stateNameVar));

            foreach (StateType subState in state.ChildList)
            {
                var codeBinaryOperatorExpression = new CodeBinaryOperatorExpression(
                           new CodeVariableReferenceExpression(stateNameVar),
                           CodeBinaryOperatorType.IdentityEquality,
                           new CodePrimitiveExpression(subState.name));

                var conditionalStatement = new CodeConditionStatement(codeBinaryOperatorExpression);

                // PersistenceOnState.Instance
                string subStateClassName = GetStateClassName(subState);
                var stateDotInstance = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(subStateClassName), "Instance");

                // SetInitialState(PersistenceOnState.Instance);
                CodeMethodInvokeExpression setInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                        new CodeThisReferenceExpression(),
                                                                        "SetInitialState",
                                                                        stateDotInstance);
                conditionalStatement.TrueStatements.Add(setInitialStateInvoke);

                if (subState.Type.HasFlag(StateType.TypeFlags.PARALLEL) == true)
                {
                    //this.ContextParallel = this.PersistenceParallelRunningParallel;
                    var contextParallelVar = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ContextParallel");
                    var parallelVar = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetParallelClassName(subState));
                    var statementAssignParallel = new CodeAssignStatement(contextParallelVar, parallelVar);
                    conditionalStatement.TrueStatements.Add(statementAssignParallel);

                    // this.ContextParallel.ActiveState;
                    var parallelRunningDotActiveState = new CodeFieldReferenceExpression(contextParallelVar, "ActiveState");

                    // this.ContextParallel.ActiveState = 2;
                    CodeStatement parallelRunningActiveStateSet = new CodeAssignStatement(parallelRunningDotActiveState,
                                                                     new CodePrimitiveExpression(subState.parallel.state.Length));

                    conditionalStatement.TrueStatements.Add(parallelRunningActiveStateSet);
                }

                conditionalStatement.TrueStatements.Add(new CodeMethodReturnStatement());
                setStateMethod.Statements.Add(conditionalStatement);
            }

            var statetementThrow = new CodeThrowExceptionStatement(
                                               new CodeObjectCreateExpression(
                                               new CodeTypeReference(typeof(System.ArgumentException)),
                                               new CodeExpression[] {} ) );

            setStateMethod.Statements.Add(statetementThrow);
        }

        /// <summary>
        /// Generate: 
        /// void EnterInitialState()
        /// {
        ///     StateMachineHelper.WalkChainEntry<TurnstileContext, StateTop>(this, StateTop.Instance, this.StateCurrent);
        /// }
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteMethodEnterInitialState(CodeTypeDeclaration contextCode, StateType state)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            contextCode.Members.Add(codeMemberMethod);
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Call the OnEntry chain from the top state to the initial state"));
            codeMemberMethod.Name = "EnterInitialState";
            codeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            StateType stateInitial = GetStateInitial(state);

            string stateInitialClassName = GetStateClassName(stateInitial);
            string stateTopClassName = GetStateClassName(Model.GetStateTop(stateInitial));
            string contextClassName = GetContextClassName(stateInitial);
            // param StateLocked.Instance
            var stateTopDotInstance = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateTopClassName), "Instance");
            // param StateTop.Instance
            var stateInitialDotInstance = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateInitialClassName), "Instance");

            // SetInitialState(StateIdle.Instance);
            //CodeMethodInvokeExpression setInitialStateInvoke = new CodeMethodInvokeExpression(
            //                                                        new CodeThisReferenceExpression(),
            //                                                        "SetInitialState",
            //                                                        new CodeFieldReferenceExpression(
            //                                                               new CodeVariableReferenceExpression(stateInitialClassName),
            //                                                               "Instance"));

            //codeMemberMethod.Statements.Add(setInitialStateInvoke);

            // this.StateCurrent
            var stateCurrent = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "StateCurrent");

            //StateMachineHelper.WalkChainEntry<TurnstileContext, StateTop>(this, StateTop.Instance, StateLocked.Instance);
            var onMethodInvoke = new CodeMethodInvokeExpression(
                      new CodeMethodReferenceExpression(
                         new CodeVariableReferenceExpression("StateMachineHelper"),
                         "WalkChainEntry",
                         new CodeTypeReference[] {
                                    new CodeTypeReference(contextClassName),
                                    new CodeTypeReference(stateTopClassName)}),
                         new CodeExpression[] { 
                                      new CodeThisReferenceExpression(), 
                                      stateTopDotInstance, 
                                      stateCurrent, 
                                       });

            codeMemberMethod.Statements.Add(onMethodInvoke);
        }

        /// <summary>
        /// Generate: 
        /// void WriteMethodLeaveCurrentState()
        /// {
        ///     StateMachineHelper.WalkChainExit<TurnstileContext, StateTop>(this, this.StateCurrent.Instance, StateTop.Instance);
        /// }
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteMethodLeaveCurrentState(CodeTypeDeclaration contextCode, StateType state)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            contextCode.Members.Add(codeMemberMethod);
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Call the OnExit chain from the current state to the top state"));
            codeMemberMethod.Name = "LeaveCurrentState";
            codeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            string stateTopClassName = GetStateClassName(Model.GetStateTop(state));
            string contextClassName = GetContextClassName(state);
            // param StateTop.Instance
            var stateTopDotInstance = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateTopClassName), "Instance");
            // param StateLocked.Instance
            var stateCurrentDotInstance = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "StateCurrent");

            var onMethodInvoke = new CodeMethodInvokeExpression(
                      new CodeMethodReferenceExpression(
                         new CodeVariableReferenceExpression("StateMachineHelper"),
                         "WalkChainExit",
                         new CodeTypeReference[] {
                                    new CodeTypeReference(contextClassName),
                                    new CodeTypeReference(stateTopClassName)}),
                         new CodeExpression[] { 
                                      new CodeThisReferenceExpression(), 
                                      stateCurrentDotInstance, 
                                      stateTopDotInstance, 
                                       });

            codeMemberMethod.Statements.Add(onMethodInvoke);

        }

        /// <summary>
        /// Generate: 
        /// void EnterHistoryState()
        /// {
        ///     StateMachineHelper.WalkChainEntry<TurnstileContext, StateTop>(this, StateTop.Instance, this.StateHistory);
        ///     this.SetInitialState(this.StateHistory);
        /// }
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteMethodEnterHistoryState(CodeTypeDeclaration contextCode, StateType state)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            contextCode.Members.Add(codeMemberMethod);
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Call the OnEntry chain from the top state to the history state"));
            codeMemberMethod.Name = "EnterHistoryState";
            codeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            StateType stateInitial = GetStateInitial(state);

            string stateInitialClassName = GetStateClassName(stateInitial);
            string stateTopClassName = GetStateClassName(Model.GetStateTop(stateInitial));
            string contextClassName = GetContextClassName(stateInitial);
            // param StateTop.Instance
            var stateTopDotInstance = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateTopClassName), "Instance");
            // param this.StateHistory
            var thisStateHistory = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "StateHistory");

            var onMethodInvoke = new CodeMethodInvokeExpression(
                      new CodeMethodReferenceExpression(
                         new CodeVariableReferenceExpression("StateMachineHelper"),
                         "WalkChainEntry",
                         new CodeTypeReference[] {
                                    new CodeTypeReference(contextClassName),
                                    new CodeTypeReference(stateTopClassName)}),
                         new CodeExpression[] { 
                                      new CodeThisReferenceExpression(), 
                                      stateTopDotInstance, 
                                      thisStateHistory, 
                                       });

            codeMemberMethod.Statements.Add(onMethodInvoke);

            // SetInitialState(this.StateHistory);
            CodeMethodInvokeExpression setInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                    new CodeThisReferenceExpression(),
                                                                    "SetInitialState",
                                                                    thisStateHistory);

            codeMemberMethod.Statements.Add(setInitialStateInvoke);
        }

        /// <summary>
        /// 
        /// private void InitializeTimers()
        /// {
        /// 
        ///     this.timerRedTimer = new System.Threading.Timer(new TimerCallback(this.EvTimerRed));
        ///     this.timerGreenTimer = new System.Threading.Timer(new TimerCallback(this.EvTimerGreen));
        /// }
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteInitializeTimers(CodeTypeDeclaration contextCode, StateType state)
        {
            if (Model.GetTimersAll().Count == 0 ||
               (Model.settings.asynchronous == false) ||
               (Model.IsRoot(state) == false))
            {
                return;
            }

            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Initialize the timers"));
            codeMemberMethod.Name = "InitializeTimers";
            codeMemberMethod.Attributes = MemberAttributes.Private | MemberAttributes.Final;

            foreach (TimerType timer in Model.GetTimersAll())
            {
                WriteInitializeTimer(codeMemberMethod, timer);
            }

            contextCode.Members.Add(codeMemberMethod);
        }

        /// <summary>
        /// 
        ///     this.timerTimer = new System.Threading.Timer(new TimerCallback(this.EvTimer));
        /// </summary>
        /// <param name="codeMemberMethod"></param>
        /// <param name="timer"></param>
        private void WriteInitializeTimer(CodeMemberMethod codeMemberMethod, TimerType timer)
        {
            // this.timerRedTimer = new Timer();
            //codeMemberMethod.Statements.Add(
            //           new CodeAssignStatement(
            //               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
            //               new CodeObjectCreateExpression("System.Timers.Timer")));

            // new TimerCallback(this.EvTimer)
            var delegateTimerCallback = new CodeDelegateCreateExpression(
                new CodeTypeReference("TimerCallback"),
                new CodeThisReferenceExpression(), timer.id);        
            
            //this.timerTimer = new System.Threading.Timer(new TimerCallback(this.EvTimer));
            codeMemberMethod.Statements.Add(
                       new CodeAssignStatement(
                           new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
                           new CodeObjectCreateExpression("System.Threading.Timer",
                               new CodeExpression[] { 
                                   delegateTimerCallback,
                                   new CodePrimitiveExpression(null),
                                   new CodeVariableReferenceExpression("Timeout.Infinite"),
                                   new CodeVariableReferenceExpression("Timeout.Infinite")
                               })));

            // this.timerRedTimer.Elapsed += new ElapsedEventHandler(this.EvTimerRed);
            //CodeDelegateCreateExpression timerDelegate = new CodeDelegateCreateExpression(
            //                                                 new CodeTypeReference("ElapsedEventHandler"),
            //                                                 new CodeThisReferenceExpression(), timer.id);
            //CodeAttachEventStatement attachStatement = new CodeAttachEventStatement(new CodeFieldReferenceExpression(
            //                      new CodeThisReferenceExpression(), GetTimerFieldName(timer)), "Elapsed", timerDelegate);
            //codeMemberMethod.Statements.Add(attachStatement);

            // this.timerRedTimer.AutoReset = false;
            //codeMemberMethod.Statements.Add(
            //           new CodeAssignStatement(
            //               new CodeFieldReferenceExpression(
            //                   new CodeFieldReferenceExpression(
            //                       new CodeThisReferenceExpression(), GetTimerFieldName(timer)), "AutoReset"),
            //               new CodePrimitiveExpression(false)));
        }

        /// <summary>
        /// Write TimerStartXxx and TimerStopXxx methods.
        /// </summary>
        /// <param name="contextCode"></param>
        private void WriteTimersStartStop(CodeTypeDeclaration contextCode, StateType state)
        {
            if (Model.IsRoot(state) == false) {
                return;
            }

            foreach (TimerType timer in Model.GetTimersAll())
            {
                WriteTimerStart(contextCode, timer);
                WriteTimerStop(contextCode, timer);
            }
        }

        /// <summary>
        /// public void TimerStartRedTimer(long duration)
        /// {
        ///     this.Observer.OnTimerStart(Name, "RedTimer", duration);
        ///     this.timerTimer.Change(duration, Timeout.Infinite);
        ///     
        ///     Used to be:
        ///     this.timerRedTimer.Interval = duration;
        ///     this.timerRedTimer.Start();
        /// }
        private void WriteTimerStart(CodeTypeDeclaration contextCode, TimerType timer)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Start the timer " + timer.name));
            codeMemberMethod.Name = GetTimerStartName(timer);
            codeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Int64", "duration"));
            
            if (Options.NoObserver == false)
            {
                // this.Observer
                var thisDotObserver = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "Observer");

                // this.Observer.OnTimerStart(Name, "MaxDuration", duration);
                var onTimerStartMethodInvoke = new CodeMethodInvokeExpression(
                                          thisDotObserver,
                                          "OnTimerStart",
                                          new CodeExpression[] { 
                                          new CodeVariableReferenceExpression("Name"), 
                                          new CodePrimitiveExpression(timer.name), 
                                          new CodeVariableReferenceExpression("duration")});

                codeMemberMethod.Statements.Add(onTimerStartMethodInvoke);
            }

            // this.timerTimer.Change(duration, Timeout.Infinite);

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                                                          new CodeFieldReferenceExpression(
                                                              new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
                                                          "Change",
                                                          new CodeExpression[] { 
                                                              new CodeVariableReferenceExpression("duration"),
                                                              new CodeVariableReferenceExpression("Timeout.Infinite")});
            codeMemberMethod.Statements.Add(methodInvoke);

            // this.timerRedTimer.Interval = duration;
            //codeMemberMethod.Statements.Add(
            //           new CodeAssignStatement(
            //               new CodeFieldReferenceExpression(
            //                   new CodeFieldReferenceExpression(
            //                       new CodeThisReferenceExpression(), GetTimerFieldName(timer)), "Interval"),
            //               new CodeVariableReferenceExpression("duration")));
            
            // this.timerRedTimer.Start();
            //CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
            //                                              new CodeFieldReferenceExpression(
            //                                                  new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
            //                                              "Start");
            //codeMemberMethod.Statements.Add(methodInvoke);
            contextCode.Members.Add(codeMemberMethod);
        }

        /// <summary>
        /// public void TimerStopRedTimer()
        /// {
        ///     this.Observer.OnTimerStop(Name, "RedTimer");
        ///     this.timerTimer.Change(Timeout.Infinite, Timeout.Infinite);
        /// }
        /// </summary>
        /// <param name="contextCode"></param>
        /// <param name="timer"></param>
        private void WriteTimerStop(CodeTypeDeclaration contextCode, TimerType timer)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            codeMemberMethod.Comments.Add(new CodeCommentStatement("Stop the timer " + timer.name));
            codeMemberMethod.Name = GetTimerStopName(timer);
            codeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            if (Options.NoObserver == false)
            {
                // this.Observer
                var thisDotObserver = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "Observer");

                // this.Observer.OnTimerStop(Name, "RedTimer");
                var onTimerStopMethodInvoke = new CodeMethodInvokeExpression(
                                          thisDotObserver,
                                          "OnTimerStop",
                                          new CodeExpression[] { 
                                          new CodeVariableReferenceExpression("Name"), 
                                          new CodePrimitiveExpression(timer.name)});

                codeMemberMethod.Statements.Add(onTimerStopMethodInvoke);
            }
            // this.timerRedTimer.Stop();
            //CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
            //                                              new CodeFieldReferenceExpression(
            //                                                  new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
            //                                              "Stop");

            //this.timerTimer.Change(Timeout.Infinite, Timeout.Infinite);
            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                                              new CodeFieldReferenceExpression(
                                                  new CodeThisReferenceExpression(), GetTimerFieldName(timer)),
                                              "Change",
                                              new CodeExpression[] { 
                                                              new CodeVariableReferenceExpression("Timeout.Infinite"),
                                                              new CodeVariableReferenceExpression("Timeout.Infinite")});

            codeMemberMethod.Statements.Add(methodInvoke);
            contextCode.Members.Add(codeMemberMethod);
        }

        // Actuator actuator1;
        // actuator1 = this.Actuator1;
        // Actuator actuator2;
        // actuator2 = this.Actuator2;
        private void WriteActuatorParameterDeclaration(CodeMemberMethod eventMethod)
        {
            foreach (ObjectType obj in Model.settings.@object)
            {
                eventMethod.Statements.Add(new CodeVariableDeclarationStatement(obj.@class, obj.instance));
                eventMethod.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(obj.instance),
                    new CodeFieldReferenceExpression(
                      new CodeThisReferenceExpression(),GetPropertyName(obj))));
            }
        }


    }
}
