Imports NeuralDot

Public Class Reshape
    Implements Layer(Of Matrix)

    Private x_in As Volume
    Public ReadOnly units, v_rows, v_cols, v_depth As Integer
    'x_in is the Volume variable fed into the layer
    'v_rows, v_cols, v_depth depends upon the previous layer

    'This reshape layer isn't limited to transforming from a conv-layer to a dense-layer, but can be used to go from one user-defined layer, of type Volume
    'to another of type Matrix

    Public Sub New(ByVal _v_rows As Integer, ByVal _v_cols As Integer, ByVal _v_depth As Integer)
        v_rows = _v_rows : v_cols = _v_cols : v_depth = _v_depth : units = (_v_cols * _v_rows * _v_depth)
    End Sub

    Public ReadOnly Property parameters As List(Of Tensor) Implements Layer(Of Matrix).parameters
        Get
            Return New List(Of Tensor)({x_in})
        End Get
    End Property 'No parameters are being trained, so only x_in (input) is returned 

    Public Function clone() As Layer(Of Matrix) Implements Layer(Of Matrix).clone
        Dim cloned As New Reshape(v_rows, v_cols, v_depth)
        Return cloned
    End Function 'Function used to clone a layer used when saving a model.

    Public Function f(x As Tensor) As Matrix Implements Layer(Of Matrix).f
        If x.GetType <> GetType(Volume) Then
            Throw New System.Exception("Input needs to be a volume for reshape")
        End If
        x_in = x
        Return Volume.cast(x_in, x_in.shape.Item1 * x_in.shape.Item2 * x_in.values.Count, 1)
    End Function 'Returns the output of the layer using the inputs fed into the layer, after applying the reshape

    Public Function update_d(l_r As Decimal, prev_delta As Tensor, ParamArray param() As Tensor) As Tensor Implements Layer(Of Matrix).update
        Return Volume.cast(Matrix.matmul(Matrix.transpose(param(0)), prev_delta), x_in.shape.Item1, x_in.shape.Item2)
        'Function casts the prev_deltas into a volume for the back-prop of the conv layers
    End Function

    Public Function update(l_r As Decimal, prev_delta As Tensor) As Tensor Implements Layer(Of Matrix).update
        Throw New NotImplementedException()
    End Function 'Function not nesesary as no parameters are being trained

    Public Sub deltaUpdate(ParamArray deltaParams() As Tensor) Implements Layer(Of Matrix).deltaUpdate
        Throw New NotImplementedException()
    End Sub 'Function not nesesary as no parameters are being trained

End Class