<System.Management.Automation.Cmdlet("Invoke", "TSQLCmd")>
Public Class Invoke_TSQLCmd
    Inherits System.Management.Automation.PSCmdlet
    <System.Management.Automation.Parameter(Position:=0, Mandatory:=True)>
    Public Property TSQLCommand As String

    <System.Management.Automation.Parameter(Position:=1, Mandatory:=True)>
    Public Property Server As String
    Protected Overrides Sub ProcessRecord()

        Dim batches = get_batches(Me.TSQLCommand)

        If batches.Count > 0 Then

            Dim conn As New SqlClient.SqlConnection(String.Format("Data Source={0};Integrated Security=True", Server))

            Dim cmd As SqlClient.SqlCommand = Nothing

            conn.Open()

            For Each sql_cmd In batches

                cmd = New SqlClient.SqlCommand(sql_cmd, conn)
                cmd.CommandTimeout = 1000 * 60 * 5

                cmd.ExecuteNonQuery()

            Next

            conn.Close()

        End If

    End Sub

    Private Function get_batches(TSQLCmd As String) As List(Of String)

        Dim res As New List(Of String)

        Dim current_batch As String = ""

        For Each item In TSQLCmd.Split(Environment.NewLine)

            If item.Trim().ToLower() = "go" Then
                res.Add(current_batch)
                current_batch = ""
            Else
                current_batch &= item
            End If

        Next

        If Not String.IsNullOrEmpty(current_batch) Then
            res.Add(current_batch)
        End If

        For i As Integer = res.Count - 1 To 0 Step -1
            Dim item = res(i)
            If String.IsNullOrEmpty(item) Then
                res.RemoveAt(i)
            End If
        Next

        Return res

    End Function
End Class