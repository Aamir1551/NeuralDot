Imports NeuralDot

Public Class netData
    'This class is responsible for setting up the data to be used by the network and making it easier for the user to set up the training data
    Public ReadOnly data As IEnumerable(Of Tuple(Of Tensor, Tensor))
    Public ReadOnly xdata, ydata As IEnumerable(Of Tensor)

    'The data variable will store the x and y data as a tuple
    'The xdata and ydata variables will store the inputs and the corresponding outputs, respectively

    Public Sub New(ByVal datax As IEnumerable(Of Tensor), ByVal datay As IEnumerable(Of Tensor))
        data = join(datax, datay)
        xdata = datax : ydata = datay
    End Sub

    Public Shared Function join(ByVal datax As IEnumerable(Of Tensor), ByVal datay As IEnumerable(Of Tensor)) As IEnumerable(Of Tuple(Of Tensor, Tensor))
        Dim data As IEnumerable(Of Tuple(Of Tensor, Tensor)) = datax.Zip(datay, Function(first, second) New Tuple(Of Tensor, Tensor)(first, second))
        Return data
    End Function 'This function returns the xdata and ydata after being joined together

    Public Function oneHot(ByVal numClasses As Integer) As IEnumerable(Of Matrix)
        Dim encoded_y As IEnumerable(Of Matrix) = ydata.Select(Function(X) DirectCast(X, Matrix).oneHot(numClasses)) 'Applying oneHot to all items in the ydata list
        Return encoded_y
    End Function 'This function oneHots the ydata. Useful for data-manipulation

    Public Function normalise(ByVal mean As Double, ByVal std As Double) As netData
        Dim normed As IEnumerable(Of Tensor) = Me.xdata.Select(Function(x) x.normalize(mean, std)) 'This line normalises each x data point
        Return New netData(normed, ydata)
    End Function 'This function normalises the xdata, removing the need for the user to do it.

    Public Shared Function toConvNetData(ByVal xfileName As String, ByVal yfileName As String, ByVal rows As Integer, ByVal cols As Integer) As netData
        Dim xdata As IEnumerable(Of Tensor) = Volume.cast(toMatrix(xfileName), rows, cols)
        Dim ydata As IEnumerable(Of Tensor) = toMatrix(yfileName)
        Return New netData(xdata, ydata)
    End Function 'This function extracts the file that stores the xdata and stores it as a list of Volumes. The ydata is stored as a list of matrices

    Public Shared Function toMatrix(ByVal fileName As String) As List(Of Matrix)
        Dim data As New List(Of Matrix)
        Using myreader As New Microsoft.VisualBasic.FileIO.TextFieldParser(fileName) 'Using streamreader
            myreader.TextFieldType = FileIO.FieldType.Delimited : myreader.SetDelimiters(",")
            Dim currentrow() As Double
            While Not myreader.EndOfData
                currentrow = Array.ConvertAll(myreader.ReadFields().ToArray, Function(x) Double.Parse(x)).ToArray()
                Dim xval(currentrow.Count - 1, 0) As Double
                Dim i As Integer = 0
                For Each item In currentrow : xval(i, 0) = item : i += 1 : Next
                data.Add(New Matrix(xval))
            End While
        End Using
        Return Volume.j_rotate((New Volume(data))).values
    End Function
    'This functon extracts data stored in a csv file and stores the data in a matrix.

End Class
