Imports StateForge.StateMachine

Namespace StateForge.Examples.Light

	Public Interface ILightActuator
		Sub DoOn()
		Sub DoOff()
	End Interface

	Public Class LightActuator
		Implements ILightActuator
		Public Sub DoOn() Implements ILightActuator.DoOn
			Console.WriteLine("DoOn")
		End Sub

		Public Sub DoOff() Implements ILightActuator.DoOff
			Console.WriteLine("DoOff")
		End Sub
	End Class

	Partial Public Class Light
'INSTANT VB NOTE: The variable lightActuator was renamed since Visual Basic does not handle variables named the same as their type well:
		Private lightActuator_Renamed As LightActuator
		Private context As LightContext

		Public Sub New()
			Me.lightActuator_Renamed = New LightActuator()
			Me.context = New LightContext(lightActuator_Renamed)
			Me.context.Observer = ObserverConsole.Instance
			Me.context.EnterInitialState()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim light_Renamed As New Light()
			light_Renamed.EvOn()
			'light.On();
			'light.On();
			'light.Off();
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
