Imports System.Threading
Imports StateForge.StateMachine


Namespace StateForge.Examples.TrafficLight

	''' <summary>
	''' The Light class knows how to switch on and off the green, yellow and red lights.
	''' </summary>
	Public Class Light
		''' <summary>
		''' Red timer duration in milliseconds
		''' </summary>
		Public Property TimerRedDuration() As Double

		''' <summary>
		''' Yellow timer duration in milliseconds
		''' </summary>
		Public Property TimerYellowDuration() As Long

		''' <summary>
		''' Green timer duration in milliseconds
		''' </summary>
		Public Property TimerGreenDuration() As Long

		''' <summary>
		''' Maximum operating duration in milliseconds
		''' </summary>
		Public Property TimerMaxDuration() As Long

		Public Sub New()
			TimerRedDuration = 100 ' msec
			TimerYellowDuration = 100 ' msec
			TimerGreenDuration = 100 ' msec

			TimerMaxDuration = 390 ' msec
		End Sub

		Public Sub TurnOnGreen()
			Console.WriteLine("TurnOnGreen")
		End Sub

		Public Sub TurnOffGreen()
			Console.WriteLine("TurnOffGreen")
		End Sub

		Public Sub TurnOnYellow()
			Console.WriteLine("TurnOnYellow")
		End Sub

		Public Sub TurnOffYellow()
			Console.WriteLine("TurnOffYellow")
		End Sub
		Public Sub TurnOnRed()
			Console.WriteLine("TurnOnRed")
		End Sub

		Public Sub TurnOffRed()
			Console.WriteLine("TurnOffRed")
		End Sub
	End Class

	''' <summary>
	''' TrafficLight is a container class which hold instances of a TrafficLightActuator and a generated TrafficLightContext class.
	''' </summary>
	Partial Public Class TrafficLight
'INSTANT VB NOTE: The variable light was renamed since Visual Basic does not handle variables named the same as their type well:
		Private light_Renamed As Light
		Private context As TrafficLightContext
		Private Shared ReadOnly traceName As String = "TrafficLight"
		Private Shared ts As New TraceSource(traceName)
'INSTANT VB NOTE: The variable autoResetEvent was renamed since Visual Basic does not handle variables named the same as their type well:
		Private Shared autoResetEvent_Renamed As New AutoResetEvent(False)

		Public Sub New()
			Me.light_Renamed = New Light()
			Me.context = New TrafficLightContext(light_Renamed)
			Me.context.Observer = ObserverTrace.Instance(traceName)
			AddHandler context.EndHandler, AddressOf StateMachineEnd
			Me.context.EnterInitialState()
		End Sub

		Private Sub StateMachineEnd(ByVal sender As Object, ByVal e As EventArgs)
			ts.TraceInformation("state machine has ended")
			autoResetEvent_Renamed.Set()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim myTrafficLight As New TrafficLight()
			myTrafficLight.Start("Ciao")
			autoResetEvent_Renamed.WaitOne()
			ts.TraceInformation("TrafficLight has ended")
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
