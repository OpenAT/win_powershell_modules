<System.Management.Automation.Cmdlet("Merge", "Directories")>
Public Class Merge_Directories
    Inherits System.Management.Automation.PSCmdlet
    <System.Management.Automation.Parameter(Position:=0, Mandatory:=True)>
    Public Property Source As String

    <System.Management.Automation.Parameter(Position:=1, Mandatory:=True)>
    Public Property Target As String
    Protected Overrides Sub ProcessRecord()


        If System.IO.Directory.Exists(Source) AndAlso System.IO.Directory.Exists(Target) Then

            merge_rec(Source, Target, "")

        End If

    End Sub
    Private Sub merge_rec(source As String, target As String, current_sub_dir As String)

        For Each file In System.IO.Directory.GetFiles(source & "\" & current_sub_dir)

            Dim fileinfo As New System.IO.FileInfo(file)

            System.IO.File.Copy(file, target & "\" & current_sub_dir & fileinfo.Name, True)

        Next

        For Each subdir In System.IO.Directory.GetDirectories(source & "\" & current_sub_dir)

            Dim dirinfo As New System.IO.DirectoryInfo(subdir)

            merge_rec(source, target, dirinfo.Name & "\")

        Next

    End Sub

End Class