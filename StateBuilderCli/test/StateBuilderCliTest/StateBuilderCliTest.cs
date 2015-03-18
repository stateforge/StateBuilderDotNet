#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateBuilderCliTest.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class StateBuilderCliTest
    {
        private StateBuilderCli app;
        private string[] args;
        private string tmpPath = Path.GetTempPath();
        private string inputFile = "Samples/HelloWorld.fsmcs";
        private string inputFileNotWellFormed = "Samples/NotWellFormed.fsmcs";
        private string inputFileNotWellFormed02 = "Samples/NotWellFormed02.fsmcs";
        private string nextStateDoesNotExistFile = "Samples/NextStateDoesNotExist.fsmcs";
        private string eventDoesNotExistFile = "Samples/EventDoesNotExist.fsmcs";
        private string errorStateIsNotLeaf = "Samples/ErrorStateIsNotLeaf.fsmcs";
        private string finalStateIsNotLeaf = "Samples/FinalStateIsNotLeaf.fsmcs";
        private string guardConditionOrder01 = "Samples/GuardConditionOrder01.fsmcs";
        private string guardConditionOrder02 = "Samples/GuardConditionOrder02.fsmcs";
        private string duplicatedTransition01 = "Samples/DuplicatedTransition01.fsmcs";
        private string duplicatedTransition02 = "Samples/DuplicatedTransition02.fsmcs";
        private string duplicatedTransition03 = "Samples/DuplicatedTransition03.fsmcs";
        private string duplicatedCondition = "Samples/DuplicatedCondition.fsmcs";
        private string syncAndTimers = "Samples/SyncAndTimers.fsmcs";
        private string timerStartDoNotExist = "Samples/TimerStartDoNotExist.fsmcs";
        private string timerStopDoNotExist = "Samples/TimerStopDoNotExist.fsmcs";
        private string timerStartDoNotExist02 = "Samples/TimerStartDoNotExist02.fsmcs";
        private string timerStopDoNotExist02 = "Samples/TimerStopDoNotExist02.fsmcs";
        private string timerStartDoNotExist03 = "Samples/TimerStartDoNotExist03.fsmcs";
        private string timerStopDoNotExist03 = "Samples/TimerStopDoNotExist03.fsmcs";
        private string duplicatedState = "Samples/DuplicatedState.fsmcs";
        private string eventInterfaceNotFound = "Samples/EventInterfaceNotFound.fsmcs";
        private string eventInterfaceNotFoundInFile = "Samples/EventInterfaceNotFoundInFile.fsmcs";
        private string eventInterfaceInvalid = "Samples/EventInterfaceInvalid.fsmcs";


        [SetUp]
        public void Setup()
        {
            app = new StateBuilderCli();
        }

        [Test]
        public void Ok()
        {
            args = new string[] { inputFile };
            string generatedFile = Path.Combine(Path.GetDirectoryName(inputFile), "HelloWorldFsm.cs");
            File.Delete(generatedFile);
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.Ok);
            Assert.IsTrue(File.Exists(generatedFile));
        }

        [Test]
        public void NoArgument()
        {
            args = new string[] { };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.InvalidCommandLine);
        }

        [Test]
        public void InputFileNotFound()
        {
            args = new string[] { "NotExistingFile.fsmcs" };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.IOException);
        }

        [Test]
        public void InvalidOption()
        {
            args = new string[] { "--NotExistingFileOption " };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.InvalidCommandLine);
        }

        [Test]
        public void OkOutputDirectory()
        {
            args = new string[] { "-d", tmpPath, inputFile };
            string generatedFile = Path.Combine(tmpPath, "HelloWorldFsm.cs");
            File.Delete(generatedFile);
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.Ok);
            Assert.IsTrue(File.Exists(generatedFile));
        }

        [Test]
        public void KoInvalidOutputDirectory()
        {
            args = new string[] { "-d", "InvalidDirectory", inputFile };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.IOException);
        }

        [Test]
        public void CannotWriteOutput()
        {
            args = new string[] { "-d", "c:/", inputFile };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.UnauthorizedAccessException);
        }

        [Test]
        public void NotWellFormed()
        {
            args = new string[] { inputFileNotWellFormed };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void NotWellFormed02()
        {
            args = new string[] { inputFileNotWellFormed02 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }
        [Test, Category("VSErrorWindow")]
        public void NextStateDoesNotExist()
        {
            args = new string[] { nextStateDoesNotExistFile };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void EventDoesNotExist()
        {
            args = new string[] { eventDoesNotExistFile };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void ErrorStateIsNotLeaf()
        {
            args = new string[] { errorStateIsNotLeaf };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void FinalStateIsNotLeaf()
        {
            args = new string[] { finalStateIsNotLeaf };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void GuardConditionOrder01()
        {
            args = new string[] { guardConditionOrder01 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void GuardConditionOrder02()
        {
            args = new string[] { guardConditionOrder02 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void DuplicatedTransition01()
        {
            args = new string[] { duplicatedTransition01 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void DuplicatedTransition02()
        {
            args = new string[] { duplicatedTransition02 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void DuplicatedTransition03()
        {
            args = new string[] { duplicatedTransition03 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void DuplicatedCondition()
        {
            args = new string[] { duplicatedCondition };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void SyncAndTimers()
        {
            args = new string[] { syncAndTimers };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStartDoNotExist()
        {
            args = new string[] { timerStartDoNotExist };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStartDoNotExist02()
        {
            args = new string[] { timerStartDoNotExist02 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStartDoNotExist03()
        {
            args = new string[] { timerStartDoNotExist03 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStopDoNotExist()
        {
            args = new string[] { timerStopDoNotExist };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStopDoNotExist02()
        {
            args = new string[] { timerStopDoNotExist02 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void TimerStopDoNotExist03()
        {
            args = new string[] { timerStopDoNotExist03 };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void DuplicatedState()
        {
            args = new string[] { duplicatedState };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void EventInterfaceNotFound()
        {
            args = new string[] { eventInterfaceNotFound };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void EventInterfaceNotFoundInFile()
        {
            args = new string[] { eventInterfaceNotFoundInFile };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }

        [Test, Category("VSErrorWindow")]
        public void EventInterfaceInvalid()
        {
            args = new string[] { eventInterfaceInvalid };
            StateBuilderCli.ReturnCode errorCode = app.run(args);
            Assert.AreEqual(errorCode, StateBuilderCli.ReturnCode.XmlException);
        }
    }
}
