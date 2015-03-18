#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderParallel.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Diagnostics;

    class CoderParallel : CoderBase
    {
        public CoderParallel(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
            : base(model, options, codeNamespace)
        {
        }

        public override void WriteCode()
        {
            WriteClass(Model.state);
        }

        // public class ParallelRunning : ContextParallel
        private void WriteClass(StateType state)
        {
            List<StateType> parallelList = state.ParallelList;
            if (parallelList == null)
            {
                return;
            }

            foreach (StateType stateParallel in parallelList)
            {
                string parallelClassName = GetParallelClassName(stateParallel);
                
                CodeTypeDeclaration parallelCode = new CodeTypeDeclaration(parallelClassName);
                // doc
                parallelCode.Comments.Add(new CodeCommentStatement(parallelClassName));

                // public
                parallelCode.Attributes = MemberAttributes.Public;

                parallelCode.BaseTypes.Add(new CodeTypeReference("ContextParallel"));

                CodeNamespace.Types.Add(parallelCode);

                WriteFields(parallelCode, stateParallel);

                WriteConstructor(parallelCode, stateParallel);

                WriteProperties(parallelCode, stateParallel);

                WriteTransitionToNextState(parallelCode, state, stateParallel);
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

        // private CallBase01Context  context;
        // private CallBase01ContextAlice myCallBase01ContextAlice;
        // private CallBase01ContextBob myCallBase01ContextBob;
        private void WriteFields(CodeTypeDeclaration parallelCode, StateType state)
        {
            // private CallBase01Context  context;
            CodeMemberField contextField = new CodeMemberField(GetContextClassName(state), Model.settings.context.instance);
            contextField.Attributes = MemberAttributes.Private;
            parallelCode.Members.Add(contextField);

            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                // private CallBase01ContextAlice myCallBase01ContextAlice;
                CodeMemberField objectField = new CodeMemberField(GetContextClassName(stateOrthogonal), "my" + GetContextClassName(stateOrthogonal));
                objectField.Attributes = MemberAttributes.Private;
                parallelCode.Members.Add(objectField);
            }
        }

        //public CallBase01ContextAlice CallBase01ContextAlice
        //{
        //    get
        //    {
        //        return this.myCallBase01ContextAlice;
        //    }
        //}
        private void WriteProperties(CodeTypeDeclaration parallelCode, StateType state)
        {
            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                CodeMemberProperty objectProperty = new CodeMemberProperty();
                objectProperty.Type = new CodeTypeReference(GetContextClassName(stateOrthogonal));
                objectProperty.Name = GetContextClassName(stateOrthogonal);
                objectProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                objectProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                                     "my" + GetContextClassName(stateOrthogonal))));
                parallelCode.Members.Add(objectProperty);
            }
        }

        //public ParallelRunning(CallManager callManager, CallBase01Config config, CallBase01Context contextParent)
        //{
        //    this.myCallBase01ContextAlice = new CallBase01ContextAlice(callManager, config, contextParent);
        //    this.myCallBase01ContextBob = new CallBase01ContextBob(callManager, config, contextParent);
        //}

        private void WriteConstructor(CodeTypeDeclaration parallelCode, StateType state)
        {
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.Comments.Add(new CodeCommentStatement("Constructor"));
            ObjectType[] objects = Model.settings.@object;

            foreach (ObjectType obj in Model.settings.@object)
            {
                ctor.Parameters.Add(new CodeParameterDeclarationExpression(obj.@class, obj.instance));
            }

            ctor.Parameters.Add(new CodeParameterDeclarationExpression(GetContextClassName(state), "contextParent"));

            foreach (StateType stateOrthogonal in state.parallel.state)
            {
                // new CallBase01ContextAlice(callManager, config, contextParent);
                CodeObjectCreateExpression newContextCode = new CodeObjectCreateExpression(GetContextClassName(stateOrthogonal));
                foreach (ObjectType obj in Model.settings.@object)
                {
                    newContextCode.Parameters.Add(new CodeVariableReferenceExpression(obj.instance));
                }
                newContextCode.Parameters.Add(new CodeVariableReferenceExpression("contextParent"));

                // this.CallBase01ContextAlice = new CallBase01ContextAlice(callManager, config, contextParent);
                ctor.Statements.Add(
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(),"my" + GetContextClassName(stateOrthogonal)),
                                            newContextCode));
            }

            // this.context = contextParent
            ctor.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), Model.settings.context.instance),
                                        new CodeVariableReferenceExpression("contextParent")));

            parallelCode.Members.Add(ctor);
        }

        //public override void TransitionToNextState()
        //{
        //    StateMachineHelper.ProcessTransitionBegin<CallBase01Context, CallBase01Context, CallBase01StateRoot>(this.context, CallBase01StateEnd.Instance);
        //    StateMachineHelper.ProcessTransitionEnd<CallBase01Context, CallBase01Context, CallBase01StateRoot>(this.context, CallBase01StateEnd.Instance));
        //    context.OnEnd(); for final state
        //    PersistenceParallelProcessAContext.SetInitialState(PersistenceParallelA1State.Instance);
        //    PersistenceParallelProcessBContext.SetInitialState(PersistenceParallelB1State.Instance);
        //}
        private void WriteTransitionToNextState(CodeTypeDeclaration parallelCode, StateType state, StateType stateParallel)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            parallelCode.Members.Add(method);
            method.Comments.Add(new CodeCommentStatement("Go to the next state"));
            method.Name = "TransitionToNextState";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            string stateNextName = stateParallel.parallel.nextState;

            CodeStatementCollection statements = new CodeStatementCollection();

            WriteProcessTransition(statements, state, stateNextName, "Begin");
            WriteProcessTransition(statements, state, stateNextName, "End");

            //context.OnEnd(); for final state
            WriteContextOnEnd(statements, state, Model.GetStateType(stateNextName));

            //ts.TraceEvent(TraceEventType.Verbose, 1, "WriteTransitionToNextState {0}", state.name);

            // When the parallel state is entered back, we are sure we are in the initial states for the subcontexts
            // PersistenceParallelProcessAContext.SetInitialState(PersistenceParallelA1State.Instance);
            // PersistenceParallelProcessBContext.SetInitialState(PersistenceParallelB1State.Instance);
            foreach (StateType stateOrthogonal in stateParallel.parallel.state)
            {
                // PersistenceParallelProcessAContext.SetInitialState(PersistenceParallelA1State.Instance);

                // SetInitialState(StateIdle.Instance);
                string contextName = GetContextClassName(stateOrthogonal);

                // Get the initial state
                string stateInitialClassName = GetStateClassName(GetStateInitial(stateOrthogonal));

                CodeMethodInvokeExpression setInitialStateInvoke = new CodeMethodInvokeExpression(
                                                                        new CodeVariableReferenceExpression(contextName),
                                                                        "SetInitialState");

                setInitialStateInvoke.Parameters.Add(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(stateInitialClassName), "Instance"));

                statements.Add(setInitialStateInvoke);
            }

            method.Statements.AddRange(statements);
        }
    }
}
