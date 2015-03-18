

namespace StateForge.Examples.Seminar
{
    using System;
    using System.Collections.Generic;

    public class Student
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// SeminarActuator performs the actions
    /// </summary>
    public class SeminarActuator
    {
        public List<Student> studentListWaiting = new List<Student>();
        public List<Student> studentList = new List<Student>();
        public int StudentsCount
        {
            get
            {
                return this.studentList.Count;
            }
        }

        public int MaxStudents { get; set; }

        public SeminarActuator()
        {
            MaxStudents = 2;
        }

        public bool IsEmpty()
        {
            return studentList.Count == 0;
        }

        public bool HasStudentInWaitingList()
        {
            return studentListWaiting.Count > 0;
        }


        public virtual void AddStudentInWaitingList(Student student)
        {
            Console.WriteLine("AddStudentInWaitingList {0}", student.Name);
            studentListWaiting.Add(student);
        }

        public virtual void ConfirmStudentEnrollment(Student student)
        {
            Console.WriteLine("ConfirmStudentEnrollment {0}", student.Name);
            studentList.Add(student);
            studentListWaiting.Remove(student);
        }

        public virtual void InformStudentInWaitingList()
        {
            Console.WriteLine("InformStudentInWaitingList");
            if (studentListWaiting.Count > 0)
            {
                Student student = studentListWaiting[0];
                ConfirmStudentEnrollment(student);
            }
        }

        public virtual void RemoveConfirmedStudent(Student student)
        {
            Console.WriteLine("RemoveConfirmedStudent {0}", student.Name);
            studentList.Remove(student);
        }

        public virtual void StartEnrollment()
        {
            Console.WriteLine("StartEnrollment");
        }

        public virtual void StopEnrollment()
        {
            Console.WriteLine("StopEnrollment");
        }

        public virtual void StartExams()
        {
            Console.WriteLine("StartExams");
        }

        public virtual void StartTeaching()
        {
            Console.WriteLine("StartTeaching");
        }

        public virtual void StopTeaching()
        {
            Console.WriteLine("StopTeaching");
        }
    }

    /// <summary>
    /// Seminar is a container class which hold instances of a SeminarActuator and a generated SeminarContext class.
    /// </summary>
    public partial class Seminar
    {
        private SeminarActuator actuator;
        private SeminarContext context;

        public Seminar()
        {
            this.actuator = new SeminarActuator();
            this.context = new SeminarContext(actuator);
            this.context.EnterInitialState();
        }

        static void Main(string[] args)
        {
            Seminar mySeminar = new Seminar();

            Student alice = new Student() { Name = "Alice" };
            Student beatrice = new Student() { Name = "Beatrice" };
            Student carla = new Student() { Name = "Carla" };

            mySeminar.StudentEnrollRequest(alice);
            mySeminar.StudentEnrollRequest(beatrice);
            mySeminar.StudentEnrollRequest(carla);

            mySeminar.StudentDropped(beatrice);
            mySeminar.StudentDropped(alice);

            mySeminar.TermStarted();
            mySeminar.StudentDropped(carla);

            mySeminar.ClassesEnded();
            mySeminar.ExamsClosed();


            Environment.Exit(0);
        }
    }
}
