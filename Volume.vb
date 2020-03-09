Imports NeuralDot

Public Class Volume
    Implements Tensor

    Public ReadOnly values As New List(Of Matrix)
    Public ReadOnly shape As Tuple(Of Integer, Integer)
    'The values variable is used to store all the elements of the Volume using matrices where each matrix repreents a layer
    'The shape variable denotes the shape of the Volume in the form = (height, width)

    'Constructors
    Public Sub New(ByVal initial_values As List(Of Matrix))
        values = initial_values
        shape = New Tuple(Of Integer, Integer)(initial_values(0).getshape(0), initial_values(0).getshape(1))
    End Sub 'This constructor is used to initalise the matrix, with initial values

    Public Sub New(ByVal h As Integer, ByVal w As Integer, ByVal d As Integer, Optional ByVal val As Double = 0)
        For i As Integer = 1 To d
            values.Add(New Matrix(h, w, val))
        Next
        shape = New Tuple(Of Integer, Integer)(h, w)
    End Sub 'Constructor creates a volume of shape = (h, w, d) with all values set to "val"

    Public Sub New(ByVal h As Integer, ByVal w As Integer, ByVal d As Integer, ByVal mean As Double, ByVal std As Double)
        For i As Integer = 1 To d
            values.Add(New Matrix(h, w, mean, std))
        Next
        shape = New Tuple(Of Integer, Integer)(h, w)
    End Sub 'Constructor creates a volume of shape = (h, w, d) with all values normally distributes with a set mean and std

    'All Constructors defined

    Public Shared Function arange(ByVal h As Integer, ByVal w As Integer, ByVal d As Integer) As Volume
        Dim result As New List(Of Matrix)
        For k As Integer = 1 To d
            result.Add(Matrix.arange(h, w) + (k - 1) * (h * w))
        Next
        Return New Volume(result)
    End Function 'COMPLETED -- Returns a Volume that has all the values in an incrementing order. Useful for debugging

    'Accessors

    Public Function getshape() As List(Of Integer) Implements Tensor.getshape
        Return New List(Of Integer)({Me.shape.Item1, Me.shape.Item2, Me.values.Count})
    End Function 'COMPLETED -- Function returns the shape of the Volume in the form (height, width, depth)

    Public Overloads Sub print() Implements Tensor.print
        For Each M In Items(Me)
            M.print()
            Console.WriteLine("")
        Next
    End Sub 'COMPLETED -- Function prints out all the elements in the Volume

    Public Property item(ByVal i As Integer, ByVal j As Integer, ByVal k As Integer) As Double
        Get
            Return Me.values(k).item(i, j)
        End Get
        Set(value As Double)
            Me.values(k).item(i, j) = value
        End Set
    End Property 'COMPLETED -- Property used to select and set element (i, j, k) in a volume

    Public Property split(ByVal i_start As Integer, ByVal i_end As Integer, ByVal j_start As Integer, ByVal j_end As Integer, ByVal k As Integer) As Matrix
        Get
            Dim result As New Matrix(i_end - i_start + 1, j_end - j_start + 1)
            For i As Integer = i_start To i_end
                For j As Integer = j_start To j_end
                    result.item(i - i_start + 1, j - j_start + 1) = Me.item(i, j, k)
                Next
            Next
            Return result
        End Get
        Set(ByVal value As Matrix)
            For i As Integer = i_start To i_end
                For j As Integer = j_start To j_end
                    Me.item(i, j, k) = value.item(i - i_start + 1, j - j_start + 1)
                Next
            Next
        End Set
    End Property 'COMPLETED -- Property used to set/select a portion of a volume

    Public ReadOnly Property split(ByVal i_start As Integer, ByVal i_end As Integer, ByVal j_start As Integer, ByVal j_end As Integer) As Volume
        Get
            Dim result As New List(Of Matrix)
            For Each M In Items(Me)
                result.Add(M.item(i_start, i_end, j_start, j_end))
            Next
            Return New Volume(result)
        End Get
    End Property 'COMPLETED -- Property used to return a Volume of shape(i_start - i_end + 1, j_start - j_end + 1, k). All values in the volume
    'correspond to an item in the indexing volume

    'Volume Operations

    Public Function rotate(ByVal theta As Integer) As Volume
        If theta = 0 Then
            Return Me 'If theta is 0, then return identity
        Else
            Return op(AddressOf Matrix.rotate, Me).rotate(theta - 1) 'Else rotate each layer in the Volume, and then rotate by (theta-1)*90 degrees
        End If
    End Function 'COMPLETED -- Function rotates the Volume by theta * 90 degrees

    Public Shared Function j_rotate(ByVal v As Volume) As Volume
        Dim result As New Volume(v.values.Count, v.shape.Item2, v.shape.Item1)
        For k As Integer = 0 To v.shape.Item1 - 1
            For i As Integer = 1 To v.values.Count
                For j As Integer = 1 To v.shape.Item2
                    result.item(i, j, k) = v.item(k + 1, j, i - 1)
                Next
            Next
        Next
        Return result
    End Function 'COMPLETED -- Rotates in the j direction by 90 degrees

    Public Function normalize(Optional ByVal mean As Double = 0, Optional ByVal std As Double = 1) As Tensor Implements Tensor.normalize
        Dim n As Integer = Me.shape.Item1 * Me.shape.Item2 * Me.values.Count
        Dim means As Double = Me.values.Select(Function(x) Matrix.sum(x).item(1, 1)).Sum / n
        Dim stds As Double = Math.Sqrt((Me.values.Select(Function(x) Matrix.sum(x * x).item(1, 1)).Sum - (mean * mean * n)) / (n - 1))
        Dim result As New List(Of Matrix)
        For Each M In Volume.Items(Me)
            result.Add((M - means) / stds)
        Next
        Return New Volume(result)
    End Function  'COMPLETED -- Returns a volume whose layers are normlised using all the elements in the volume


    Public Function mean(ByVal axis As Integer) As Matrix
        Dim result As New Matrix(Me.shape.Item1, Me.shape.Item2)
        If axis = 2 Then
            For Each M In Me.values
                result += M
            Next
            Return result / Me.values.Count
        Else
            Throw New System.Exception("axis 1 or 2 has not yet been implemented yet for Volume")
        End If
    End Function 'COMPLETED -- Returns the mean of a volume in a specified dimension. Currently, only works for axis = 2

    Public Function transpose() As Volume
        Return op(AddressOf Matrix.transpose, Me)
    End Function 'COMPLETED -- Function returns the transpose of each element in the matrix

    Public Sub transposeSelf() Implements Tensor.transposeSelf
        For Each m In values
            m.transposeSelf()
        Next
    End Sub ' COMPLETED -- Sub routine transposes each layer in the matrix

    Public Function clone() As Tensor Implements Tensor.clone
        Dim cloned As New List(Of Matrix)
        For Each m In Items(Me)
            cloned.Add(m)
        Next
        Return New Volume(cloned)
    End Function 'COMPLETED -- Function returns a clone of the current volume

    'All Volume Operations Defined

    'Joint Operations

    Public Shared Operator +(ByVal x As Volume, ByVal y As Volume) As Volume
        Return op(x, AddressOf Matrix.add, y)
    End Operator 'COMPLETED -- Adds two Volumes together elementwise

    Public Shared Operator +(ByVal x As Volume, ByVal y As Decimal) As Volume
        Return op(x, AddressOf Matrix.add, y)
    End Operator 'COMPLETED -- Adds a scaler to a volume elementwise

    Public Shared Operator +(ByVal y As Decimal, ByVal x As Volume) As Volume
        Return op(x, AddressOf Matrix.add, y)
    End Operator 'COMPLETED -- Adds a scaler to a Volume elementwise

    Public Shared Operator -(ByVal x As Volume, ByVal y As Volume) As Volume
        Return op(x, AddressOf Matrix.add, -1 * y)
    End Operator 'COMPLETED -- Subtracts a Volume by a scaler

    Public Shared Operator -(ByVal x As Volume, ByVal y As Decimal) As Volume
        Return op(x, AddressOf Matrix.add, -y)
    End Operator 'COMPLETED -- Subtracts a Volume by a scaler

    Public Shared Operator -(ByVal y As Decimal, ByVal x As Volume) As Volume
        Return y + (-1 * x)
    End Operator 'COMPLETED -- Subtracts a Volume by a scaler

    Public Shared Operator *(ByVal x As Volume, ByVal y As Volume) As Volume
        Return op(x, AddressOf Matrix.multiply, y)
    End Operator 'COMPLETED -- Applies elementwise multiplication on two Volumes 

    Public Shared Operator *(ByVal x As Volume, ByVal y As Decimal) As Volume
        Return op(x, AddressOf Matrix.multiply, New Volume(Items(x)(0).getshape(0), Items(x)(0).getshape(1), x.values.Count + 1, y))
    End Operator 'COMPLETED -- Applies elementwise multiplication between a scaler and a Volume

    Public Shared Operator *(ByVal x As Decimal, ByVal y As Volume) As Volume
        Return y * x
    End Operator 'COMPLETED -- Applies elementwise multiplication between a scaler and a Volume

    Public Shared Operator /(ByVal x As Volume, ByVal y As Double) As Volume
        Return x * (1 / y)
    End Operator 'COMPLETED -- Divides each element of a Volume, i.e v_ij = v_ij / y

    Public Shared Function conv2d(ByVal v As Volume, ByVal kernels As Volume, Optional stridesx As Integer = 1, Optional stridesy As Integer = 1, Optional padding As String = "valid") As Volume
        'conv2d is applying a convolution using in 2 dimensions. This means that every 2d kernel is applied to every layer in the volume.
        Dim result_values As New List(Of Matrix) : Dim all_channels As List(Of Matrix) = Items(v).ToList

        For Each k In Items(kernels)
            Dim temp As Matrix = Matrix.conv(all_channels(0), k, stridesx, stridesy, padding)
            For Each M In all_channels.GetRange(1, all_channels.Count - 1)
                temp += Matrix.conv(M, k, stridesx, stridesy, padding) 'Summing up the result of all the convolutions for that particular kernel
            Next
            result_values.Add(temp)
        Next
        Return New Volume(result_values)
    End Function 'COMPLETED -- Applies 2d convolution

    Public Shared Function maxpool(ByVal filter As Volume, ByVal kernely As Integer, ByVal kernelx As Integer, ByVal stridesy As Integer, ByVal stridesx As Integer) As Volume
        Dim result As New List(Of Matrix)
        For Each M In Items(filter)
            result.Add(M.maxpool(kernelx, kernely, stridesx, stridesy))
        Next
        Return New Volume(result)
    End Function 'COMPLETED -- Returns the maxpooling of a volume using a kernel of shape (kernelx, kernely) and step size = (stridesx, stridesy)

    Public Shared Function op(ByVal x As Volume, ByVal f As Func(Of Matrix, Matrix, Matrix), ByVal y As Volume) As Volume
        Dim result As New List(Of Matrix)
        For i As Integer = 0 To x.values.Count - 1
            result.Add(f.Invoke(x.values(i), y.values(i)))
        Next
        Return New Volume(result)
    End Function 'COMPLETED -- Applies a function f(Matrix, Matrix) -> Matrix, to all the layers in the Volumes x and y

    Public Shared Function op(ByVal v As Volume, ByVal f As Func(Of Matrix, Matrix, Matrix), ByVal m As Matrix) As Volume
        Dim result As New Volume(v.shape.Item1, v.shape.Item2, 0)
        For Each x In Items(v)
            result.values.Add(f.Invoke(x, m))
        Next
        Return result
    End Function 'COMPLETED -- Function applies a function "f" to a Volume "x" and "y" layer-wise

    Public Shared Function op(ByVal m As Matrix, ByVal f As Func(Of Matrix, Matrix, Matrix), ByVal v As Volume) As Volume
        Return op(v, f, m)
    End Function 'COMPLETED -- Function applies a function "f" to a Volume "x" and "y" layer-wise

    Public Shared Function op(ByVal v As Volume, ByVal f As Func(Of Matrix, Double, Matrix), ByVal y As Double) As Volume
        Dim result As New Volume(v.shape.Item1, v.shape.Item2, 0)
        For Each x In Items(v)
            result.values.Add(f.Invoke(x, y))
        Next
        Return result
    End Function 'COMPLETED -- Function applies a function "f(v,y)" to a volume "v" and a double "y"

    Public Shared Function op(ByVal y As Double, ByVal f As Func(Of Matrix, Double, Matrix), ByVal v As Volume) As Volume
        Return Volume.op(v, f, y)
    End Function 'COMPLETED -- Function applies a function "f(v,y)" to a volume "v" and a double "y"

    Public Shared Function op(ByVal f As Func(Of Matrix, Matrix), ByVal v As Volume) As Volume
        Dim result As New Volume(v.shape.Item1, v.shape.Item2, 0)
        For Each x In Items(v)
            result.values.Add(f.Invoke(x))
        Next
        Return result
    End Function 'COMPLETED -- Applies a function layer-wise to a Volume

    Public Shared Function op(ByVal f As Func(Of Double, Double), ByVal v As Volume) As Volume
        Dim result As New Volume(v.shape.Item1, v.shape.Item2, 0)
        For Each m In Items(v)
            result.values.Add(Matrix.op(f, m))
        Next
        Return result
    End Function 'COMPLETED -- Applies a function elementwise to a Volume
    'All Joint Operations Defined

    'Iterators
    Public Overloads Shared Iterator Function Items(ByVal v As Volume) As IEnumerable(Of Matrix)
        For Each M In v.values
            Yield M
        Next
    End Function 'COMPLETED -- Returns an IEnumerable of all the layers in the Volume

    Public Overloads Iterator Function Items() As IEnumerable(Of Tensor)
        For Each Matrix In Me.values
            Yield Matrix
        Next
    End Function 'COMPLETED -- Returns an IEnumerable of all the layers in the Volume

    Public Shared Iterator Function subvolume(ByVal v As Volume, ByVal kernelx As Integer, ByVal kernely As Integer, ByVal stridesx As Integer, ByVal stridesy As Integer) As IEnumerable(Of Volume)
        For i As Integer = 1 To v.shape.Item1 - kernely + 1 Step stridesy
            For j As Integer = 1 To v.shape.Item2 - kernelx + 1 Step stridesx
                Yield v.split(i, i + kernely - 1, j, j + kernelx - 1)
            Next
        Next
    End Function 'COMPLETED -- Returns a collection of volumes, by sliding a window of length (kernelx, kernely) and using strides = (stridesx, stridesy) on each individual layer
    ' All Iterators defined

    'Casts
    Public Shared Function cast(ByVal matrixList As List(Of Matrix), ByVal rows As Integer, ByVal cols As Integer) As List(Of Volume)
        Dim l_v As New List(Of Volume)
        For Each M In matrixList
            l_v.Add(Volume.cast(M, rows, cols))
        Next
        Return l_v
    End Function 'COMPLETED -- Casts a list of matrices into volumes elementwise

    Public Shared Function cast(ByVal v As Volume, ByVal rows As Integer, ByVal cols As Integer) As Matrix
        If rows * cols <> v.shape.Item1 * v.shape.Item2 * v.values.Count Then
            Throw New System.Exception("Dimensions for matrix must only be sufficient to store all the items in the Volume")
        End If
        Dim result As New Matrix(rows, cols)
        Dim i As Integer = 0
        For Each M In Volume.Items(v)
            For Each k In Matrix.val(M)
                result.item((Math.Truncate(i / result.getshape(1)) + 1), (i Mod result.getshape(1)) + 1) = k 'Assigning each element in result with 
                'its corresponding value in the Volume "v"
                i += 1
            Next
        Next
        Return result
    End Function 'COMPLETED -- Function used to cast a Volume into a matrix of shape = (rows, cols)

    Public Shared Function cast(ByVal m As Matrix, ByVal rows As Integer, ByVal cols As Integer) As Volume
        Dim result As New Volume(rows, cols, m.getshape(0) * m.getshape(1) / (rows * cols))
        Dim i As Integer = 0
        For Each d In Matrix.val(m)
            result.item(((Math.Truncate(i / (cols))) Mod rows) + 1, (i Mod (cols)) + 1, Math.Truncate(i / ((cols * rows)))) = d
            i += 1
        Next
        Return result
    End Function 'COMPLETED -- Function casts a matrix into a volume of shape = (rows, cols)
    'All Casts defined
    'Volume Class Completed

End Class
