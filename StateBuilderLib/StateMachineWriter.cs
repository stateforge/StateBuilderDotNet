#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateMachineWriter.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.IO;

    public class StateMachineWriter
    {
        public int IndentLevel { get; set; }
        public int IndentSize { get; set; }
        public string space = String.Empty;
        public StreamWriter StreamWriter {get; private set;}

        public StateMachineWriter(StreamWriter streamWriter) {
            StreamWriter = streamWriter;
            IndentSize = 4;
            IndentLevel = 0;
        }

        public void WriteLine(string line) {
            StreamWriter.Write(space);
            StreamWriter.WriteLine(line);
        }

        public void Indent()
        {
            IndentLevel += 1;
            BuildIndent();
        }

        public void UnIndent()
        {
            IndentLevel -= 1;
            Debug.Assert(IndentLevel >= 0);

            BuildIndent();
        }

        private void BuildIndent()
        {
            space = "";
            for (int i = 0; i < IndentLevel * IndentSize; i++){
                space += " ";
            }
        }
    }
}
