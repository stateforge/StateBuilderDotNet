#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateBuilderOptions.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;

    public class StateBuilderOptions
    {
        public enum Language
        {
            CS,
            VB
        }

        public StateBuilderOptions()
        {
            TargetLanguage = Language.CS;
        }

        public Language TargetLanguage { get; set; }
        public bool NoObserver { get; set; }
        public string PrependFile { get; set; }
    }
}
