Imports NeuralDot

Public MustInherit Class AdvancedOptimisers
    Inherits Optimizer

    Public Sub New(ByRef _net As Net, ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor)))
        MyBase.New(_net, xydata)

        'The following code, makes sure that the net is a dense net only
        For Each layer In _net.netLayers
            If layer.GetType <> GetType(Dense) Then
                Throw New System.Exception("For AdvancedOptimizers, Network must be a dense neural network")
            End If
        Next
        model.predict(xydata.ToList(0).Item1) 'By running this line of code, Object reference errors are avoided as any non initialised variables are then initialised.
    End Sub

    Public MustOverride Overrides Function run(ByVal learning_rate As Decimal, ByVal printLoss As Boolean, ByVal batchSize As Integer, ByVal ParamArray param() As Decimal) As List(Of Tensor)
    Public MustOverride Overrides Sub resetParameters()

End Class
