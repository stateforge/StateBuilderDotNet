#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateBuilder.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    #region Using Directives
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using CommandLine;
    using CommandLine.Text;
    using System.Collections;
    using System.Reflection;
    using System.Xml;
    using System.Windows.Forms;
    using System.Collections.Specialized;
    #endregion

    public partial class StateBuilderGui : System.Windows.Forms.Form
    {
        static TraceSource ts = new TraceSource("StateBuilder");
        private static readonly HeadingInfo headingInfo = new HeadingInfo(Application.ProductName, Application.ProductVersion);
        

        private StateBuilder stateBuilder;
        private ReturnCode errorCode = ReturnCode.Ok;
        private Options options = new Options();
        private string CurrentFsmFileName { get; set; }

        public enum ReturnCode
        {
            Ok = 0,
            InvalidCommandLine = -1,
            InputFileNotFound = -2,
            IOException = -3,
            XmlException = -4,
            UnauthorizedAccessException = -5,
            UserQuit = -6,
            Error = -7,
        }

        StateBuilderGui()
        {
            
            InitializeComponent();
            InitializeMostRecentUsedList();

            if (string.IsNullOrEmpty(Settings.Default.PrependFile) == false)
            {
                textBoxPrependFile.Text = Settings.Default.PrependFile;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ts.TraceInformation("{0} version {1} by {2}", Application.ProductName, Application.ProductVersion, Application.CompanyName);

            Application.EnableVisualStyles();

            

            var gui = new StateBuilderGui();

            gui.ParseArgs(args);

            if (gui.options.batchMode == true)
            {
                gui.errorCode = gui.Generate();
            }
            else
            {
                gui.UpdateGuiFromOption();
                Application.Run(gui);
            }

            System.Diagnostics.Debug.Flush();
            Environment.Exit((int)gui.errorCode);
        }

        private bool ParseArgs(string[] args)
        {
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Error parsing command line: {0}", Environment.CommandLine);
                errorCode = ReturnCode.InvalidCommandLine;
                return false;
            }

            // Bach mode
            if (options.batchMode == true)
            {
                ts.TraceEvent(TraceEventType.Verbose, 1, "Run in batch mode");
            }

            return true;
        }

        private void UpdateGuiFromOption()
        {
            // Input file
            if (options.InputStateMachineFiles.Count > 0)
            {
                CurrentFsmFileName = options.InputStateMachineFiles[0];
                UpdateMostRecentUsedList(CurrentFsmFileName);
                FillComboxBoxInputFilename();
                ts.TraceEvent(TraceEventType.Verbose, 1, "Input file: {0}", CurrentFsmFileName);
            }

            // Output directory
            if (string.IsNullOrEmpty(options.outputDirectory) == false)
            {
                textBoxOutputDirectory.Text = options.outputDirectory;
                ts.TraceEvent(TraceEventType.Verbose, 1, "Output directory: {0}", textBoxOutputDirectory.Text);
            }
        }

        private sealed class Options
        {
            #region Option Attribute

            [Option("b", "batch mode",
                    Required = false,
                    HelpText = "Run in batch mode")]
            public bool batchMode = false;

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
                var help = new HelpText(StateBuilderGui.headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo(Application.CompanyName, 2010);
                //help.AddPreOptionsLine("This is NOT free software.");
                help.AddPreOptionsLine("Usage: " + Application.ProductName + " myFsm.fsmcs");
                help.AddOptions(this);

                return help;
            }
            #endregion
        }




        private StateBuilder CreateBuilderFromOption(Options options)
        {
            StateBuilder stateBuilder = new StateBuilder(options.InputStateMachineFiles[0]);
            // Output directory
            if (string.IsNullOrEmpty(options.outputDirectory) == false)
            {
                stateBuilder.OutputDirectory = options.outputDirectory;
            }
            return stateBuilder;
        }


        public ReturnCode Generate()
        {
            try
            {
                this.stateBuilder = CreateBuilderFromOption(this.options);
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

        /// <summary>
        /// Handle a click on the about form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm(this);
            aboutForm.Show();
        }

      

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string helpUrl = "http://www.stateforge.com/Help/StateBuilderDotNet/statebuilderdotnet-state-machine-generator.aspx";
            System.Diagnostics.Process.Start(helpUrl);
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open an existing finite state machine.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        /// 
        private void openFsmFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open a finite state machine in XML format";
                dlg.Filter = "c# fsm (*.fsmcs)|*.fsmcs|vb.net fsm (*.fsmvb)|*.fsmvb|All files (*.*)|*.*";
                dlg.DefaultExt = "fsmcs";
                dlg.InitialDirectory = Directory.GetCurrentDirectory();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        CurrentFsmFileName = dlg.FileName;
                        UpdateMostRecentUsedList(CurrentFsmFileName);
                        FillComboxBoxInputFilename();
                        this.toolStripStatusLabel.Text = "Click on \"Generate Code\" to build to Finite State Machine";
                    }

                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex);
                        MessageBox.Show(ex.Message, Application.ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }

        }

        

        /// <summary>
        /// Generate the code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGenerateCode_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(CurrentFsmFileName))
            {
                openFsmFile_Click(sender, e);
            }

            if (File.Exists(CurrentFsmFileName) == false)
            {
                MessageBox.Show("The file " + CurrentFsmFileName + " does not exist", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateMostRecentUsedList(CurrentFsmFileName);
            FillComboxBoxInputFilename();

            stateBuilder = new StateBuilder(CurrentFsmFileName);

            try
            {
                this.toolStripStatusLabel.Text = "Generating code";
                // Prepend Copyright
                if (string.IsNullOrEmpty(textBoxPrependFile.Text) == false)
                {
                    stateBuilder.Options.PrependFile = textBoxPrependFile.Text;
                }

                // NoObserver
                stateBuilder.Options.NoObserver = checkBoxNoObserver.Checked;

                // Output directory
                if (String.IsNullOrEmpty(textBoxOutputDirectory.Text) == false)
                {
                    stateBuilder.OutputDirectory = textBoxOutputDirectory.Text;
                }

                // Build it now
                stateBuilder.Build();
                this.toolStripStatusLabel.Text = "Code successfully generated";
                errorCode = ReturnCode.Ok;
            }
            catch (IOException ioException)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "IOException: {0}", ioException.Message);
                MessageBox.Show("IOException in file " + stateBuilder.InputFileName + ": " + ioException.Message, Application.ProductName,
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorCode = ReturnCode.IOException;
            }
            catch (XmlException xmlException)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "XmlException: {0}", xmlException.Message);
                ts.TraceEvent(TraceEventType.Error, 1, "Line: ({0},{0})", xmlException.LineNumber, xmlException.LinePosition);
                MessageBox.Show("XmlException in file " + stateBuilder.InputFileName + ": " + xmlException.Message, Application.ProductName,
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorCode = ReturnCode.XmlException;
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                ts.TraceEvent(TraceEventType.Error, 1, "Unauthorized Access Exception: {0}", unauthorizedAccessException.Message);
                MessageBox.Show("Unauthorized Access Exception for file " + stateBuilder.InputFileName + ": " + unauthorizedAccessException.Message, Application.ProductName,
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Exception while processing file " + stateBuilder.InputFileName + ": " + exception.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorCode = ReturnCode.Error;
            }

            if (errorCode != ReturnCode.Ok)
            {
                this.toolStripStatusLabel.Text = "Error while generated code";
            }
        }

        /// <summary>
        /// Save changes on exit if necessary
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
        /// <summary>
        /// Initialize the most recent used list
        /// </summary>
        private void InitializeMostRecentUsedList()
        {
            StringCollection mruList = Settings.Default.MruList;
            // Create the MRU on first use
            if (mruList == null)
            {
                Settings.Default.MruList = new StringCollection();
            }
            else
            {
                // Get rid of files that no longer exist
                for (int i = 0; i < mruList.Count; i++)
                {
                    if (!File.Exists(mruList[i]))
                    {
                        mruList.RemoveAt(i);
                    }
                }
                FillComboxBoxInputFilename();
            }
        }
        /// <summary>
        /// Update the MRU list with the specified project filename by
        /// adding it to the list or making it the most recently used.
        /// </summary>
        protected void UpdateMostRecentUsedList(string fsmFile)
        {
            StringCollection mruList = Settings.Default.MruList;

            fsmFile = Path.GetFullPath(fsmFile);

            if (mruList.Contains(fsmFile))
                mruList.Remove(fsmFile);

            mruList.Insert(0, fsmFile);

            while (mruList.Count > 10)
                mruList.RemoveAt(9);
        }

        /// <summary>
        /// Fill the input file combox from the most recent used files
        /// </summary>
        private void FillComboxBoxInputFilename()
        {
            StringCollection mruList = Settings.Default.MruList;
            comboBoxInputFiles.Items.Clear();
            foreach (string file in mruList)
            {
                comboBoxInputFiles.Items.Add(file);
            }

            if (comboBoxInputFiles.Items.Count > 0)
            {
                comboBoxInputFiles.SelectedIndex = 0;
            }

            if (mruList.Count > 0)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(comboBoxInputFiles.SelectedItem.ToString()));
            }

        }

        /// <summary>
        /// Set the current input file name whem the combox index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxInputFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentFsmFileName = comboBoxInputFiles.SelectedItem.ToString();
            UpdateMostRecentUsedList(CurrentFsmFileName);
        }

        private void buttonBrowseOutputDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = Path.GetDirectoryName(CurrentFsmFileName);
                dlg.Description = "Select the Output Directory";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBoxOutputDirectory.Text = dlg.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Open a file dialog to choose the file to prepend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowsePrependFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Choose file to prepend to the generated code, i.e copyright";
                dlg.Filter = "All files (*.*)|*.*";
                dlg.InitialDirectory = Directory.GetCurrentDirectory();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBoxPrependFile.Text = dlg.FileName;
                    Settings.Default.PrependFile = dlg.FileName;
                }
            }
        }

        /// <summary>
        /// Clear the PrependFile settings when the prepend text box is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPrependFile_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPrependFile.Text.Length == 0)
            {
                Settings.Default.PrependFile = null;
            }
        }

    }
}
