Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Threading
Imports StateForge.StateMachine


Namespace StateForge.Examples.Ping

	''' <summary>
	''' Ping 
	''' </summary>
	Partial Public Class Ping
		Public Property Timeout() As Double
		Public Property Target() As String
		Public Property Count() As Integer
		Public Property Rx() As Integer
		Public Property Tx() As Integer

		Private context As PingContext
		Private Shared ReadOnly traceName As String = "Ping"
		Private Shared ts As New TraceSource(traceName)
'INSTANT VB NOTE: The variable autoResetEvent was renamed since Visual Basic does not handle variables named the same as their type well:
		Private Shared autoResetEvent_Renamed As New AutoResetEvent(False)

		Private pingSender As New System.Net.NetworkInformation.Ping()
		Private packetOptions As New PingOptions(50, True)
		Private packetData() As Byte = Encoding.ASCII.GetBytes("................................")

		Public Sub New()
			AddHandler pingSender.PingCompleted, AddressOf PingComplete
			Me.context = New PingContext(Me)
			Me.context.Observer = ObserverTrace.Instance(traceName)
			AddHandler context.EndHandler, AddressOf StateMachineEnd
			Me.context.EnterInitialState()
			Reset()
		End Sub

		'TODO HEEFRE
		Public Sub DoCancel()
			Me.pingSender.SendAsyncCancel()
		End Sub

		Friend Sub Send()
			ts.TraceInformation("Ping: Send to {0}", Me.Target)
			pingSender.SendAsync(Me.Target, CInt(Fix(Me.Timeout)), Me)
		End Sub

		Friend Sub PingComplete(ByVal sender As Object, ByVal e As PingCompletedEventArgs)
			If e.Cancelled Then
				ts.TraceInformation("Ping was canceled")


			ElseIf e.Error IsNot Nothing Then
				ts.TraceEvent(TraceEventType.Error, 1, "An error occured: {0}", e.Error)
				Me.context.EvError()

			Else
				Dim pingResponse As PingReply = e.Reply
				ShowPingResults(pingResponse)
			End If
		End Sub

		Public Sub ShowPingResults(ByVal pingResponse As PingReply)
			If pingResponse Is Nothing Then
				ts.TraceEvent(TraceEventType.Warning, 1, "There was no response")
				Return
			ElseIf pingResponse.Status = IPStatus.Success Then
				ts.TraceEvent(TraceEventType.Information, 1, "Reply from {0}  RTT {1}", pingResponse.Address.ToString(), pingResponse.RoundtripTime)
				Me.context.EvPingReply()
			Else
				ts.TraceEvent(TraceEventType.Warning, 1, "Ping was unsuccessful, ip status {0}", pingResponse.Status)
				Me.context.EvError()
			End If
		End Sub

		Private Sub Reset()
			Me.Target = "127.0.0.1"
			Me.Timeout = 1000 ' msec
			Me.Count = 5
			Me.Rx = 0
			Me.Tx = 0
		End Sub

		Public Sub PrintStatistics()
			Console.WriteLine("Ping to {0}  attempt {1} of {2}, error: {3}", Me.Target, Me.Tx, Me.Count, Me.Tx - Me.Rx)
		End Sub

		Private Sub StateMachineEnd(ByVal sender As Object, ByVal e As EventArgs)
			ts.TraceInformation("State machine has ended")
			autoResetEvent_Renamed.Set()
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim ping_Renamed As New Ping()
			ping_Renamed.Target = "127.0.0.1"
			ping_Renamed.StartPing()
			autoResetEvent_Renamed.WaitOne()
			ts.TraceInformation("Ping has ended")
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
