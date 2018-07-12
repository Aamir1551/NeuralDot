Public Class Momentum
    Inherits AdvancedOptimisers
    Private v_dw, v_db As New List(Of Matrix)

    Public Sub New(ByVal _net As Net, ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor)))
        MyBase.New(_net, xydata)
        resetParameters()
    End Sub

    Public Overrides Sub resetParameters()
        'Following code initialises the variables required for momentum optimisation
        v_dw.Clear() : v_db.Clear() : losses.Clear() : iterations = 0
        For n As Integer = 0 To model.netLayers.Count - 1
            Dim layer_par As List(Of Tensor) = model.netLayers(n).parameters
            v_dw.Add(New Matrix(layer_par(0).getshape(0), layer_par(0).getshape(1)))
            v_db.Add(New Matrix(layer_par(1).getshape(0), layer_par(1).getshape(1)))
        Next
    End Sub

    Public Overrides Function run(ByVal l_r As Decimal, ByVal printLoss As Boolean, ByVal batchSize As Integer, ParamArray param() As Decimal) As List(Of Tensor)
        If param.Count <> 1 Then
            Throw New System.Exception("Momentum requires 1 parameters for training")
        End If

        Dim batches As List(Of IEnumerable(Of Tuple(Of Tensor, Tensor))) = MyBase.splitdata(batchSize) 'This line splits the data into the different batchsizes

        For Each batch In batches 'Looping through each batch, as we are doing mini-batch gradient descent
            Dim d As Tuple(Of List(Of Matrix), List(Of Matrix)) = MyBase.calculateGradients(batch)
            Dim dw As List(Of Matrix) = d.Item1 : Dim db As List(Of Matrix) = d.Item2 'This line retreives the gradients for w & b

            'Following code applies the momentum optimiser technique to the network
            For layer As Integer = 0 To model.netLayers.Count - 1
                v_dw(layer) = (v_dw(layer) * param(0) + (1 - param(0)) * dw(layer))
                v_db(layer) = (v_db(layer) * param(0) + (1 - param(0)) * db(layer))
                model.netLayers(layer).deltaUpdate(-l_r * v_dw(layer), -l_r * v_db(layer))
            Next
        Next

        'Following code will store the new loss after the updates have been done
        losses.Add(calculateCost(dataxy))
        If printLoss Then
            Console.WriteLine("Error for epoch {0} is: ", iterations)
            losses.Last.print()
        End If
        iterations += 1
        Return losses

    End Function

End Class