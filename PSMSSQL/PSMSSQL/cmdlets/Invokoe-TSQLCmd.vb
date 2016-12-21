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

        For Each scr In tests
            Me.WriteVerbose("/*TEST Start*/" & Environment.NewLine & scr & Environment.NewLine & "/*TEST End*/")
        Next
        For Each scr In batches
            Me.WriteVerbose("/*BATCH Start*/" & Environment.NewLine & scr & Environment.NewLine & "/*BATCH End*/")
        Next

        Dim change_requested As Boolean = True

        Dim conn As New SqlClient.SqlConnection(String.Format("Data Source={0};Integrated Security=True", Server))

        If tests.Count > 0 Then

            Dim cmd As SqlClient.SqlCommand = Nothing

            conn.Open()

            change_requested = False

            For Each test In tests

                cmd = New SqlClient.SqlCommand(test, conn)

                Dim change As Boolean = Convert.ToBoolean(cmd.ExecuteScalar())

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

        For Each item In System.Text.RegularExpressions.Regex.Split(TSQLCmd, "^\s*GO\s*$", Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Multiline)


            If Not String.IsNullOrEmpty(item) Then
                res.Add(item)
            End If

        Next

        Return res

    End Function
End Class