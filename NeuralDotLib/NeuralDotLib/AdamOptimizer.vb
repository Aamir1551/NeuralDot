Public Class AdamOptimizer
    Inherits AdvancedOptimisers
    Private v_dw, v_db, s_dw, s_db As New List(Of Matrix)

    Public Sub New(ByVal _net As Net, ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor)))
        MyBase.New(_net, xydata)
        resetParameters()
    End Sub

    Public Overrides Sub resetParameters()
        'Following code initialises the variables required for ADAM optimisation
        v_dw.Clear() : v_db.Clear() : s_dw.Clear() : s_db.Clear() : losses.Clear() : iterations = 0
        For n As Integer = 0 To model.netLayers.Count - 1
            Dim layer_par As List(Of Tensor) = model.netLayers(n).parameters
            v_dw.Add(New Matrix(layer_par(0).getshape(0), layer_par(0).getshape(1)))
            v_db.Add(New Matrix(layer_par(1).getshape(0), layer_par(1).getshape(1)))
            s_dw.Add(New Matrix(layer_par(0).getshape(0), layer_par(0).getshape(1)))
            s_db.Add(New Matrix(layer_par(1).getshape(0), layer_par(1).getshape(1)))
        Next
    End Sub

    Public Overrides Function run(ByVal l_r As Decimal, ByVal printLoss As Boolean, ByVal batchSize As Integer, ParamArray param() As Decimal) As List(Of Tensor)
        If param.Count <> 2 Then
            Throw New System.Exception("Adam requires 2 parameters for training")
        End If


        Dim batches As List(Of IEnumerable(Of Tuple(Of Tensor, Tensor))) = MyBase.splitdata(batchSize) 'This line splits the data into the different batchsizes
        Dim decay_term As Decimal = Math.Sqrt(1 - Math.Pow(param(1), iterations)) / (1 - Math.Pow(param(0), iterations) + 0.000001) 'decay term is being set for this particular iteration

        For Each batch In batches 'Looping through each batch, as we are doing mini-batch gradient descent
            Dim d As Tuple(Of List(Of Matrix), List(Of Matrix)) = calculateGradients(batch)
            Dim dw As List(Of Matrix) = d.Item1 : Dim db As List(Of Matrix) = d.Item2

            'The following code applies Adam Optimization technique to the network
            For layer As Integer = 0 To model.netLayers.Count - 1
                s_dw(layer) = s_dw(layer) * param(1) + (1 - param(1)) * dw(layer) * dw(layer)
                s_db(layer) = s_db(layer) * param(1) + (1 - param(1)) * db(layer) * db(layer)
                v_dw(layer) = (v_dw(layer) * param(0) + (1 - param(0)) * dw(layer))
                v_db(layer) = (v_db(layer) * param(0) + (1 - param(0)) * db(layer))
                model.netLayers(layer).deltaUpdate(-l_r * dw(layer) * (1 / (Matrix.op(Function(x) Math.Sqrt(x), s_dw(layer)) + 0.000001)), -l_r * db(layer) * (1 / (Matrix.op(Function(x) Math.Sqrt(x), s_db(layer)) + 0.000001)))
                model.netLayers(layer).deltaUpdate(-l_r * v_dw(layer) * (1 / (Matrix.op(Function(x) Math.Sqrt(x), s_dw(layer)) + 0.00001)) * decay_term, -l_r * v_db(layer) * (1 / (Matrix.op(Function(x) Math.Sqrt(x), s_db(layer)) + 0.00001)) * decay_term)
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