Imports StateForge.StateMachine


Namespace Company.Product

	Partial Public Class HelloWorld
		Private context As HelloWorldContext

		Public Sub New()
			context = New HelloWorldContext(Me)
			Me.context.Observer = ObserverConsole.Instance
		End Sub

		Protected Friend Sub DoPrint()
			Console.WriteLine("HelloWorld")
		End Sub

		Shared Sub Main(ByVal args() As String)
			Dim helloWorld_Renamed = New HelloWorld()
			helloWorld_Renamed.EvPrint()
			Environment.Exit(0)
		End Sub
	End Class
End Namespace
