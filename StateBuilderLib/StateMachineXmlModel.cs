#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateMachineXmlModel.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using ICSharpCode.NRefactory;
    using ICSharpCode.NRefactory.Ast;
    using ICSharpCode.NRefactory.PrettyPrinter;

    class StateMachineXmlModel
    {
        private static TraceSource ts = new TraceSource("StateBuilder");
        /// <summary>
        /// The input state machine filename in XML format.
        /// </summary>
        public string InputFileName { get; set; }

        /// <summary>
        /// The Xml Shema that describes the state machine.  
        /// </summary>
        private string schemaName = "StateMachineDotNet-v1.xsd";

        /// <summary>
        /// Xml tags
        /// </summary>
        static public readonly string TAG_NAME = "name";
        static public readonly string TAG_EVENT_ID = "id";
        static public readonly string TAG_STATE = "state";
        static public readonly string TAG_ONENTRY = "onEntry";
        static public readonly string TAG_ONEXIT = "onExit";
        static public readonly string TAG_STATE_NAME = "name";
        static public readonly string TAG_NEXT_STATE = "nextState";
        static public readonly string TAG_EVENT_LIST = "events";
        static public readonly string TAG_EVENT_PROVIDER = "eventSource";
        static public readonly string TAG_EVENT_INTERFACE = "interface";
        static public readonly string TAG_EVENT_FILE = "file";
        static public readonly string TAG_EVENT = "event";
        static public readonly string TAG_TRANSITION = "transition";
        static public readonly string TAG_CONDITION = "condition";
        static public readonly string TAG_KIND = "kind";
        static public readonly string TAG_TIMER = "timer";
        static public readonly string TAG_TIMER_START = "timerStart";
        static public readonly string TAG_TIMER_STOP = "timerStop";
        static public readonly string TAG_DURATION = "duration";
        static public readonly string TAG_ACTION = "action";
        static public readonly string TAG_PARALLEL = "parallel";
        static public readonly string TAG_FEEDER = "feeder";

        static public readonly string ERROR_TRANSITION_ORDER = "S1001";
        static public readonly string ERROR_TRANSITION_DUPLICATED = "S1002";
        static public readonly string ERROR_EVENT_DOES_NOT_EXIST = "S1003";
        static public readonly string ERROR_NEXT_STATE_DOES_NOT_EXIST = "S1004";
        static public readonly string ERROR_COMPOSITE_AND_FINAL = "S1005";
        static public readonly string ERROR_XSD_VALIDATION = "S1006";
        static public readonly string ERROR_CONDITION_DUPLICATED = "S1007";
        static public readonly string ERROR_SYNCHRONOUS_CANNOT_HAVE_TIMERS = "S1008";
        static public readonly string ERROR_TIMER_DOES_NOT_EXIST = "S1009";
        static public readonly string ERROR_STATE_DUPLICATED = "S1010";
        static public readonly string ERROR_INTERNAL = "S1011";
        static public readonly string ERROR_FILE_NOT_FOUND = "S1012";
        static public readonly string ERROR_EVENT_INTERFACE_NOT_FOUND_IN_FILE = "S1013";
        static public readonly string ERROR_EVENT_INTERFACE_INVALID = "S1014";

        Dictionary<string, string> stateMap = new Dictionary<string, string>();

        public StateMachineXmlModel(string inputFileName)
        {
            InputFileName = inputFileName;
        }

        /// <summary>
        /// Buid the State Machine code.
        /// </summary>
        /// <returns></returns>
        public StateMachineType Build()
        {
            ValidateInputFile(InputFileName);
            StateMachineType model = DeserializeFile(InputFileName);

            FillDefaultSettings(model, InputFileName);

            // Post Process model to add state parent ad fill state map, fill the event and interface map
            model.FillModelPost();

            ValidateModel(InputFileName, model);
            return model;
        }

        /// <summary>
        /// Create and fill the State Machine object from its XML file.
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <returns></returns>
        protected StateMachineType DeserializeFile(string inputFileName)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "DeserializeFile:  {0}", inputFileName);
            XmlSerializer serializer = new XmlSerializer(typeof(StateMachineType));
            // TODO HEEFRE use "using()"
            TextReader reader = new StreamReader(inputFileName);
            StateMachineType fsm = (StateMachineType)serializer.Deserialize(reader);
            reader.Close();
            return fsm;
        }

        /// <summary>
        /// Validate the XML input file according to its XSD.
        /// Errors are reported through SchemaValidationEventHandler
        /// </summary>
        /// <param name="inputFileName"></param>
        protected void ValidateInputFile(string inputFileName)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateInputFile {0} against {1}", inputFileName, schemaName);
            Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StateForge.Resources." + schemaName);
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.Schemas.Add(null, XmlReader.Create(schemaStream));
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaValidationEventHandler);
            using (XmlReader reader = XmlReader.Create(inputFileName, settings))
            {
                while (reader.Read())
                {

                }
            }
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateInputFile done");
        }

        /// <summary>
        /// Validates the model, it performs checks that the XSD validator cannot do.
        /// </summary>
        /// <param name="inputFileName">The input file name.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateModel(string inputFileName, StateMachineType model)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateModel {0}", model.settings.name);
            using (XmlReader reader = XmlReader.Create(inputFileName))
            {
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == TAG_STATE)
                            {
                                ValidateState(reader, model);
                            }
                            else if (reader.Name == TAG_EVENT_LIST)
                            {
                                ValidateEventList(reader, model);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the event list.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateEventList(XmlReader reader, StateMachineType model)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEventList");
            using (XmlReader subReader = reader.ReadSubtree())
            {
                while (subReader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == TAG_EVENT_PROVIDER)
                            {
                                ValidateEventProvider(subReader, model);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the eventSource tag.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateEventProvider(XmlReader reader, StateMachineType model)
        {
            string name = reader[TAG_NAME];
            string file = reader[TAG_EVENT_FILE];
            string feeder = reader[TAG_FEEDER];
            ts.TraceEvent(TraceEventType.Verbose, 1,
                "ValidateEventProvider {0}, feeder {1}",
                name == null ? "" : name,
                file == null ? "" : file,
                feeder == null ? "" : feeder);

            //TODO check if file exist
            using (XmlReader subReader = reader.ReadSubtree())
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == TAG_EVENT_FILE)
                    {
                        ParseEventInterface(reader, model, name, file, feeder);
                    }
                }

                while (subReader.Read())
                {
                    switch (subReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (subReader.Name == TAG_TIMER)
                            {
                                ValidateTimer(subReader, model);
                            }
                            else if (subReader.Name == TAG_EVENT)
                            {
                                ValidateEvent(subReader, model, feeder);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the timer tag.
        /// Synchronous machine cannot have this tag.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateTimer(XmlReader reader, StateMachineType model)
        {
            string timerName = reader[TAG_NAME];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTimer {0}", timerName);
            if (model.settings.asynchronous == false)
            {
                string error = "Synchronous state machine cannot handle timers, set asynchronous=\"true\" or remove this timer element.";
                ReportError(reader, ERROR_SYNCHRONOUS_CANNOT_HAVE_TIMERS, error);
            }
        }

        /// <summary>
        /// Validate the event tag.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateEvent(XmlReader reader, StateMachineType model, string feeder)
        {
            string eventName = reader[TAG_NAME];
            string eventId = reader[TAG_EVENT_ID];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEvent name {0},  id {1}", eventName, eventId);
            model.AddEventToFeeder(feeder, eventId);
        }

        /// <summary>
        /// Validate the state element. This is a recursive function.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateState(XmlReader reader, StateMachineType model)
        {
            string stateName = reader[TAG_STATE_NAME];
            StateType state = model.GetStateType(stateName);
            if (state == null)
            {
                string error = "Cannot find state " + stateName;
                ReportError(reader, ERROR_INTERNAL, error);
                return;
            }

            if (this.stateMap.ContainsKey(stateName) == true)
            {
                string error = "Duplicated state " + stateName;
                ReportError(reader, ERROR_STATE_DUPLICATED, error);
                return;
            }
            else
            {
                this.stateMap[stateName] = stateName;
            }

            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateState {0}", stateName);

            using (XmlReader subReader = reader.ReadSubtree())
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == TAG_KIND)
                    {
                        ValidateStateKind(reader, model, state);
                    }
                }

                while (subReader.Read())
                {
                    switch (subReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (subReader.Name == TAG_STATE)
                            {
                                ValidateState(subReader, model);
                            }
                            else if (reader.Name == TAG_TRANSITION)
                            {
                                ValidateTransition(subReader, model, state);
                            }
                            else if (reader.Name == TAG_ONENTRY)
                            {
                                ValidateEntryExit(subReader, model, state);
                            }
                            else if (reader.Name == TAG_ONEXIT)
                            {
                                ValidateEntryExit(subReader, model, state);
                            }
                            else if (reader.Name == TAG_PARALLEL)
                            {
                                ValidateParallel(subReader, model, state);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate a parallel region.
        /// </summary>
        /// <param name="subReader"></param>
        /// <param name="model"></param>
        /// <param name="state"></param>
        private void ValidateParallel(XmlReader reader, StateMachineType model, StateType state)
        {
            string name = reader[TAG_NAME];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateParallel name {0}", name);

            using (XmlReader subReader = reader.ReadSubtree())
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == TAG_NEXT_STATE)
                    {
                        ValidateTransitionNextState(reader, model);
                    }
                }

                while (subReader.Read())
                {
                    switch (subReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateParallel element {0}", subReader.Name);
                            if (subReader.Name == TAG_STATE)
                            {
                                ValidateState(subReader, model);
                            }

                            break;

                        case XmlNodeType.Attribute:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateParallel attribute {0}", subReader.Name);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the onEntry and onEntry tag.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="state"></param>
        private void ValidateEntryExit(XmlReader reader, StateMachineType model, StateType state)
        {
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEntryExit state {0}", state.name);
            using (XmlReader subReader = reader.ReadSubtree())
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == TAG_ACTION)
                    {
                        ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEntryExit state {0} action {1}", state.name, reader.Value);
                    }
                }

                while (subReader.Read())
                {
                    switch (subReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEntryExit element {0}", subReader.Name);
                            if (subReader.Name == TAG_ACTION)
                            {
                            }
                            else if (subReader.Name == TAG_TIMER_START)
                            {
                                ValidateTimerStart(subReader, model);
                            }
                            else if (subReader.Name == TAG_TIMER_STOP)
                            {
                                ValidateTimerStop(subReader, model);
                            }
                            break;

                        case XmlNodeType.Attribute:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateEntryExit attribute {0}", subReader.Name);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the attribute state@kind
        /// Composite and final flags are imcompatible.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateStateKind(XmlReader reader, StateMachineType model, StateType state)
        {
            string kind = reader.Value;
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateStateKind state {0}, kind {1}", state.name, kind);
            if (state.Type.HasFlag(StateType.TypeFlags.FINAL) && state.Type.HasFlag(StateType.TypeFlags.COMPOSITE))
            {
                string error = "State " + state.name + " is a composite state which cannot have the final or the error attribute";
                ReportError(reader, ERROR_COMPOSITE_AND_FINAL, error);
            }
        }

        /// <summary>
        /// Validate a transition such as the nextState and the event attribute.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="model">The state for this transition.</param>
        private void ValidateTransition(XmlReader reader, StateMachineType model, StateType state)
        {
            string eventName = reader[TAG_EVENT];
            string conditionAttribute = reader[TAG_CONDITION];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransition state {0} event {1}",
                state.name, eventName);

            using (XmlReader subReader = reader.ReadSubtree())
            {
                List<TransitionType> transitions = model.GetTransitionList(state, eventName);

                CheckDuplicatedTransition(reader, model, transitions);
                CheckTransitionOrder(reader, model, transitions);

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == TAG_EVENT)
                    {
                        ValidateTransitionEvent(reader, model);
                    }
                    else if (reader.Name == TAG_NEXT_STATE)
                    {
                        ValidateTransitionNextState(reader, model);
                    }
                    else if (reader.Name == TAG_CONDITION)
                    {
                        ValidateTransitionAttributeCondition(reader, model, state);
                    }
                }

                while (subReader.Read())
                {
                    switch (subReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransition element {0}", reader.Name);
                            if (subReader.Name == TAG_CONDITION)
                            {
                                TransitionType transition = model.GetTransition(state, eventName, reader.ReadString());
                                ValidateTransitionElementCondition(subReader, model, transition, conditionAttribute);
                            }
                            else if (subReader.Name == TAG_TIMER_START)
                            {
                                ValidateTimerStart(subReader, model);
                            }
                            else if (subReader.Name == TAG_TIMER_STOP)
                            {
                                ValidateTimerStop(subReader, model);
                            }
                            break;

                        case XmlNodeType.Attribute:
                            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransition attribute {0}", reader.Name);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validate the timerStart tag, the attribute timerStart@timer must exist in events
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateTimerStart(XmlReader reader, StateMachineType model)
        {
            string timerName = reader[TAG_TIMER];
            string duration = reader[TAG_DURATION];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTimerStart {0}, duration {1}", timerName, duration);

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == TAG_TIMER)
                {
                    if (model.GetTimerFromName(timerName) == null)
                    {
                        string error = "Timer " + timerName + " does not exist";
                        ReportError(reader, ERROR_TIMER_DOES_NOT_EXIST, error);
                    }
                }
            }
        }

        /// <summary>
        /// Validate the timerStop tag, the attribute timerStop@timer must exist in events
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateTimerStop(XmlReader reader, StateMachineType model)
        {
            string timerName = reader[TAG_TIMER];
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTimerStop {0}", timerName);
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == TAG_TIMER)
                {
                    if (model.GetTimerFromName(timerName) == null)
                    {
                        string error = "Timer " + timerName + " does not exist";
                        ReportError(reader, ERROR_TIMER_DOES_NOT_EXIST, error);
                    }
                }
            }
        }

        /// <summary>
        /// Check for duplicated condition, condition may have either the attribute or the element but not both.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="transition"></param>
        private void ValidateTransitionElementCondition(
            XmlReader reader,
            StateMachineType model,
            TransitionType transition,
            string conditionAttribute)
        {
            if (conditionAttribute != null)
            {
                string error = "Transition " + model.GetTransitionName(transition) + " have condition as an attribute and as an element, remove one of them";
                ReportError(reader, ERROR_CONDITION_DUPLICATED, error);
            }
        }

        /// <summary>
        /// Transition without condition must be the last one.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="transitions"></param>
        private void CheckTransitionOrder(XmlReader reader, StateMachineType model, List<TransitionType> transitions)
        {
            if (transitions.Count >= 2)
            {
                for (int i = 0; i < transitions.Count - 1; i++)
                {
                    if (string.IsNullOrEmpty(model.GetCondition(transitions[i])) == true)
                    {
                        string error = "Transition " + model.GetTransitionName(transitions[i]) + " does not have a condition. Transition without condition must be last and unique";
                        ReportError(reader, ERROR_TRANSITION_ORDER, error);
                    }
                }
            }
        }



        /// <summary>
        /// Check for duplicated transition.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="transitions"></param>
        private void CheckDuplicatedTransition(XmlReader reader, StateMachineType model, List<TransitionType> transitions)
        {
            var TransitionIdList = new List<string>();

            foreach (TransitionType transition in transitions)
            {
                string transitionId = model.GetTransitionName(transition);
                if (TransitionIdList.IndexOf(transitionId) < 0)
                {
                    TransitionIdList.Add(transitionId);
                }
                else
                {
                    string error = "Duplicated transitions " + transitionId + ", remove one transition";
                    ReportError(reader, ERROR_TRANSITION_DUPLICATED, error);
                }
            }
        }

        /// <summary>
        /// Check that the attribute transition@event is a valid event.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateTransitionEvent(XmlReader reader, StateMachineType model)
        {
            string eventId = reader.Value;
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransitionEvent event {0}", eventId);
            EventType evt = model.GetEventFromId(eventId);
            if (evt == null)
            {
                string error = "Event " + eventId + " does not exist";
                ReportError(reader, ERROR_EVENT_DOES_NOT_EXIST, error);
            }
        }

        /// <summary>
        /// Check that the attribute transition@nextState is a valid state.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        private void ValidateTransitionNextState(XmlReader reader, StateMachineType model)
        {
            string nextStateName = reader.Value;
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransitionNextState next state {0}", nextStateName);
            StateType nextState = model.GetStateType(nextStateName);
            if (nextState == null)
            {
                string error = "Next state " + nextStateName + " does not exist";
                ReportError(reader, ERROR_NEXT_STATE_DOES_NOT_EXIST, error);
            }
        }

        /// <summary>
        /// Check that the transition with an attribute condition is a valid transition.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model">The State Machine Model</param>
        /// <param name="state">The state for this transition.</param>
        private void ValidateTransitionAttributeCondition(XmlReader reader, StateMachineType model, StateType state)
        {
            IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
            int lineNumber = xmlInfo.LineNumber;
            int linePosition = xmlInfo.LinePosition;
            string condition = reader.Value;
            ts.TraceEvent(TraceEventType.Verbose, 1, "ValidateTransitionEvent ({0},{1}) condition {2}", lineNumber, linePosition, condition);
        }

        /// <summary>
        /// Fill the default settings. 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="inputFileName"></param>
        private void FillDefaultSettings(StateMachineType model, string inputFileName)
        {
            if (String.IsNullOrEmpty(model.settings.name) == true)
            {
                model.settings.name = Path.GetFileNameWithoutExtension(inputFileName);
            }

            if (model.settings.@using == null)
            {
                model.settings.@using = new string[0];
            }

            FillDefaultContext(model);
        }

        /// <summary>
        /// Fill set default context class name and instance name.
        /// </summary>
        /// <param name="model"></param>
        private void FillDefaultContext(StateMachineType model)
        {
            ContextType context = model.settings.context;
            if (context == null)
            {
                context = new ContextType();
                model.settings.context = context;
            }
            if (String.IsNullOrEmpty(context.instance))
            {
                context.instance = "context";
            }
            if (String.IsNullOrEmpty(context.@class))
            {
                context.@class = model.settings.name + "Context";
            }
        }

        /// <summary>
        /// This callback is invoked by XmlReader when a validation error occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SchemaValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Exception != null)
            {
                ReportError(e.Exception.SourceUri, e.Exception.LineNumber, e.Exception.LinePosition, ERROR_XSD_VALIDATION, e.Message);
            }
        }

        /// <summary>
        /// Report the error to the trace system to the console and throw an exception.
        /// </summary>
        /// <param name="baseURI">The source file name URI.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="error">The error message</param>
        private void ReportError(string baseURI, int lineNumber, int linePosition, string errorCode, string error)
        {
            string message = "{0}({1},{2}) : error " + errorCode + ": {3}";
            ts.TraceEvent(TraceEventType.Error, 1, message,
                          baseURI, lineNumber, linePosition, error);
            Console.WriteLine(message,
                System.IO.Path.GetFileName(this.InputFileName), lineNumber, linePosition, error);
            throw new XmlException(error, null, lineNumber, linePosition);
        }

        /// <summary>
        ///  Report the error to the trace system to the console and throw an exception.
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="error">The error message</param>
        private void ReportError(XmlReader reader, string errorCode, string error)
        {
            IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
            int lineNumber = xmlInfo.LineNumber;
            int linePosition = xmlInfo.LinePosition;
            ReportError(reader.BaseURI, xmlInfo.LineNumber, xmlInfo.LinePosition, errorCode, error);
        }

        /// <summary>
        /// Parse an event interface
        /// </summary>
        /// <param name="reader">The Xml reader.</param>
        /// <param name="model"></param>
        /// <param name="interfaceName">The interface name.</param>
        /// <param name="file">The filename where is the interface.</param>
        /// <param name="feeder">The class that feeds the events</param>
        private void ParseEventInterface(XmlReader reader, StateMachineType model, string interfaceName, string file, string feeder)
        {
            if (string.IsNullOrEmpty(file) == true)
            {
                return;
            }

            string inputDirectory = Path.GetDirectoryName(this.InputFileName);
            string interfaceFileName = inputDirectory + Path.DirectorySeparatorChar + file;

            try
            {
                StreamReader textReader = new StreamReader(interfaceFileName);
                IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, textReader);
                parser.Parse();
                if (parser.Errors.Count > 0)
                {
                    string error = "Error parsing interface " + interfaceFileName + ": " + parser.Errors.ErrorOutput;
                    ReportError(reader, ERROR_EVENT_INTERFACE_INVALID, error);
                    return;
                }

                CompilationUnit cu = parser.CompilationUnit;
                var visitorEvent = new VisitorEventInterface(model, interfaceName, feeder);
                cu.AcceptVisitor(visitorEvent, null);
                if (visitorEvent.InterfaceFound == false)
                {
                    string error = "Interface " + interfaceName + " not found in file " + interfaceFileName;
                    ReportError(reader, ERROR_EVENT_INTERFACE_NOT_FOUND_IN_FILE, error);
                }
            }
            catch (IOException iOException)
            {
                string error = "IOException for file " + interfaceFileName + ": " + iOException.Message;
                ReportError(reader, ERROR_FILE_NOT_FOUND, error);
            }
        }
    }
}
