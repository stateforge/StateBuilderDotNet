Imports StateForge.StateMachine


Namespace StateForge.Examples.Microwave

	''' <summary>
	''' IMicrowaveActuator is an interface that is used by the Microwave class to perform actions.
	''' This interface is set in the state machine description through the attribute //StateMachine/settings/object@class
	''' </summary>
	Public Interface IMicrowaveActuator
		Sub DoOn()
		Sub DoOff()
	End Interface

	''' <summary>
	''' MicrowaveActuator is a concrete implementation of the IMicrowaveActuator interface.
	''' </summary>
	Public Class MicrowaveActuator
		Implements IMicrowaveActuator
		Public Sub DoOn() Implements IMicrowaveActuator.DoOn
			Console.WriteLine("DoOn")
		End Sub

		Public Sub DoOff() Implements IMicrowaveActuator.DoOff
			Console.WriteLine("DoOff")
		End Sub
	End Class

	''' <summary>
	''' Microwave is a container class which hold instances of a MicrowaveActuator and a generated MicrowaveContext class.
	''' </summary>
	Partial Public Class Microwave
		Private context As MicrowaveContext

		Public Sub New()
			Me.context = New MicrowaveContext(Me)
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
			Dim microwave_Renamed As New Microwave()
			microwave_Renamed.EvStart()
			microwave_Renamed.EvDoorOpened()
			microwave_Renamed.EvTurnOn()
			microwave_Renamed.EvDoorClosed()
			microwave_Renamed.EvDoorClosed()
			microwave_Renamed.EvCookingDone()
			microwave_Renamed.EvDoorOpened()
			microwave_Renamed.EvTurnOff()
			microwave_Renamed.EvDoorOpened()
			microwave_Renamed.EvTurnOn()
			microwave_Renamed.EvDoorClosed()

			Environment.Exit(0)
		End Sub
	End Class
End Namespace
