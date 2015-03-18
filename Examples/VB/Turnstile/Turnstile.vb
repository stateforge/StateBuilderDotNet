Namespace StateForge.Examples.Turnstile

	Public Class Barrier
		Public Overridable Sub DoLock()
			Console.WriteLine("Barrier.DoLock")
		End Sub

		Public Overridable Sub DoUnlock()
			Console.WriteLine("Barrier.DoUnlock")
		End Sub
	End Class

	Public Class Alarm
		Public Overridable Sub DoRing()
			Console.WriteLine("Alarm.DoRing")
		End Sub
		Public Overridable Sub DoAlertStaff()
			Console.WriteLine("Alarm.DoAlertStaff")
		End Sub
	End Class

	Public Class CoinMachine
		Public Overridable Sub DoAccept()
			Console.WriteLine("CoinMachine.DoAccept")
		End Sub

		Public Overridable Sub DoReject()
			Console.WriteLine("CoinMachine.DoReject")
		End Sub
	End Class

	Partial Public Class Turnstile
'INSTANT VB NOTE: The variable barrier was renamed since Visual Basic does not handle variables named the same as their type well:
		Private barrier_Renamed As Barrier
'INSTANT VB NOTE: The variable alarm was renamed since Visual Basic does not handle variables named the same as their type well:
		Private alarm_Renamed As Alarm
'INSTANT VB NOTE: The variable coinMachine was renamed since Visual Basic does not handle variables named the same as their type well:
		Private coinMachine_Renamed As CoinMachine
		Private context As TurnstileContext

		Public Sub New()
			Me.barrier_Renamed = New Barrier()
			Me.alarm_Renamed = New Alarm()
			Me.coinMachine_Renamed = New CoinMachine()
			Me.context = New TurnstileContext(barrier_Renamed, alarm_Renamed, coinMachine_Renamed)
			Me.context.EnterInitialState()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim myTurnstile As New Turnstile()
			myTurnstile.Coin()
			myTurnstile.Pass()
			myTurnstile.Coin()
			myTurnstile.Pass()
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
