Public Class GradientDescentOptimizer
    Inherits Optimizer

    Public Sub New(ByRef _net As Net, ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor)))
        MyBase.New(_net, xydata)
        resetParameters()
    End Sub

    Public Overrides Sub resetParameters()
        iterations = 0 : losses.Clear()
    End Sub

    Public Overrides Function run(ByVal learning_rate As Decimal, ByVal printLoss As Boolean, ByVal batchSize As Integer, ByVal ParamArray param() As Decimal) As List(Of Tensor)
        Dim batches As List(Of IEnumerable(Of Tuple(Of Tensor, Tensor))) = MyBase.splitdata(batchSize) 'This line splits the data into the different batchsizes
        'Following code find the average cost for this particulat mini-batch 

        Dim pred As Tensor
        Dim errors As New List(Of Matrix)

        For Each batch In batches 'Looping over every batch
            'The following code wil now find the average dirivative w.r.t the output of the net for this particulat mini-batch
            For Each vector In batch
                pred = model.predict(vector.Item1)
                errors.Add(model.loss.d(pred, vector.Item2) * model.netLayers.Peek.parameters.Last)
            Next
            Dim deltas As New Stack(Of Tensor)
            deltas.Push((New Volume(errors) / batch.Count).mean(2))

            'Following code updates each layer using deltas
            model.netLayers.Peek.update(learning_rate, deltas.Peek)
            For layer As Integer = 1 To model.netLayers.Count - 1
                deltas.Push(model.netLayers(layer).update(learning_rate, deltas.Peek, model.netLayers(layer - 1).parameters(0)))
            Next
        Next

        losses.Add(calculateCost(dataxy)) 'loss is calulcated using the new updates
        If printLoss Then
            Console.WriteLine("Error for epoch {0} is: ", iterations)
            losses.Last.print()
        End If
        iterations += 1
        Return losses
    End Function 'Function applies mini-batch gradient descent to train the network

End Class