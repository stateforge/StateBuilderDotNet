
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using CommandLine;
using CommandLine.Text;
using System.Collections;
using System.Reflection;
using System.Xml;

#endregion

namespace StateForge
{
    public sealed class StateBuilderCli
    {
        static TraceSource ts = new TraceSource("StateBuilder");
        private static readonly String companyName = "StateForge";
        private static readonly String appName = "StateBuilderCli";
        private static readonly String appVersion = "1.0";
        private static readonly HeadingInfo headingInfo = new HeadingInfo(appName, appVersion);

        private StateBuilder stateBuilder;
        private ReturnCode errorCode = ReturnCode.Ok;
        private Options options;

        public enum ReturnCode
        {
            Ok = 0,
            InvalidCommandLine = -1,
            InputFileNotFound = -2,
            IOException = -3,
            XmlException = -4,
            UnauthorizedAccessException = -5,
            Error = -6,
        }

        static void Main(string[] args)
        {
            StateBuilderCli app = new StateBuilderCli();
            ReturnCode errorCode = app.run(args);
            ts.Close();
            Environment.Exit((int)errorCode);
        }

        public StateBuilderCli()
        {
            
        }

        public ReturnCode run(string[] args)
        {
            //DisplayProperties(ts);
            ts.TraceInformation("version {0} by {1}", appVersion, companyName);
            if (parseArgs(args) == false){
                return errorCode;
            }

            try
            {
                stateBuilder.Build();
            }
            catch (IOException ioException)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "IOException: {0}", ioException.Message);
                errorCode = ReturnCode.IOException;
            }
            catch (XmlException xmlException)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "XmlException: {0}", xmlException.Message);
                errorCode = ReturnCode.XmlException;
            }
            catch (UnauthorizedAccessException unauthorizedAccessException) 
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Unauthorized Access Exception: {0}", unauthorizedAccessException.Message);
                errorCode = ReturnCode.UnauthorizedAccessException;
            }
            catch (Exception exception)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "StackTrace: {0}", exception.StackTrace);
                if (exception.InnerException != null)
                {
                    ts.TraceEvent(TraceEventType.Error, 1, "InnerException: {0}", exception.InnerException.Message);
                }
                ts.TraceEvent(TraceEventType.Error, 1, "Exception: {0}", exception.Message);
                errorCode = ReturnCode.Error;
            }
            return errorCode; 
        }

        private bool parseArgs(string[] args)
        {
            options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options) || (options.InputStateMachineFiles.Count != 1))
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Error parsing command line: {0}", Environment.CommandLine);
                errorCode = ReturnCode.InvalidCommandLine;
                return false;
            }

            stateBuilder = new StateBuilder(options.InputStateMachineFiles[0]);
            // Input file
            ts.TraceEvent(TraceEventType.Verbose, 1, "Input file: {0}", stateBuilder.InputFileName);
            // Output directory
            if(string.IsNullOrEmpty(options.outputDirectory) == false){
                stateBuilder.OutputDirectory = options.outputDirectory;
            }
            ts.TraceEvent(TraceEventType.Verbose, 1, "Output directory: {0}", stateBuilder.OutputDirectory);

            return true;
        }

        public static void DisplayProperties(TraceSource ts)
        {
            Console.WriteLine("TraceSource name = " + ts.Name);
            Console.WriteLine("TraceSource switch level = " + ts.Switch.Level);
            Console.WriteLine("TraceSource switch = " + ts.Switch.DisplayName);
            SwitchAttribute[] switches = SwitchAttribute.GetAll(Assembly.GetExecutingAssembly());
            for (int i = 0; i < switches.Length; i++)
            {
                Console.WriteLine("Switch name = " + switches[i].SwitchName);
                Console.WriteLine("Switch Type = " + switches[i].SwitchType);
            }

            // Get the custom attributes for the TraceSource.
            Console.WriteLine("Number of custom trace source attributes = "
                + ts.Attributes.Count);
            foreach (DictionaryEntry de in ts.Attributes)
                Console.WriteLine("Custom trace source attribute = "
                    + de.Key + "  " + de.Value);
            // Get the custom attributes for the trace source switch.
            foreach (DictionaryEntry de in ts.Switch.Attributes)
                Console.WriteLine("Custom switch attribute = "
                    + de.Key + "  " + de.Value);
            Console.WriteLine("Number of listeners = " + ts.Listeners.Count);
            foreach (TraceListener traceListener in ts.Listeners)
            {
                Console.Write("TraceListener: " + traceListener.Name + "\t");
                // The following output can be used to update the configuration file.
                Console.WriteLine("AssemblyQualifiedName = " +
                    (traceListener.GetType().AssemblyQualifiedName));
            }
        }

        private sealed class Options
        {
            #region Option Attribute
            [Option("d", "directory",
                    Required = false,
                    HelpText = "The directory where to write the state machine source code ")]
            public string outputDirectory = String.Empty;

            [ValueList(typeof(List<string>), 
                MaximumElements = 1)]
            public IList<string> InputStateMachineFiles = null;

            [HelpOption(
                    HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(StateBuilderCli.headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo(companyName, 2015);
                //help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                //help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: " + appName + " myFsm.fsmcs");
                help.AddOptions(this);

                return help;
            }
            #endregion
        }
    }

}
