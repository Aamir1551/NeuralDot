Imports NeuralDot

Public Class Net

    Public ReadOnly netLayers As New Stack(Of Layer(Of Tensor))
    Public ReadOnly checkpoints As New List(Of Tuple(Of Stack(Of Layer(Of Tensor)), DateTime))
    Public ReadOnly netFeatures As Integer, loss As Mapping
    'Variable netLayers is used to store every layer in the Net, via a stack of layer
    'The checkpoints variable stores, all the saved Models along with their respective Time when they were saved
    'The variable feature is an integer denoting the number of features of the input variable
    'The loss variable is used to denote the cost function being used.

    Public Sub New(ByVal features As Integer, ByVal loss_function As Mapping)
        'Constructing a Net only requires number of features and loss function being used to train Net.
        netFeatures = features
        loss = loss_function
    End Sub

    Public Sub AddDense(ByVal units As Integer, ByVal act As Mapping, Optional mean As Double = 0, Optional std As Double = 1)
        'A dense layer consists of a set of layers, activation being applied after matrix multiplication, and the intial values set.
        'The shape of the transformation matrix = (units,units_in_prev_layer) 
        If netLayers.Count = 0 Then 'If no layer set, then units_in_prev = netFeatures
            netLayers.Push(New Dense(netFeatures, units, act, mean, std))
        ElseIf netLayers.Peek.GetType = GetType(Reshape) Then
            'If prevLayers was a conv_layer then units_in_prev = reshape.units
            netLayers.Push(New Dense(DirectCast(netLayers.Peek(), Reshape).units, units, act, mean, std))
        Else
            netLayers.Push(New Dense(DirectCast(netLayers.Peek(), Dense).units, units, act, mean, std))
        End If
    End Sub

    Public Sub AddConv(ByVal filters As Integer, ByVal kernelx As Integer, ByVal kernely As Integer, ByVal stridesx As Integer, ByVal stridesy As Integer,
                ByVal act As Mapping, Optional ByVal padding As String = "valid", Optional ByVal mean As Double = 0, Optional ByVal std As Double = 1)
        netLayers.Push(New Conv(filters, kernelx, kernely, stridesx, stridesy, padding, act, mean, std))
    End Sub 'This sub-routine adds a conv-layer, with the properties defined by the user

    Public Sub AddMaxPool(ByVal kernelx As Integer, ByVal kernely As Integer, ByVal stridesx As Integer, ByVal stridesy As Integer)
        netLayers.Push(New MaxPool(kernelx, kernely, stridesx, stridesy))
    End Sub 'This sub-routine adds a MaxPooling layer, with properties defined by the user

    Public Sub AddReshape(ByVal v_rows As Integer, ByVal v_cols As Integer, ByVal v_depth As Integer)
        netLayers.Push(New Reshape(v_rows, v_cols, v_depth))
    End Sub 'This sub adds a reshape-layer that will be used to reshape a volume into a matrix. This is used to go from one-type of layer of type Tensor
    'to another of type Tensor

    Public Function predict(ByVal x As Tensor) As Tensor
        Dim pred As Tensor = x
        For Each layer In netLayers.Reverse
            pred = layer.f(pred)
        Next
        Return pred
    End Function 'Function returns prediction using the current weight values of the layers

    Public Function predict(ByVal x As IEnumerable(Of Tensor)) As IEnumerable(Of Tensor)
        Dim result As IEnumerable(Of Tensor) = x.Select(Function(g) predict(g))
        Return result
    End Function 'Function returns prediction for an enumerable of inputs

    Public Sub save()
        Dim ops_new As New Stack(Of Layer(Of Tensor))
        For Each layer In Me.netLayers.Reverse
            ops_new.Push(layer.clone)
        Next
        checkpoints.Add(New Tuple(Of Stack(Of Layer(Of Tensor)), Date)(ops_new, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")))
    End Sub 'sub-routine saves current state of the Net along with time saved to checkpoint list

    Public Function load(ByVal check_point As Integer) As Net
        If check_point > checkpoints.Count - 1 Or check_point < 0 Then
            Throw New System.Exception("Check point is out of range")
        End If
        Dim prev_model As New Net(Me.netFeatures, Me.loss)
        For Each layer In Me.checkpoints(check_point).Item1.Reverse
            prev_model.netLayers.Push(layer.clone)
        Next
        Return prev_model
    End Function 'Function returns Net saved.

End Class
