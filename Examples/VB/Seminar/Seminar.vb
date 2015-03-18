Namespace StateForge.Examples.Seminar

	Public Class Student
		Public Property Name() As String
	End Class

	''' <summary>
	''' SeminarActuator performs the actions
	''' </summary>
	Public Class SeminarActuator
		Public studentListWaiting As New List(Of Student)()
		Public studentList As New List(Of Student)()
		Public ReadOnly Property StudentsCount() As Integer
			Get
				Return Me.studentList.Count
			End Get
		End Property

		Public Property MaxStudents() As Integer

		Public Sub New()
			MaxStudents = 2
		End Sub

		Public Function IsEmpty() As Boolean
			Return studentList.Count = 0
		End Function

		Public Function HasStudentInWaitingList() As Boolean
			Return studentListWaiting.Count > 0
		End Function


		Public Overridable Sub AddStudentInWaitingList(ByVal student_Renamed As Student)
			Console.WriteLine("AddStudentInWaitingList {0}", student_Renamed.Name)
			studentListWaiting.Add(student_Renamed)
		End Sub

		Public Overridable Sub ConfirmStudentEnrollment(ByVal student_Renamed As Student)
			Console.WriteLine("ConfirmStudentEnrollment {0}", student_Renamed.Name)
			studentList.Add(student_Renamed)
			studentListWaiting.Remove(student_Renamed)
		End Sub

		Public Overridable Sub InformStudentInWaitingList()
			Console.WriteLine("InformStudentInWaitingList")
			If studentListWaiting.Count > 0 Then
				Dim student_Renamed As Student = studentListWaiting(0)
				ConfirmStudentEnrollment(student_Renamed)
			End If
		End Sub

		Public Overridable Sub RemoveConfirmedStudent(ByVal student_Renamed As Student)
			Console.WriteLine("RemoveConfirmedStudent {0}", student_Renamed.Name)
			studentList.Remove(student_Renamed)
		End Sub

		Public Overridable Sub StartEnrollment()
			Console.WriteLine("StartEnrollment")
		End Sub

		Public Overridable Sub StopEnrollment()
			Console.WriteLine("StopEnrollment")
		End Sub

		Public Overridable Sub StartExams()
			Console.WriteLine("StartExams")
		End Sub

		Public Overridable Sub StartTeaching()
			Console.WriteLine("StartTeaching")
		End Sub

		Public Overridable Sub StopTeaching()
			Console.WriteLine("StopTeaching")
		End Sub
	End Class

	''' <summary>
	''' Seminar is a container class which hold instances of a SeminarActuator and a generated SeminarContext class.
	''' </summary>
	Partial Public Class Seminar
		Private actuator As SeminarActuator
		Private context As SeminarContext

		Public Sub New()
			Me.actuator = New SeminarActuator()
			Me.context = New SeminarContext(actuator)
			Me.context.EnterInitialState()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim mySeminar As New Seminar()

			Dim alice As New Student() With {.Name = "Alice"}
			Dim beatrice As New Student() With {.Name = "Beatrice"}
			Dim carla As New Student() With {.Name = "Carla"}

			mySeminar.StudentEnrollRequest(alice)
			mySeminar.StudentEnrollRequest(beatrice)
			mySeminar.StudentEnrollRequest(carla)

			mySeminar.StudentDropped(beatrice)
			mySeminar.StudentDropped(alice)

			mySeminar.TermStarted()
			mySeminar.StudentDropped(carla)

			mySeminar.ClassesEnded()
			mySeminar.ExamsClosed()


			Environment.Exit(0)
		End Sub
	End Class
End Namespace
