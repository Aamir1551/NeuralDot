Public Class Mapping
    Public Delegate Function activation(ByVal x As Matrix) As Matrix
    Public Delegate Function loss(ByVal x As Matrix, ByVal y As Matrix) As Matrix
    Protected f_, d_

    'act is a delegate function that is used to map a matrix of numbers to another matrix of numbers via a function "f" which maps a double to a double, 
    'i.e f(double) -> double

    'loss is a delegate function that will be used to find the error and will be used for backprop. loss(x, y) - > e, 
    'where "x" represent the prediction, "y" represents the true value And "e" represents the error 

    Public Sub New(ByVal loss_f As loss, ByVal loss_d As loss)
        'The function loss_f is used to find the loss given the output and actual value.
        'The function loss_d is used to find the dirivative of the loss w.r.t the output
        f_ = loss_f
        d_ = loss_d
    End Sub 'This constructor is used to generate a new loss function

    Public Sub New(ByVal activation_f As activation, ByVal _d As activation)
        'The function _f defined the activation function used in a layer.
        'The function _d is used to find the dirivative of the loss w.r.t the output of the activation function (_f)
        f_ = activation_f
        d_ = _d
    End Sub 'This constructor is used to generate a new activation function

    Function f(ByVal x As Matrix) As Matrix
        Return f_.Invoke(x)
    End Function 'Returns the output using the activation function chosen, given an input "x"

    Function f(ByVal x As Matrix, ByVal y As Matrix) As Matrix
        Return f_.invoke(x, y)
    End Function 'Returns the output using the loss functions chosen, given inputs "x,y"

    Function d(ByVal x As Matrix) As Matrix
        Return d_.Invoke(x)
    End Function 'Function returns the dirivative of the activation function chosen for a particular "x" value

    Function d(ByVal x As Matrix, ByVal y As Matrix) As Matrix
        Return d_.Invoke(x, y)
    End Function 'Function returns the dirivative of the loss function chosen for a particular "x" and "y" value


    'The following are the lists of activation functions defined, and therefore can be used by the network
    Public Shared linear As New Mapping(Function(x) x, Function(x) New Matrix(x.getshape(0), x.getshape(1), 1))
    Public Shared relu As New Mapping(Function(x) x.max(0), Function(x) 0 < x)
    Public Shared sigmoid As New Mapping(AddressOf sigmoidAct, AddressOf sigmoidDerivative)
    Public Shared tanh As New Mapping(Function(x) Matrix.op(AddressOf Math.Tanh, x), Function(x) (1 / Matrix.op(AddressOf Math.Cosh, x)) * (1 / Matrix.op(AddressOf Math.Cosh, x)))
    Public Shared swish As New Mapping(Function(x) x * sigmoid.f(x), Function(x) sigmoid.f(x) * x * sigmoid.d(x))
    Public Shared softmax_act As New Mapping(Function(x) Matrix.exp(x - x.max()) / Matrix.sum(Matrix.exp(x - x.max()), 0).item(1, 1), Function(x) softmax_act.f(x) * (1 - softmax_act.f(x)))



    'The following code for the sigmoid function is for the extra-speed up, as the dirivative of sigmoid is s*(1-s), where s = sigmoid(x)
    Private Shared Function sigmoidAct(ByVal x As Matrix) As Matrix
        Return 1 / (1 + Matrix.op(AddressOf Math.Exp, -1 * x))
    End Function

    Private Shared Function sigmoidDerivative(ByVal x As Matrix) As Matrix
        Dim s = sigmoidAct(x)
        Return s * (1 - s)
    End Function

    'The following are the lists of loss functions defined, and therefore can be used by the network
    'The x value represents the prediction by the net, whereas the y value represents the actual value
    Public Shared squared_error As New Mapping(Function(x, y) (x - y) * (x - y) * 0.5, Function(x, y) (x - y))
    Public Shared softmax As New Mapping(Function(x, y) -1 * Matrix.op(AddressOf Math.Log, x) * y, Function(x, y) (x - y))

End Class