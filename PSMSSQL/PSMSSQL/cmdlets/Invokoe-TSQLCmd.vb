<System.Management.Automation.Cmdlet("Invoke", "TSQLCmd")>
Public Class Invoke_TSQLCmd
    Inherits System.Management.Automation.PSCmdlet
    <System.Management.Automation.Parameter(Position:=0, Mandatory:=True)>
    Public Property TSQLCommand As String

    <System.Management.Automation.Parameter(Position:=1, Mandatory:=True)>
    Public Property Server As String

    <System.Management.Automation.Parameter(Position:=2, Mandatory:=False)>
    Public Property TestTSQLCommand As String

    <System.Management.Automation.Parameter(Position:=3, Mandatory:=False)>
    Public Property JSONOutput As System.Management.Automation.SwitchParameter
    Protected Overrides Sub ProcessRecord()

        Dim batches = get_batches(Me.TSQLCommand)
        Dim tests = get_batches(Me.TestTSQLCommand)

        Dim change_requested As Boolean = True

        Dim conn As New SqlClient.SqlConnection(String.Format("Data Source={0};Integrated Security=True", Server))

        If tests.Count > 0 Then

            Dim cmd As SqlClient.SqlCommand = Nothing

            conn.Open()

            change_requested = False

            For Each test In tests

                cmd = New SqlClient.SqlCommand(test, conn)

                Dim change As Boolean = Convert.ToBoolean(cmd.ExecuteScalar(cmd))

                If change Then
                    change_requested = True
                    Exit For
                End If

            Next

            conn.Close()

        End If

        If batches.Count > 0 AndAlso change_requested Then

            Dim cmd As SqlClient.SqlCommand = Nothing

            conn.Open()

            For Each sql_cmd In batches

                cmd = New SqlClient.SqlCommand(sql_cmd, conn)
                cmd.CommandTimeout = 1000 * 60 * 5

                cmd.ExecuteNonQuery()

            Next

            conn.Close()

        End If

        If JSONOutput.IsPresent Then
            Dim output As String = "{ ""changed"" : " & change_requested.ToString().ToLower() & ", ""comment"" : """"}"
            Me.WriteObject(output)
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