
namespace StateForge.Examples.Seminar
{
    using System;
    using NUnit.Framework;
    using StateForge.StateMachine;
    using Rhino.Mocks;

    [TestFixture]
    public class SeminarTest
    {
        private SeminarContext context;  
        private SeminarActuator actuator;
        private MockRepository repository;
        private IObserver observer;
        private static String contextName = "SeminarContext";
        private string current, next, transition;

        private Student alice, beatrice, carla;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            this.actuator = new SeminarActuator();
            observer = repository.StrictMock<IObserver>();
            this.context = new SeminarContext(this.actuator);
            this.context.Observer = observer;

            this.alice = new Student() { Name = "Alice" };
            this.beatrice = new Student() { Name = "Beatrice" };
            this.carla = new Student() { Name = "Carla" };
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Start()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    Expect.Call(() => this.observer.OnEntry(contextName, "Seminar"));
                    Expect.Call(() => this.observer.OnEntry(contextName, "Enrollment"));
                    Expect.Call(() => this.observer.OnEntry(contextName, "OpenForEnrollment"));
                }
            }

            using (repository.Playback())
            {
                this.context.EnterInitialState();
            }
        }

        [Test]
        public void StandardEnrollment()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvStudentEnrollRequest(alice);
                    current = "OpenForEnrollment"; next = "OpenForEnrollment"; transition = "StudentEnrollRequest";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvTermStarted();
                    current = "OpenForEnrollment"; next = "BeingTaught"; transition = "TermStarted";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Enrollment"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvClassesEnded();
                    current = "BeingTaught"; next = "FinalExams"; transition = "ClassesEnded";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvExamsClosed();
                    current = "FinalExams"; next = "End"; transition = "ExamsClosed";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                }
            }

            using (repository.Playback())
            {
                this.context.StudentEnrollRequest(this.alice);

                this.context.TermStarted();

                this.context.ClassesEnded();

                this.context.ExamsClosed();
            }
        }

        [Test]
        public void DisastrousEnrollment()
        {
            using (repository.Record())
            {
                using (repository.Ordered())
                {
                    // this.context.EvStudentEnrollRequest(alice);
                    current = "OpenForEnrollment"; next = "OpenForEnrollment"; transition = "StudentEnrollRequest";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvStudentEnrollRequest(beatrice);
                    current = "OpenForEnrollment"; next = "Full"; transition = "StudentEnrollRequest[(mySeminar.StudentsCount + 1) == mySeminar.MaxStudents]";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvStudentEnrollRequest(alice);
                    current = "Full"; next = "Full"; transition = "StudentEnrollRequest";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvStudentDropped(this.beatrice);
                    current = "Full"; next = "Full"; transition = "StudentDropped[mySeminar.HasStudentInWaitingList()]";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvStudentDropped(this.alice);
                    current = "Full"; next = "OpenForEnrollment"; transition = "StudentDropped";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvTermStarted();
                    current = "OpenForEnrollment"; next = "BeingTaught"; transition = "TermStarted";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnExit(contextName, "Enrollment"));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                    // this.context.EvStudentDropped(this.carla);
                    current = "BeingTaught"; next = "End"; transition = "StudentDropped[mySeminar.IsEmpty()]";
                    Expect.Call(() => this.observer.OnTransitionBegin(contextName, current, next, transition));
                    Expect.Call(() => this.observer.OnExit(contextName, current));
                    Expect.Call(() => this.observer.OnEntry(contextName, next));
                    Expect.Call(() => this.observer.OnTransitionEnd(contextName, current, next, transition));

                }
            }

            using (repository.Playback())
            {
                this.context.StudentEnrollRequest(this.alice);
                this.context.StudentEnrollRequest(this.beatrice);
                this.context.StudentEnrollRequest(this.carla);

                this.context.StudentDropped(this.beatrice);
                this.context.StudentDropped(this.alice);

                this.context.TermStarted();
                this.context.StudentDropped(this.carla);


            }
        }

       
    }
}
