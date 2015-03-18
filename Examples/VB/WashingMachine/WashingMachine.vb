Imports StateForge.StateMachine



Namespace StateForge.Examples.WashingMachine

	Partial Public Class WashingMachine
		Private context As WashingMachineContext

		Public Sub New()
			Me.context = New WashingMachineContext(Me)
			Me.context.Observer = ObserverConsole.Instance
			Me.context.EnterInitialState()
		End Sub

		Public Property Observer() As IObserver
			Get
				Return Me.context.Observer
			End Get
			Set(ByVal value As IObserver)
				Me.context.Observer = value
			End Set
		End Property

		Shared Sub Main(ByVal args() As String)
			Dim washingMachine_Renamed As New WashingMachine()
			washingMachine_Renamed.Start()
			washingMachine_Renamed.WashingDone()
			washingMachine_Renamed.Fault()
			washingMachine_Renamed.DiagnoseSuccess()
			washingMachine_Renamed.RinsingDone()
			washingMachine_Renamed.SpinningDone()
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
