#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderFeeder.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    class CoderFeeder : CoderBase
    {
        public CoderFeeder(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
            : base(model, options, codeNamespace)
        {
        }

        public override void WriteCode()
        {
            foreach (var pair in Model.FeedersMap)
            {
                WriteFeeder(pair.Key, pair.Value);
            }
        }

        //public partial class HelloWorld
        //{
        //    public void Print()
        //    {
        //        context.EvPrint();
        //    }
        //}
        private void WriteFeeder(string feeder, List<EventType> events)
        {
            CodeTypeDeclaration feederCode = new CodeTypeDeclaration(feeder);
            CodeNamespace.Types.Add(feederCode);
            // doc
            feederCode.Comments.Add(new CodeCommentStatement(feeder));

            // public
            feederCode.Attributes = MemberAttributes.Public;

            // partial
            feederCode.IsPartial = true;

            foreach (EventType evt in events)
            {
                WriteEvent(feederCode, evt);
            }
        }

        //    public void Print()
        //    {
        //        context.EvPrint();
        //    }
        private void WriteEvent(CodeTypeDeclaration feederCode, EventType evt)
        {
            CodeMemberMethod eventMethod = new CodeMemberMethod();
            feederCode.Members.Add(eventMethod);

            eventMethod.Attributes = MemberAttributes.Final;

            eventMethod.Name = evt.id;
            eventMethod.Attributes = eventMethod.Attributes | MemberAttributes.Public;

            eventMethod.Comments.Add(new CodeCommentStatement(eventMethod.Name));

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                                              new CodeVariableReferenceExpression(Model.settings.context.instance),
                                              evt.id);

            if (evt.parameter != null)
            {
                foreach (ParameterType param in evt.parameter)
                {
                    eventMethod.Parameters.Add(new CodeParameterDeclarationExpression(GetTypeReference(param.type), param.name));
                    methodInvoke.Parameters.Add(new CodeVariableReferenceExpression(param.name));
                }
            }

            // Add StateCurrent.EvOpen(this);
            eventMethod.Statements.Add(methodInvoke);
        }
    }
}
