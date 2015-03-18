#region Copyright
//------------------------------------------------------------------------------
// <copyright file="CoderStateMachine.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.CodeDom;
    
    public class CoderStateMachine : CoderBase
    {
        private CoderFeeder coderFeeder;
        private CoderContext coderContext;
        private CoderParallel coderParallel;
        private CoderState coderState;

        public CoderStateMachine(StateMachineType model, StateBuilderOptions options, CodeNamespace codeNamespace)
            : base(model, options, codeNamespace)
        {
            this.coderFeeder = new CoderFeeder(model, options, codeNamespace);
            this.coderContext = new CoderContext(model, options, codeNamespace);
            this.coderParallel = new CoderParallel(model, options, codeNamespace);
            this.coderState = new CoderState(model, options, codeNamespace);
        }

        public override void WriteCode()
        {
            ts.TraceInformation("WriteCode");
            this.coderFeeder.WriteCode();
            this.coderContext.WriteCode();
            this.coderParallel.WriteCode();
            this.coderState.WriteCode();
        }


    }
}
