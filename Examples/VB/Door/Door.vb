Imports System.Threading
Imports StateForge.StateMachine

Namespace StateForge.Examples.Door

	Public Interface IEngine
		Sub StartOpen()
		Sub StartClose()
		Sub [Stop]()
	End Interface

	Public Class Engine
		Implements IEngine
		Private Shared ts As New TraceSource("Engine")

		Public Sub StartOpen() Implements IEngine.StartOpen
			ts.TraceInformation("StartOpen")
		End Sub

		Public Sub StartClose() Implements IEngine.StartClose
			ts.TraceInformation("StartClose")
		End Sub

		Public Sub [Stop]() Implements IEngine.Stop
			ts.TraceInformation("Stop")
		End Sub
	End Class

	Partial Public Class Door
		Public Engine As IEngine

		Public context As DoorContext
		Private Shared ReadOnly traceName As String = "Door"
		Private Shared ts As New TraceSource(traceName)
'INSTANT VB NOTE: The variable autoResetEvent was renamed since Visual Basic does not handle variables named the same as their type well:
		Private Shared autoResetEvent_Renamed As New AutoResetEvent(False)

		Public Sub New()
			Me.Engine = New Engine()
			InitializeContext()
		End Sub

		Public Sub New(ByVal engine As IEngine)
			Me.Engine = engine
			InitializeContext()
		End Sub

		Private Sub InitializeContext()
			Me.context = New DoorContext(Engine)
			Me.context.Observer = ObserverTrace.Instance(traceName)
			AddHandler context.EndHandler, AddressOf StateMachineEnd
			Me.context.EnterInitialState()
		End Sub

		Private Sub StateMachineEnd(ByVal sender As Object, ByVal e As EventArgs)
			ts.TraceInformation("State machine has ended")
			autoResetEvent_Renamed.Set()
		End Sub

		Public Sub Test01()
			Me.Start()
			Me.OpenRequest()
			Me.Quit()
			autoResetEvent_Renamed.WaitOne()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim door_Renamed = New Door()
			door_Renamed.Test01()
			ts.TraceInformation("Door has ended")
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
