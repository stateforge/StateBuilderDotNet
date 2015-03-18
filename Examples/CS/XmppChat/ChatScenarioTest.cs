namespace StateForge.Examples.XmppChat
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using System.Diagnostics;
    using System.Configuration;

    [TestFixture]
    public class ChatScenarioTest
    {
        static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private static readonly string traceName = "Client";
        private static TraceSource ts = new TraceSource(traceName);

        Client alice, bob;
        ChatScenario chatScenario;
        XmppDb Db {get; set;}
        
        string server;
        string dbConnectionString;

        string username1 = "stateforge1";
        string password1 = "stateforge1";

        string username2 = "stateforge2";
        string password2 = "stateforge2";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.server = System.Configuration.ConfigurationManager.AppSettings["Server"];
            this.dbConnectionString = System.Configuration.ConfigurationManager.AppSettings["DbConnectionString"];

            this.Db = new XmppDb(this.dbConnectionString);
            this.Db.Open();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.Db.Close();
        }

        [SetUp]
        public void SetUp()
        {
            this.chatScenario = new ChatScenario();
            this.alice = this.chatScenario.CreateClient(username1, password1, server);
            this.bob = this.chatScenario.CreateClient(username2, password2, server);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void TestRegisterNewAccount()
        {
            this.Db.UserRemove(alice.Jid.User);
            var context = new TestRegisterNewAccountContext(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestScenarioLogin()
        {
            this.Db.UserAdd(alice.Jid.User, password1);
            var context = new TestLoginContext(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestScenarioLogin02()
        {
            this.Db.UserAdd(alice.Jid.User, password1);
            this.Db.UserAdd(bob.Jid.User, password2);
            var context = new TestLogin02Context(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestOpenClose01()
        {
            var context = new TestOpenClose01Context(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestOpenClose02()
        {
            var context = new TestOpenClose02Context(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestScenarioUnSubscribe()
        {
            var context = new TestUnSubscribeContext(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestRosterAddRemove()
        {
            var context = new TestRosterAddRemoveContext(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestRosterAdd()
        {
            var context = new TestRosterAddContext(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestSubscription01()
        {
            var context = new TestSubscription01Context(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        [Test]
        public void TestChat01()
        {
            var context = new TestChat01Context(this.chatScenario, this.alice, this.bob);
            InitializeContext(context);
            Start(context);
        }

        private void Start(IChatScenario context)
        {
            this.chatScenario.Context = context;
            this.chatScenario.Start();
            autoResetEvent.WaitOne();
            Assert.IsFalse(this.chatScenario.HasError());
        }

        static private void End(object sender, EventArgs e)
        {
            autoResetEvent.Set();
        }

        private void InitializeContext(ContextBase context)
        {
            context.Observer = ObserverTrace.Instance(traceName);
            context.RegisterEndHandler(new EventHandler<EventArgs>(End));
            context.EnterInitialState();
        }

        public ChatScenarioTest()
        {
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ChatScenarioTest chatScenarioTest = new ChatScenarioTest();
            chatScenarioTest.TestFixtureSetup();
            int numTest = 2;

            for (int i = 1; i <= numTest; i++)
            {
                Console.WriteLine("Session number " + i);

                Console.WriteLine("\tTestRegisterNewAccount");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestRegisterNewAccount();

                Console.WriteLine("\tTestScenarioLogin");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestScenarioLogin();

                Console.WriteLine("\tTestScenarioLogin02");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestScenarioLogin02();

                Console.WriteLine("\tTestOpenClose01");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestOpenClose01();

                Console.WriteLine("\tTestOpenClose02");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestOpenClose02();

                Console.WriteLine("\tTestRosterAddRemove");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestRosterAddRemove();

                Console.WriteLine("\tTestRosterAdd");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestRosterAdd();

                Console.WriteLine("\tTestSubscription01");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestSubscription01();

                Console.WriteLine("\tTestChat01");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestChat01();

                Console.WriteLine("\tTestScenarioUnSubscribe");
                chatScenarioTest.SetUp();
                chatScenarioTest.TestScenarioUnSubscribe();
            }

            chatScenarioTest.TestFixtureTearDown();

            Environment.Exit(0);
        }
    }
}
