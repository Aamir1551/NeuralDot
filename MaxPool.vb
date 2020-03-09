Imports NeuralDot

Public Class MaxPool
    Implements Layer(Of Volume)

    Public ReadOnly stridesx, stridesy, kernelx, kernely 'User defined
    Private x_in As Volume
    'x_in is the variable fed into the layer, 
    'No parameters are being trained and no non-linear transformation is being applied so no variables being tuned
    'stridesx, stridesy, kernelx, kernely describe how maxpooling will occur.

    Public Sub New(ByVal _kernelx As Integer, ByVal _kernely As Integer, ByVal _stridesx As Integer, ByVal _stridesy As Integer)
        kernelx = _kernelx : kernely = _kernely
        stridesx = _stridesx : stridesy = _stridesy
    End Sub

    Public ReadOnly Property parameters As List(Of Tensor) Implements Layer(Of Volume).parameters
        Get
            Return New List(Of Tensor)({x_in})
        End Get 'No parameters are being trained, so only x_in (input) is returned 
    End Property

    Public Sub deltaUpdate(ParamArray deltaParams() As Tensor) Implements Layer(Of Volume).deltaUpdate
        Throw New NotImplementedException()
    End Sub 'No updates can occur as no parameters are being updates

    Public Function clone() As Layer(Of Volume) Implements Layer(Of Volume).clone
        Return New MaxPool(kernelx, kernely, stridesx, stridesy)
    End Function 'Function used to clone a layer used when saving a model.

    Public Function f(x As Tensor) As Volume Implements Layer(Of Volume).f
        x_in = x
        Return Volume.maxpool(x, kernely, kernelx, stridesy, stridesx)
    End Function 'Returns the output of the layer using the inputs fed into the layer

    Public Function update(l_r As Decimal, prev_delta As Tensor) As Tensor Implements Layer(Of Volume).update
        Throw New NotImplementedException()
    End Function

    Public Function update(l_r As Decimal, prev_delta As Tensor, ParamArray param() As Tensor) As Tensor Implements Layer(Of Volume).update
        Dim dh As Volume = prev_delta
        Dim result As New List(Of Matrix)
        Dim x_channels = Volume.Items(x_in).ToList
        Dim channel_num As Integer = 0

        'Following code will find the gradient w.r.t the maxpooled elements.
        'This algorithm can be derived through chain-rule and matrix-algebra.
        For x_channel As Integer = 0 To x_channels.Count - 1
            Dim x_slices As List(Of Matrix) = Matrix.submatrix(x_channels(x_channel), kernelx, kernely, stridesx, stridesy).ToList
            Dim temp As New Matrix(x_in.shape.Item1, x_in.shape.Item2)
            Dim k As Integer = 0

            For i As Integer = 0 To x_in.shape.Item1 - kernely Step stridesy
                For j As Integer = 0 To x_in.shape.Item2 - kernelx Step stridesx
                    temp.item(i + 1, i + kernely, j + 1, j + kernelx) += (x_slices(k).max - 0.00001 < x_slices(k)) * dh.item(Math.Truncate(k / dh.shape.Item2) + 1, (k Mod dh.shape.Item2) + 1, x_channel)
                    k += 1
                Next
            Next
            result.Add(temp)
        Next

        Return New Volume(result)
    End Function 'Function returns the gradient of the corresponding maxpool elements
End Class