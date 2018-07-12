Imports NeuralDot

Public Interface Layer(Of Out T As Tensor)
    'This base class Layer will be inherited by Dense, Conv, MaxPool and Reshape
    'This base class will hold the common function that all thses layers will use
    'This is a generic class of type Tensor, as all layers must be of type Tensor

    ReadOnly Property parameters As List(Of Tensor)
    'The parameter property will be in common for all layers as all layers will need to output the variables they are storing
    'This property is useful for back-prop and debugging

    Function f(ByVal x As Tensor) As T
    'Function will be used to forward propagate through a layer
    Function clone() As Layer(Of T)
    'Function will be used to clone a layer. This is useful when saving a model as all layers will need to be cloned when saving a model
    Function update(ByVal learning_rate As Decimal, ByVal prev_delta As Tensor, ByVal ParamArray param() As Tensor) As Tensor
    'This function is used when a layer depends upon the prevous layers parameters. Therefore, this function is a MustInherit, as when the user defines their
    'own functions they may need to use this depending upon how forward propagation works within the layers

    Function update(ByVal learning_rate As Decimal, ByVal prev_delta As Tensor) As Tensor
    'This function updates the parameters usisng prev_delta which is the gradient of loss function w.r.t the parameters
    'The applicability of this function depends upon how the forward propagation works in this layer.

    Sub deltaUpdate(ByVal ParamArray deltaParams() As Tensor)
    'This sub-routine updates the parameters that are being trained, using deltaParams as the respective gradient via backprop
End Interface
