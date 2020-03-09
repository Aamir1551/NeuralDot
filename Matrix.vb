Imports NeuralDot
Public Class Matrix
    Implements Tensor

    Private values(,) As Double
    Private shape As Tuple(Of Integer, Integer)
    Private tState As Boolean = False
    'The values(,) variable is a 2d array that is used to store each element of the matrix
    'The shape variable describes the shape of the matrix in the form (number of rows, number of columns)
    'The tState variable can either be True or False. This is used to denote whether the matrix has been transposed or not

    'Constructors
    Public Sub New(ByVal initial_values(,) As Double)
        values = initial_values
        shape = New Tuple(Of Integer, Integer)(initial_values.GetLength(0), initial_values.GetLength(1))
    End Sub 'COMPLETED  -- This constructor is used to initialize a matrix with initial values

    Public Sub New(ByVal rows As Integer, ByVal cols As Integer)
        Me.shape = New Tuple(Of Integer, Integer)(rows, cols)
        ReDim values(rows - 1, cols - 1)
    End Sub  'COMPLETED -- This constructor is used to create a matrix of zeros with a SET size

    Public Sub New(ByVal rows As Integer, ByVal cols As Integer, ByVal mean As Decimal, ByVal std As Decimal)
        Me.New(rows, cols)
        For j As Integer = 0 To cols - 1
            For i As Integer = 0 To rows - 1
                Me.values(i, j) = Math.Round(norm(mean, std), 2)
            Next
        Next
    End Sub 'Completed -- Constructor Instantiates matrix with values which are normal Distribution

    Public Sub New(ByVal rows As Integer, ByVal cols As Integer, ByVal value As Decimal)
        Me.New(rows, cols)
        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To cols - 1
                Me.values(i, j) = value
            Next
        Next
    End Sub 'COMPLTED -- Constrctor Instantiates matrix with all values the same

    'ALL Constructors Defined

    Public Shared Function arange(ByVal rows As Integer, ByVal cols As Integer) As Matrix
        Dim result As New Matrix(rows, cols)
        Dim i As Double = 0
        For Each d In val(result)
            result.item(Math.Truncate(i / cols) + 1, (i Mod cols) + 1) = i
            i += 1
        Next
        Return result
    End Function 'COMPLETED -- Returns a Matrix that has all the values in an incrementing order. Useful for debugging and Testing Other functions

    Public Function reshape(ByVal rows As Integer, ByVal cols As Integer) As Matrix
        If rows * cols <> Me.shape.Item1 * Me.shape.Item2 Then
            Throw New System.Exception("Matrix dimensions do not conform for reshape")
        End If
        Dim result As New Matrix(rows, cols)
        Dim i As Integer = 0
        For Each d In val(Me)
            result.item(Math.Truncate(i / cols) + 1, (i Mod cols) + 1) = d
            i += 1
        Next
        Return result
    End Function  'COMPLETED -- Rehapes a matrix in to another matrix with shape = (rows, cols)

    Public Function getshape() As List(Of Integer) Implements Tensor.getshape
        Return New List(Of Integer) From {Me.shape.Item1, Me.shape.Item2}
    End Function   'COMPLETED -- Returns the variable shape which is = (values.shape(0) + 1, values.shape(2) + 1)

    Public Property item(ByVal i As Integer, ByVal j As Integer) As Double
        Get
            'Get is used to return a specified value from a matrix, specified by the i,j values
            Try
                If tState = False Then
                    Return values(i - 1, j - 1)
                    'If matrix is tranposed return j,i instead of the value i,j
                Else
                    Return values(j - 1, i - 1)
                End If
            Catch ex As Exception
                If ((0 < i) And (i < Me.getshape(0) + 1)) AndAlso ((0 < j) And (j < Me.getshape(1) + 1)) Then
                    'This is becuase "i,j" is within the acceptebale range, meaning that the exception was due to the number being too small or large 
                    Throw New System.Exception("Value is either too small or too large")
                End If
                Console.WriteLine(Me.getshape(0))
                Console.WriteLine(Me.getshape(1))
                Throw New System.Exception("Index is out of bounds for Matrix")
            End Try
        End Get
        Set(value As Double)
            'Set is used to set a particular element (i,j) of a matrix to a specified value
            Try
                If tState = False Then
                    values(i - 1, j - 1) = value
                Else
                    'If the matrix is transposed then set the j,i item to be the value given
                    values(j - 1, i - 1) = value
                End If
            Catch ex As Exception
                Console.WriteLine(Me.getshape(0))
                Console.WriteLine(Me.getshape(1))
                Throw New System.Exception("Index is out of bounds for Matrix")
            End Try
        End Set
    End Property 'COMPLETED -- Property is used to set or get an item from a Matrix

    Public Property item(ByVal i_start As Integer, ByVal i_end As Integer, ByVal j_start As Integer, ByVal j_end As Integer) As Matrix
        Get
            Dim result As New Matrix(i_end - i_start + 1, j_end - j_start + 1)
            For i As Integer = i_start To i_end
                For j As Integer = j_start To j_end
                    result.item(i - i_start + 1, j - j_start + 1) = Me.item(i, j)
                Next
            Next
            Return result
            'Get is used to return a Submatrix from a matrix
        End Get
        Set(ByVal value As Matrix)
            For i As Integer = i_start To i_end
                For j As Integer = j_start To j_end
                    Me.item(i, j) = value.item(i - i_start + 1, j - j_start + 1)
                Next
            Next
            'Set is used to set a submatrix of a matrix
        End Set
    End Property 'COMPLETED -- Property is used to set or get a submatrix of a matrix

    Public Overloads Sub print() Implements Tensor.print
        For i As Integer = 1 To Me.shape.Item1
            For j As Integer = 1 To Me.shape.Item2
                Console.Write("{0} ", Me.item(i, j))
            Next
            Console.WriteLine("")
        Next
    End Sub 'COMPLETED -- Subroutine is used to print out the values of a matrix

    Public Overloads Shared Sub print(ByVal MatList As IEnumerable(Of Matrix))
        For Each m In MatList
            m.print()
            Console.WriteLine("")
        Next
    End Sub 'COMPLETED -- Prints out several the elements of several matrices

    Public Overloads Shared Sub tensor_print(ByVal vollist As IEnumerable(Of Tensor))
        For Each vol In vollist
            vol.print()
        Next
    End Sub 'COMPLETED -- Used to print out an IEumerable of Tensors

    Public Function get_values() As Double(,)
        Return values
    End Function 'COMPLETED -- Function returns all the values in the matrix

    Public Shared Function randn(ByVal rows As Integer, ByVal cols As Integer) As Matrix
        Return New Matrix(rows, cols, 0, 1)
    End Function 'Completed -- Function returns a matrix of values with normal dist

    Public Sub transposeSelf() Implements Tensor.transposeSelf
        tState = Not tState
        shape = New Tuple(Of Integer, Integer)(Me.shape.Item2, Me.shape.Item1)
    End Sub 'COMPLETED -- Subroutine used to swap "ij" and also swap indices of shape - transposes the current matrix - Does not create an instance

    Public Function transpose() As Matrix
        Dim transposed As Matrix = Me.clone
        transposed.transposeSelf()
        Return transposed
    End Function 'COMPLETED -- Function returns a transposed of a matrix

    Public Shared Function transpose(ByVal m As Matrix) As Matrix
        Dim transposed As Matrix = m.clone
        transposed.transposeSelf()
        Return transposed
    End Function 'COMPLETED -- FUNCTION returns a transposed of a matrix

    Public Function clone() As Tensor Implements Tensor.clone
        Dim cloned As New Matrix(Me.shape.Item1, Me.shape.Item2)
        cloned.values = Me.values.Clone
        cloned.tState = Me.tState
        'For a matrix to be cloned, values tState and shape variables are required to be the same
        Return cloned
    End Function 'completed -- function returns an identical matrix (A clone)

    Public Shared Function remove_index(ByVal m As Matrix, ByVal row As Integer, ByVal axis As Integer) As Matrix
        'If axis is 0 then a column is deleted else if axis is 1 then a row is deleted
        'The row parameter is used to state which row or column is being deleted. The index starts from 0
        If axis = 1 Then
            m.transposeSelf()
            'The matrix(M) is being transposed as removing a row from the matrix(M) is equaivilant to removing a column in the transposed matrix
        End If
        If axis <> 0 And axis <> 1 Then
            Throw New System.Exception("Axis must be 0 or 1")
            'An excpetion is thrown as the direction to remove a column or row is only limited to 0 or 1 meaning column or row, respectively.
        ElseIf Not (0 <= row < m.shape.Item2) Then
            Throw New System.Exception("Index was out of range")
            'An exception is thrown as the row parameter is not within the acceptable range, so that the row/column can be deleted
        End If

        Dim result As New Matrix(m.shape.Item1, m.shape.Item2 - 1)  'The output matrix will have one less column than the input Matrix M
        Dim index_i, index_j As Integer

        'The following is used to copy all the values of the matrix onto the matrix result. The elements of the column/row that will be removed will not be copied on to the matrix result
        For j As Integer = 0 To m.shape.Item2 - 1
            index_i = 0
            If Not (j = row) Then
                For i As Integer = 0 To m.shape.Item1 - 1
                    result.values(index_i, index_j) = m.item(i + 1, j + 1)
                    index_i += 1
                Next
                index_j += 1
            End If
        Next
        If axis = 1 Then
            result.transposeSelf()   'The matrix is transposed to turn it back into its original form. The matrix will only be transposed if the axis is 1 as only then the Matrix is transposed.
        End If
        Return result
    End Function 'COMPLETED --Removes a row/col from the list

    Public Shared Function conv(ByVal m As Matrix, ByVal kernel As Matrix, Optional ByVal stridesx As Integer = 1, Optional ByVal stridesy As Integer = 1, Optional ByVal padding As String = "valid") As Matrix
        Dim paddy, paddx As Integer
        If padding = "full" Then
            'For a full convolution, the Matrix should be first zero-padded such that the every element in the matrix can be used to convolve with the kernel, and then a Valid convolution is applied.
            m = Matrix.padd(m, kernel.shape.Item1 - 1, kernel.shape.Item2 - 1)
            Return conv(m, kernel, stridesx, stridesy, "valid")
        End If
        If padding = "same" Then
            If ((m.shape.Item1 Mod stridesy) = 0) Then
                paddy = Math.Max(kernel.shape.Item1 - stridesy, 0)
            Else
                paddy = Math.Max(kernel.shape.Item1 - (m.shape.Item1 Mod stridesy), 0)
            End If
            If ((m.shape.Item2 Mod stridesx) = 0) Then
                paddx = Math.Max(kernel.shape.Item2 - stridesx, 0)
            Else
                paddx = Math.Max(kernel.shape.Item2 - (m.shape.Item2 Mod stridesx), 0)
            End If
            m = Matrix.addcol(m, 1, Math.Floor(paddy / 2), 1)
            m = Matrix.addcol(m, m.shape.Item1 + 1, paddy - Math.Floor(paddy / 2), 1)
            m = Matrix.addcol(m, 1, Math.Floor(paddx / 2), 0)
            m = Matrix.addcol(m, m.shape.Item2 + 1, paddx - Math.Floor(paddx / 2), 0)
            'The amount of padding done for SAME convolution follows the tensorlflow guidlines for the amount of padding
            Return conv(m, kernel, stridesx, stridesy, "valid")
        ElseIf padding = "valid" Then
            Dim result As New Matrix(Math.Truncate((m.shape.Item1 - kernel.getshape(0)) / stridesy) + 1, Math.Truncate((m.shape.Item2 - kernel.getshape(1)) / stridesx) + 1)
            Dim i As Integer = 0
            'The following code is used to compute the resulting convolved Matrix
            'The dot product is used as convolution is essentially a series of dot products
            For Each S In submatrix(m, kernel.shape.Item2, kernel.shape.Item1, stridesx, stridesy)
                result.values(Math.Truncate(i / result.shape.Item2), i Mod result.shape.Item2) = Matrix.dotsum(S, kernel)
                i += 1
            Next
            Return result
        End If
        Console.WriteLine(padding)
        Throw New System.Exception("Padding must be either valid, same or full")
    End Function 'COMPLETED -- Returns the convolution after a kernel have been applied

    Public Shared Function padd(ByVal m As Matrix, ByVal i As Integer, ByVal j As Integer) As Matrix
        'This function is used to padd a Matrix with zeros and is used allot for convolutions and in general for image manipulation
        Dim result As Matrix = m.clone
        result = addcol(result, 1, i, 0)
        result = addcol(result, result.shape.Item2 + 1, i, 0)
        result = addcol(result, 1, j, 1)
        result = addcol(result, result.shape.Item1 + 1, j, 1)
        Return result
    End Function 'COMPLETED -- Functions returns the matrix padded with zero

    Public Overloads Shared Function addcol(ByVal m As Matrix, ByVal index As Integer, ByVal n As Integer, ByVal axis As Integer) As Matrix
        Select Case axis
            Case 1
                'The matrix(M) is being transposed as adding a row to the matrix(M) is equaivilant to adding a column in the transposed matrix
                Return addcol(m.transpose, index, n, 0).transpose
            Case 0
                If index <= 0 Or index > m.shape.Item2 + 1 Then
                    'The error is thrown here as the index is out of the accpetable bounds for a column to be inserted
                    Throw New System.Exception("Index cannot be more that zero, and needs to be less than the size of dimension in which col/row is being added")
                End If
                Dim output As New Matrix(m.shape.Item1, m.shape.Item2 + n)  'adds "n" extra columns
                'The following code inserts all the values from the Matrix (M) into the output Matrix
                For j As Integer = 0 To m.shape.Item1 - 1
                    For i As Integer = 0 To index - 2
                        output.values(j, i) = m.item(j + 1, i + 1)
                    Next
                Next
                For j As Integer = 0 To output.shape.Item1 - 1
                    For i As Integer = index To output.shape.Item2 - 1 - n + 1
                        output.values(j, i + n - 1) = m.item(j + 1, i)
                    Next
                Next
                Return output
            Case Else
                'Error is thrown here as only a column or row can only be inserted
                Throw New System.Exception("axis can only be 0 or 1")
        End Select
    End Function 'COMPLETED -- Adds "n" number of cols/rows to a matrix

    Public Overloads Shared Function join(ByVal m As Matrix, ByVal n As Matrix, ByVal index As Integer) As Matrix
        If m.shape.Item1 <> n.shape.Item1 Then
            'An error is thrown here is both Matrices do not have the same number of rows
            Throw New System.Exception("Number of Rows must be the same for both Matrices")
        End If
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2 + n.shape.Item2)
        Dim i As Integer = 0
        For k As Integer = 0 To m.shape.Item2 - 1
            If i = index Then
                i += n.shape.Item2
            End If
            For l As Integer = 0 To m.shape.Item1 - 1
                result.values(l, i) = m.item(l + 1, k + 1)
            Next
            i += 1
        Next
        For k As Integer = 0 To n.shape.Item2 - 1
            For l As Integer = 0 To m.shape.Item1 - 1
                result.values(l, index + k) = n.item(l + 1, k + 1)
            Next
        Next
        Return result
    End Function 'COMPLETED -- Concatenates a Matrix(M) to another Matrix(N) at the specified axis

    Public Function maxpool(ByVal kernelx As Integer, ByVal kernely As Integer, ByVal stridesx As Integer, ByVal stridesy As Integer) As Matrix
        Dim result As New Matrix(((Me.shape.Item1 - kernely) / stridesy) + 1, ((Me.shape.Item2 - kernelx) / stridesx) + 1)
        Dim i As Integer = 0
        'Following code is used to select the Maximum element out of each submatrix
        For Each m In submatrix(Me, kernelx, kernely, stridesx, stridesy) 'submatrix is a function of type ienumerable
            result.values(Math.Truncate(i / result.shape.Item2), i Mod result.shape.Item2) = m.max()
            i += 1
        Next
        Return result
    End Function 'Completed -- Applies maxpooling to a matrix with a kernel of size = (kernelx, kernely)

    Public Function max() As Double
        Dim max_val As Double 'Variable Used to store the maxmimum number currently found in Matrix
        For Each value In Me.values
            If value > max_val Then
                max_val = value 'If the current value is larger than max_val, then set max_val to new maximum value
            End If
        Next
        Return max_val
    End Function 'COMPLETED -- Returns the maximum value in a Matrix

    Public Shared Function rotate(ByVal m As Matrix, Optional ByVal theta As Integer = 1) As Matrix
        If theta = 0 Then 'If theta is 0, then no rotation is required
            Return m
        Else
            'Rotating a matrix 90 degrees clockwise is the same as transposing the matrix, and then reversing each row
            Dim transposed As Matrix = m.transpose
            Dim result As New Matrix(transposed.shape.Item1, 0)
            For Each col In transposed.columns.Reverse
                result = Matrix.join(result, col, result.shape.Item2)
            Next
            Return rotate(result, theta - 1) 'As theta is a multiple of 90 and rotation is a commutative operation, if theta > 1, then return matrix rotated by theta-1 times
        End If
    End Function 'COMPLETED -- Rotates the matrix by theta * 90

    Public Function oneHot(ByVal num_classes As Integer) As Matrix
        'oneHot converts a row vector into a matrix of shape(num_classes, num_samples). Each corresponding item in the row vector is then used
        'to select each column in the resulting matrix by placing a 1, whilst the rest of the items are set to 0.
        If Me.getshape(0) <> 1 Then 'Checking to see if the matrix is a row vector or not
            Throw New System.Exception("Matrix must be a row vector for one hot")
        End If
        Dim oneHotArr(num_classes - 1, Me.getshape(1) - 1) As Double
        For j As Integer = 0 To Me.getshape(1) - 1
            oneHotArr(Me.item(1, j + 1), j) = 1 'Useing the items in the matrix to select, place a 1, in the resultant matrix
        Next
        Return New Matrix(oneHotArr)
    End Function 'COMPLETED -- Returns the onehot of a matrix

    Public Function invOneHot() As Matrix
        Dim result As New Matrix(1, Me.getshape(1))
        For j As Integer = 1 To Me.getshape(1)
            Dim maxval As Double = Double.MinValue 'Variable used to store the maximum item in this column vector
            Dim pos As Integer = 0 'Variable used to store the position of the maximum item in the vector
            For i As Integer = 1 To Me.getshape(0)
                If Me.item(i, j) > maxval Then 'If the maximum item is greater than assign the maxmav to that item and "pos" to the position of that maxmimum value
                    maxval = Me.item(i, j)
                    pos = i
                End If
            Next
            result.item(1, j) = pos - 1
        Next
        Return result
    End Function 'COMPLETED -- Returns the inverse of Onehot encoding to a matrix

    Public Function normalize(Optional ByVal mean As Double = 0, Optional ByVal std As Double = 1) As Tensor Implements Tensor.normalize
        Dim means As Matrix = Matrix.sum(Me, 0) / Me.getshape(0) 'Finds the column means
        Dim stds As Matrix = Matrix.op(AddressOf Math.Pow, (Matrix.sum(Matrix.op(AddressOf Math.Pow, Me, 2), 0) - Matrix.op(AddressOf Math.Pow, Me, 2) * Me.getshape(0)) / (Me.getshape(0) - 1), 0.5) 'Finds the std for this particular colum

        Dim result As New Matrix(Me.getshape(0), Me.getshape(1))
        For i As Integer = 1 To Me.getshape(0)
            For j As Integer = 1 To Me.getshape(1)
                result.item(i, j) = (Me.item(i, j) - means.item(1, j)) / stds.item(1, j) 'normalises each item in the matrix
            Next
        Next
        Return (result * std) + mean
    End Function  'COMPLETED -- Returns the normalised version of a particular matrix

    Public Function broadcast(ByVal m As Matrix) As Matrix
        If m.shape.Item2 = 1 Then
            Dim vals(Me.shape.Item1 - 1, Me.shape.Item2 - 1) As Double
            For i As Integer = 1 To Me.shape.Item1
                For j As Integer = 1 To Me.shape.Item2
                    vals(i - 1, j - 1) = m.item(i, 1) 'Every column of the matrix is duplicated and joined together
                Next
            Next
            Return New Matrix(vals)
        ElseIf m.shape.Item1 = 1 Then
            Return Me.broadcast(m.transpose).transpose
        Else
            Throw New System.Exception("m.getshape(1) must be 1 for broadcast")
        End If
    End Function 'Broadcast is used to apply an operation to two matrices that are not of the same shape. One of the matrices however, must be a row/column vector

    Public Shared Function op(ByVal f As Func(Of Double, Double), ByVal m As Matrix) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2)
        For i As Integer = 1 To m.shape.Item1
            For j As Integer = 1 To m.shape.Item2
                result.values(i - 1, j - 1) = f.Invoke(m.item(i, j))
            Next
        Next
        Return result
    End Function 'COMPLETED -- Applies a function(double -> double) elementwise to a matrix

    Public Shared Function op(ByVal f As Func(Of Double, Double, Double), ByVal m As Matrix, ByVal n As Double) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2)
        For i As Integer = 1 To m.shape.Item1
            For j As Integer = 1 To m.shape.Item2
                result.values(i - 1, j - 1) = f.Invoke(m.item(i, j), n) 'Applies function "f(x,y)" using parameters (m_ij, n) 
            Next
        Next
        Return result
    End Function 'COMPLETED -- Applies a function((double, dobule) -> double) elementwise to matrix using "n" as the second parameter

    Public Shared Function sum(ByVal m As Matrix, ByVal axis As Integer) As Matrix
        Dim result As New Matrix(1, m.shape.Item2)
        If axis = 0 Then
            'Following code sums up all the columns of the matrix
            For j As Integer = 1 To m.shape.Item2
                Dim sum_ As Double = 0
                For i As Integer = 1 To m.shape.Item1
                    sum_ += m.item(i, j)
                Next
                result.values(0, j - 1) = sum_
            Next
        ElseIf axis = 1 Then
            Return sum(m.transpose, 0).transpose()
        Else
            Throw New System.Exception("Axis to reduce mean can only be 0 or 1")
        End If
        Return result
    End Function 'COMPLETED -- Returns a reduced matrix by summing in the direction of "axis"

    Public Shared Function sum(ByVal m As Matrix) As Matrix
        Return Matrix.sum(Matrix.sum(m, 0), 1)
    End Function 'Completed -- Returns the Sum of all the elements in the Matrix

    Public Shared Function AbsSum(ByVal m As Matrix) As Matrix
        Return Matrix.sum(Matrix.sum(Matrix.op(AddressOf Math.Abs, m), 0), 1)
    End Function 'COMPLETED -- Returns the absolute sum of a matrix

    Public Shared Function SquaredSum(ByVal m As Matrix) As Matrix
        Return Matrix.sum(Matrix.sum(Matrix.op(AddressOf Math.Pow, m, 2), 0), 1)
    End Function 'COMPLETED -- Returns the sum of the squares of each item in a matrix

    Public Shared Function mean(ByVal m As Matrix, ByVal axis As Integer) As Matrix
        Dim n As Integer = m.getshape(axis)
        Return sum(m, axis) / n
    End Function 'COMPLETED -- Returns a reduced matrix by calculating the mean in the direction of "axis"

    Public Shared Function exp(ByVal m As Matrix) As Matrix
        Return op(AddressOf Math.Exp, m)
    End Function 'COMPLETED -- Exponentiates every item in the Matrix

    'Defining matrix joint operations
    Public Shared Function add(ByVal x As Matrix, ByVal y As Matrix, Optional broadcast As Boolean = True) As Matrix

        'Following code checks for any possible broadcasting
        'If one of the matrices, contain only 1 item, then apply scaler addition to the matrices
        If (Not (y Like x)) And broadcast Then
            If x.shape.Item1 = x.shape.Item2 And x.shape.Item2 = 1 Then
                Return y + x.item(1, 1)
            ElseIf y.shape.Item1 = y.shape.Item2 And y.shape.Item2 = 1 Then
                Return x + y.item(1, 1)
            End If
            'If either of matrices, have a dimension that is of length 1, then brodcast in that dimension
            If x.shape.Item2 = 1 Or y.shape.Item2 = 1 Then
                If x.shape.Item2 = 1 Then
                    Return y.broadcast(x) + y
                Else
                    Return x.broadcast(y) + x
                End If
            ElseIf x.shape.Item1 = 1 Or y.shape.Item1 = 1 Then
                If x.shape.Item1 = 1 Then
                    Return y.broadcast(x) + y
                Else
                    Return x.broadcast(y) + x
                End If
            End If
            Console.WriteLine("Shape of me is {0}, {1}, Shape of B is {2}, {3},", y.shape.Item1, y.shape.Item2, x.shape.Item1, x.shape.Item2)
            Throw New System.Exception("Shapes do not conform for addition")
        End If

        'The Following code, is used to add two matrices together
        Dim outputshape As Tuple(Of Integer, Integer) = x.shape
        Dim c As New Matrix(outputshape.Item1, outputshape.Item2)
        For i As Integer = 1 To x.shape.Item1
            For j As Integer = 1 To x.shape.Item2
                c.values(i - 1, j - 1) = y.item(i, j) + x.item(i, j)
            Next
        Next
        Return c
    End Function  'COMPLETED -- Adds to matrixes together elementwise

    Public Shared Function add(ByVal x As Matrix, ByVal y As Decimal) As Matrix
        Dim result As New Matrix(x.shape.Item1, x.shape.Item2, y)
        Return x + result
    End Function 'COMPLETED -- Returns the sum of a matrix with a decimal value

    Public Shared Function matmul(ByVal x As Matrix, ByVal y As Matrix) As Matrix
        If x.shape.Item2 <> y.shape.Item1 Then
            Console.WriteLine("Shape of A is {0}, {1}. Shape of B is {2}, {3}", x.shape.Item1, x.shape.Item2, y.shape.Item1, y.shape.Item2)
            Throw New System.Exception("Shapes do not conform for matrix multiplication")
        End If
        'The following code, is for matrix multiplication using the standard way
        Dim result As New Matrix(x.shape.Item1, y.shape.Item2)
        For i As Integer = 0 To x.shape.Item1 - 1
            For j As Integer = 0 To y.shape.Item2 - 1
                Dim sum As Decimal = 0
                For k = 0 To x.shape.Item2 - 1
                    Try
                        sum += x.item(i + 1, k + 1) * y.item(k + 1, j + 1)
                    Catch ex As Exception
                        Throw New System.Exception("Value was too large or too Small.")
                    End Try
                Next
                result.values(i, j) = sum
            Next
        Next
        Return result
    End Function  'COMPLETED -- Returns the product of Matrix Multiplication

    Public Shared Function multiply(ByVal m As Matrix, ByVal c As Decimal) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2, c)
        Return result * m
    End Function 'COMPLETED -- Returns the Result of a Scaler multiplication with a constant c

    Public Shared Function multiply(ByVal x As Matrix, ByVal y As Matrix) As Matrix
        If Not (y Like x) Then
            Console.WriteLine("y has shape {0}, {1}", y.shape.Item1, y.shape.Item2)
            Console.WriteLine("x has shape {0}, {1}", x.shape.Item1, x.shape.Item2)
            Throw New System.Exception("Shapes do not conform for elementwise multiplication")
        End If
        Dim result As New Matrix(y.shape.Item1, y.shape.Item2)
        For i As Integer = 1 To y.shape.Item1
            For j As Integer = 1 To y.shape.Item2
                Try
                    result.values(i - 1, j - 1) = x.item(i, j) * y.item(i, j)
                Catch ex As Exception
                    Throw New System.Exception("Value is either too large or too small")
                End Try
            Next
        Next
        Return result
    End Function 'COMPLETED -- Returns the multiplication of two matrices elementwise

    Public Shared Function dotsum(ByVal x As Matrix, ByVal y As Matrix) As Double
        Dim product As Matrix = y * x
        Return Matrix.sum(Matrix.sum(product, 1)).item(1, 1)
    End Function 'COMPLETED -- Returns the sum of all the elementwise multiplication of the 2 matrices

    ' All Joint Operations defined


    ' All operators defined below

    Public Shared Operator =(ByVal a As Matrix, ByVal b As Matrix) As Boolean
        If Not a Like b Then
            Return False
        End If
        For i As Integer = 1 To a.shape.Item1
            For j As Integer = 1 To a.shape.Item2
                If a.item(i, j) <> b.item(i, j) Then 'Statement compare each value in the matrix
                    Return False
                End If
            Next
        Next
        Return True
    End Operator 'Completed -- Checks if all items in the matrix are the same

    Public Shared Operator <>(ByVal a As Matrix, ByVal b As Matrix) As Boolean
        Return Not a = b
    End Operator 'Completed -- Returns: Not=

    Public Shared Operator +(ByVal a As Matrix, ByVal b As Matrix) As Matrix
        Return Matrix.add(a, b)
    End Operator  'COMPLETED -- Returns the sum of two matrices elementwise

    Public Shared Operator +(ByVal a As Matrix, ByVal b As Decimal) As Matrix
        Return Matrix.add(a, b)
    End Operator 'COMPLETED -- Returns the sum of a decimal value to a matrix elementwise

    Public Shared Operator +(ByVal a As Decimal, ByVal b As Matrix) As Matrix
        Return Matrix.add(b, a)
    End Operator 'COMPLETED -- Returns the sum of a decimal value to a matrix elementwise

    Public Shared Operator -(ByVal a As Matrix, ByVal b As Matrix) As Matrix
        Return a + (-1 * b)
    End Operator 'COMPLETED -- Returns the differece of two matrices elementwise

    Public Shared Operator -(ByVal a As Matrix, ByVal b As Decimal) As Matrix
        Return Matrix.add(a, -b)
    End Operator 'COMPLETED -- Returns the differece of a matrix with a decimal number

    Public Shared Operator -(ByVal a As Decimal, ByVal b As Matrix) As Matrix
        Return New Matrix(b.shape.Item1, b.shape.Item2, a) - b
    End Operator 'COMPLETED -- Returns the differece of a matrix with a decimal number

    Public Shared Operator *(ByVal a As Matrix, ByVal b As Matrix) As Matrix
        Return Matrix.multiply(a, b)
    End Operator 'COMPLETED -- Returns the multiplication of two matrices elementwise

    Shared Operator /(ByVal a As Matrix, ByVal b As Matrix) As Matrix
        Return a * (1 / b)
    End Operator

    Public Shared Operator *(ByVal a As Matrix, ByVal b As Decimal) As Matrix 'Scaler multiplication
        Return Matrix.multiply(a, b)
    End Operator 'COMPLETED -- Returns the scaler product of a Matrix with a Decimal

    Public Shared Operator *(ByVal a As Decimal, ByVal b As Matrix) As Matrix
        Return b * a
    End Operator 'COMPLETED --  Returns the scaler product of a Matrix with a Decimal

    Public Shared Operator /(ByVal a As Matrix, ByVal b As Decimal) As Matrix
        Return a * (1 / b)
    End Operator 'COMPLETED -- Returns the scaler division of a matrix

    Public Shared Operator Like(ByVal a As Matrix, ByVal b As Matrix) As Boolean
        If a.shape.Item1 = b.shape.Item1 And a.shape.Item2 = b.shape.Item2 Then
            Return True
        Else
            Return False
        End If
    End Operator 'Completed -- Returns true if both A and B have the same shape   

    Public Function max(ByVal a As Decimal) As Matrix
        Dim result As New Matrix(Me.shape.Item1, Me.shape.Item2)
        For i As Integer = 1 To Me.shape.Item1
            For j As Integer = 1 To Me.shape.Item2
                result.values(i - 1, j - 1) = Math.Max(a, Me.item(i, j))
            Next
        Next
        Return result
    End Function 'COMPLETED -- Applies the max operator to each item in a matrix

    Public Shared Operator <(ByVal a As Decimal, ByVal m As Matrix) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2)
        For i As Integer = 1 To m.shape.Item1
            For j As Integer = 1 To m.shape.Item2
                result.item(i, j) = Int(a < m.item(i, j))
            Next
        Next
        Return result
    End Operator 'COMPLETED -- Compares each value in the matrix elementwise, with another item and returns 1 if value is larger, 
    'Else returns 0 If value was smaller

    Public Shared Operator >(ByVal a As Decimal, ByVal m As Matrix) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2)
        For i As Integer = 1 To m.shape.Item1
            For j As Integer = 1 To m.shape.Item2
                result.item(i, j) = Int(a > m.item(i, j))
            Next
        Next
        Return result
    End Operator 'COMPLETED -- Compares each value in the matrix elementwise, with another item and returns 1 if value is smaller, 

    Public Shared Operator /(ByVal a As Decimal, ByVal m As Matrix) As Matrix
        Dim result As New Matrix(m.shape.Item1, m.shape.Item2)
        For i As Integer = 1 To m.shape.Item1
            For j As Integer = 1 To m.shape.Item2
                result.values(i - 1, j - 1) = a / m.item(i, j)
            Next
        Next
        Return result
    End Operator 'COMPLETED -- each item in the matrix is set to m_ij -> a / m_ij 

    'All Operators defined 

    'Iterators

    Public Overloads Iterator Function columns() As IEnumerable(Of Tensor)
        For j As Integer = 1 To Me.shape.Item2
            Dim result As New Matrix(Me.shape.Item1, 1)
            For i As Integer = 1 To Me.shape.Item1
                result.values(i - 1, 0) = Me.item(i, j) 'Copies items of the column of the matrix to another matrix
            Next
            Yield result
        Next
    End Function 'COMPLETED -- Returns an enumerable of columns of the matrix

    Public Shared Iterator Function submatrix(ByVal m As Matrix, ByVal kernelx As Integer, ByVal kernely As Integer, ByVal stridesx As Integer, ByVal stridesy As Integer) As IEnumerable(Of Matrix)
        For i As Integer = 1 To m.getshape(0) - kernely + 1 Step stridesy
            For j As Integer = 1 To m.getshape(1) - kernelx + 1 Step stridesx
                Yield m.item(i, i + kernely - 1, j, j + kernelx - 1)
            Next
        Next
    End Function 'COMPLETED -- Returns a collection of matrices, by sliding a window of length (kernelx, kernely) and using strides = (stridesx, stridesy)

    Public Shared Iterator Function val(ByVal m As Matrix, Optional stepx As Integer = 1, Optional stepy As Integer = 1) As IEnumerable(Of Double)
        For i As Integer = 1 To m.getshape(0) Step stepy
            For j As Integer = 1 To m.getshape(1) Step stepx
                Yield m.item(i, j)
            Next
        Next
    End Function 'COMPLETED -- Returns items from a matrix, using a stepsize of (stepx, stepy)
    'All iterators defined
    'Matrix Class Completed

    Public Shared Function norm(ByVal mean As Decimal, ByVal std As Decimal) As Double
        Randomize()
        Return Math.Sqrt(-2 * Math.Log(Rnd())) * Math.Cos(2 * Math.PI * Rnd()) * std + mean
    End Function  'Used to Create random normal numbers using the box-muller transform

End Class