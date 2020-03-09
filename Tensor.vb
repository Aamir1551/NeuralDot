Public Interface Tensor
    'The base class tensor, will be inherited by the Volume and the Matrix class.
    'Also both Volume and Matrix are tensors and will both have functions that are in common.


    Sub print() 'The sub print is used to print out the values of the Tensor. This is nessesary that every child inherits this as the user may want
    'to see all the values the Tensor holds
    Sub transposeSelf() 'This subroutine transposes the Tensor. This is a useful operation as Transpose is used many times in deep-nets, especially for back-prop

    Function clone() As Tensor 'This function will be used by all Tensors, when cloning every layer. This clone function returns the exact same Tensor, with
    'the same values and same state.
    Function normalize(Optional ByVal mean As Double = 0, Optional ByVal std As Double = 1) As Tensor 'This function will be used to normalise the values
    'in a matrix.
    Function getshape() As List(Of Integer) 'Function returns the shape of the Tensor. Function returns a list as the tensors can have an arbitrary number
    'of dimensions.

End Interface