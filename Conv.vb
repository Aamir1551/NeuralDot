Imports NeuralDot

Public Class Conv
    Implements Layer(Of Volume)
    Public ReadOnly act As Mapping
    Private x_in, filter, z, b, a As Volume
    Public ReadOnly filters_depth, kernelx, kernely, stridesx, stridesy As Integer, padding As String 'User defined variables

    'x_in, filter, z, b, a are all variables, with the trainable variables being only filter and b
    'Relationship between variables is: z = conv2d(x_in, filter) + b : a = act(z)
    'act is the activation function being used for this layer
    'kernelx, kernely denote the width and height of the kernel being applied, respectively.
    'stridesx, stridesy and padding denote the properties of the type of conv2d

    Public Sub New(ByVal _filters_depth As Integer, ByVal _kernelx As Integer, ByVal _kernely As Integer, ByVal _stridesx As Integer, ByVal _stridesy As Integer,
                   ByVal _padding As String, ByVal _act As Mapping, ByVal mean As Double, ByVal std As Double)
        kernelx = _kernelx : kernely = _kernely : filters_depth = _filters_depth
        stridesx = _stridesx : stridesy = _stridesy : act = _act : padding = _padding
        filter = New Volume(kernely, kernelx, filters_depth, mean, std) : b = New Volume(1, 1, _filters_depth, mean, std)
    End Sub 'Constructor initialises parmeters that will be tuned

    Public ReadOnly Property parameters As List(Of Tensor) Implements Layer(Of Volume).parameters
        Get
            Return New List(Of Tensor)({filter, b, a})
        End Get
        'Property returns the parameters of the conv_layer
    End Property

    Public Sub deltaUpdate(ParamArray deltaParams() As Tensor) Implements Layer(Of Volume).deltaUpdate
        filter += deltaParams(0)
        b += deltaParams(1)
        'sub-routine allows user to make their own update to the layer - Useful when user wants to create/test their own optimization algorithms
    End Sub

    Public Overridable Function clone() As Layer(Of Volume) Implements Layer(Of Volume).clone
        Dim cloned As New Conv(filters_depth, kernelx, kernely, stridesx, stridesy, padding, act, 0, 0)
        cloned.filter = filter.clone : cloned.b = b.clone
        Return cloned
        'Function used to clone a layer used when saving a model.
    End Function

    Public Function f(x As Tensor) As Volume Implements Layer(Of Volume).f
        'x is the input into the layer
        x_in = x
        z = Volume.conv2d(x_in, filter, stridesx, stridesy, padding) + b
        a = Volume.op(AddressOf act.f, z)
        Return a
    End Function 'Returns the output of the layer using the inputs fed into the layer

    Public Function update(l_r As Decimal, prev_delta As Tensor) As Tensor Implements Layer(Of Volume).update
        Throw New NotImplementedException()
    End Function

    Public Function update(l_r As Decimal, _prev_dz As Tensor, ParamArray param() As Tensor) As Tensor Implements Layer(Of Volume).update
        'param should be empty as convolution doesn't require previous weights for finding updates.
        Dim dfilter As New List(Of Matrix)
        Dim dz As Volume = _prev_dz.clone
        Dim act_d As Volume = Volume.op(AddressOf act.d, z)
        Dim dx As New List(Of Matrix)

        'Following code finds dfilter, using _prev_dz
        For filter_channel As Integer = 0 To filter.values.Count - 1
            Dim temp As New Volume(kernely, kernelx, 1)
            Dim i As Integer = 0
            For Each kernel_window In Volume.subvolume(x_in, kernelx, kernely, stridesx, stridesy)
                temp += kernel_window * dz.item(Math.Truncate(i / dz.shape.Item2) + 1, (i Mod dz.shape.Item2) + 1, filter_channel) * act_d.item(Math.Truncate(i / dz.shape.Item2) + 1, (i Mod dz.shape.Item2) + 1, filter_channel)
                i += 1
            Next
            dfilter.Add(temp.mean(2))
        Next

        'The Following code finds dx using dfilter
        For dx_channel As Integer = 0 To x_in.values.Count - 1
            Dim dx_channel_sum As New Volume(x_in.shape.Item1, x_in.shape.Item2, 1)
            For f As Integer = 0 To filter.values.Count - 1
                Dim k As Integer = 0
                For i As Integer = 0 To x_in.shape.Item1 - kernely Step stridesy
                    For j As Integer = 0 To x_in.shape.Item2 - kernelx Step stridesx
                        dx_channel_sum.split(i + 1, i + kernely, j + 1, j + kernelx, 0) = filter.values(f) * dz.item(Math.Truncate(k / dz.shape.Item2) + 1, (k Mod dz.shape.Item2) + 1, f)
                        k += 1
                    Next
                Next
                dx.Add(dx_channel_sum.mean(2))
            Next
        Next

        filter -= New Volume(dfilter) * l_r
        b -= Volume.op(AddressOf Matrix.sum, _prev_dz) * l_r
        Return New Volume(dx)
    End Function
End Class