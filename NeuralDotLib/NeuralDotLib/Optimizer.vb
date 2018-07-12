Public MustInherit Class Optimizer
    Public ReadOnly model As Net, dataxy As IEnumerable(Of Tuple(Of Tensor, Tensor))
    Public iterations As Integer = 0, losses As New List(Of Tensor) 'The list losses will store all the losses for every iteration
    'The variable Model stores the Net being trained by referenced. This means when an update occurs the net is updates
    'dataxy stores the data that will be used to train the net

    Public Sub New(ByRef _net As Net, ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor)))
        model = _net
        dataxy = xydata
    End Sub

    MustOverride Function run(ByVal learning_rate As Decimal, ByVal printLoss As Boolean, ByVal batchSize As Integer, ByVal ParamArray param() As Decimal) As List(Of Tensor)
    'This function "run" is MustOverride, as every optimiser must have a method to train the net using a mini-batch.
    'There is no need to hardcode batch or stoachastic gradient descent as they are just special cases of mini-batch gradient descent, i.e batchsize = m, batchsize = 1, respectively.
    'The iterations variable describe the number of training iterations, used to train the net
    'If printLoss = True, then the loss is printed out on each training epoch
    'param() denotes the parameters that will be used by the optimiser in traning the net
    'After each training epoch, the error will be stored in a list, which is returned by this function

    MustOverride Sub resetParameters() 'This sub-routine resets the parameters being used to train the net. This includes the iterations variable.

    Public Function calculateCost(ByVal xydata As IEnumerable(Of Tuple(Of Tensor, Tensor))) As Matrix
        Dim temp As New Matrix(1, 1)
        For j As Integer = 0 To xydata.Count - 1
            temp += model.loss.f(model.predict(xydata(j).Item1), xydata(j).Item2)
        Next
        Return temp / xydata.Count
    End Function
    'This function returns the average cost of the net using the current weights

    Public Function splitdata(ByVal batchSize As Integer) As List(Of IEnumerable(Of Tuple(Of Tensor, Tensor)))
        Dim batchdata As New List(Of IEnumerable(Of Tuple(Of Tensor, Tensor))) 'This list will store all the data for the seperate batches
        For batchNum As Integer = 0 To dataxy.Count / batchSize - 1
            Dim temp As New List(Of Tuple(Of Tensor, Tensor)) 'The Temp list will store the examples for a particular batch for the gradient descent
            For n As Integer = 0 To batchSize - 1
                temp.Add(dataxy(batchNum * batchSize + n))
            Next
            batchdata.Add(temp.AsEnumerable)
        Next
        Return batchdata
    End Function 'This function will organise all the data examples into seperate batches for minibatch gradient descent


    Public Function calculateGradients(ByVal xypoints As IEnumerable(Of Tuple(Of Tensor, Tensor))) As Tuple(Of List(Of Matrix), List(Of Matrix))

        Dim pred As Tensor
        Dim errors As New List(Of Matrix)
        For Each point In xypoints
            pred = model.predict(point.Item1)
            errors.Add(model.loss.d(pred, point.Item2) * model.netLayers.Peek.parameters.Last)
        Next
        Dim deltas As New Stack(Of Tensor)
        deltas.Push((New Volume(errors) / dataxy.Count).mean(2))

        Dim d As IEnumerable(Of Matrix) = DirectCast(model.netLayers(0), Dense).gradient(deltas.Peek)
        Dim dw As New List(Of Matrix)({d(0)})
        Dim db As New List(Of Matrix)({d(1)})
        For layer As Integer = 1 To model.netLayers.Count - 1

            Dim dlayer As IEnumerable(Of Matrix) = DirectCast(model.netLayers(layer), Dense).gradient(deltas.Peek, model.netLayers(layer - 1).parameters(0))
            dw.Add(dlayer(0))
            db.Add(dlayer(1))
            deltas.Push(db(layer))
        Next
        Return New Tuple(Of List(Of Matrix), List(Of Matrix))(dw, db)
    End Function 'This function calculates the gradients for a batch of xypoints, i.e mini-batch gradient descent

End Class