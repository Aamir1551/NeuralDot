Imports NeuralDot

Public Class Dense
    Implements Layer(Of Matrix)

    Private x_in, w, z, b, a As Matrix
    Public ReadOnly act As Mapping 'User defined variable
    Public ReadOnly units As Integer 'User defined variable

    'x_in, w, z, b, a are all variables of type matrix that will be used by the net. 
    'Relationship between variables Is z = matmul(w,x_in) + b : a = act(z) & w = (units, prev_units)
    'w and b are the parameter that will be optimised to lower cost
    'x_in is the input into the layer and act is the activation for this current layer 

    Public ReadOnly Property parameters As List(Of Tensor) Implements Layer(Of Matrix).parameters
        Get
            Return New List(Of Tensor)({w, b, a, z, act.d(z)})
        End Get
    End Property 'Property returns parameters

    Public Sub New(ByVal prevUnits As Integer, ByVal layerUnits As Integer, ByVal layerAct As Mapping, ByVal mean As Double, ByVal std As Double)
        units = layerUnits : act = layerAct
        w = New Matrix(layerUnits, prevUnits, mean, std)
        b = New Matrix(layerUnits, 1, mean, std)
    End Sub 'Constructor initialises parmeters that will be tuned

    Public Function f(ByVal X As Tensor) As Matrix Implements Layer(Of Matrix).f 'Shape of X is (_units_prev, m)
        x_in = X
        z = Matrix.matmul(w, X) + b
        a = act.f(z)
        Return a
    End Function 'Returns the output of the layer using the inputs fed into the layer

    Public Overridable Overloads Function update(ByVal l_r As Decimal, ByVal prev_delta As Tensor, ByVal ParamArray param() As Tensor) As Tensor Implements Layer(Of Matrix).update
        Dim grads As IEnumerable(Of Matrix) = gradient(prev_delta, param) 'grads returns (dw, db)
        w -= l_r * grads(0) : b -= l_r * grads(1) 'Parameters are being updated
        Return grads(1) 'Function returns db, as will be needed for the next-layers update for back-prop
    End Function 'Function updates the parameters using prev_delta and  param

    Public Overloads Function update(ByVal l_r As Decimal, ByVal prev_delta As Tensor) As Tensor Implements Layer(Of Matrix).update
        'This update function is only used by the Last layer in the Net if it is a dense net
        Dim grads As IEnumerable(Of Matrix) = gradient(prev_delta) 'gradient function returns (dw, db)
        w -= l_r * grads(0) : b -= l_r * grads(1) 'Parameters are being updated
        Return grads(1) 'Function returns db, as will be needed for the next-layers update for back-prop
    End Function

    Public Function gradient(prev_delta As Matrix, ByVal ParamArray param() As Tensor) As IEnumerable(Of Matrix)
        'prev_delta is the rate_of_change with respect the previous layers output and param is the weight parameter from the layer above
        Dim delta As Matrix = Matrix.matmul(Matrix.transpose(param(0)), prev_delta) * act.d(z)
        Return New List(Of Matrix)({Matrix.matmul(delta, x_in.transpose), delta})
    End Function 'Function returns gradient in the form = (dw, db)

    Public Function gradient(prev_delta As Matrix) As IEnumerable(Of Matrix)
        Return New List(Of Matrix)({Matrix.matmul(prev_delta, x_in.transpose), prev_delta}) 'Function returns gradient using omly the prev_delta.
        'Function only used if layer is final layer in Net
    End Function

    Public Sub deltaUpdate(ByVal ParamArray deltaParams() As Tensor) Implements Layer(Of Matrix).deltaUpdate
        w += deltaParams(0)
        b += deltaParams(1)
        'Function allows users to make their own updates. Function takes in parameters, and then updates using parameters
    End Sub

    Public Overridable Function clone() As Layer(Of Matrix) Implements Layer(Of Matrix).clone
        Dim cloned As New Dense(Me.w.getshape(1), units, act, 0, 0)
        cloned.w = w.clone : cloned.b = b.clone
        Return cloned
    End Function 'Function used to clone a layer used when saving a model. Note: It is required to first make a foreward propagation, before training clone as the variables such as layer outputs will not have instantiated. 

End Class