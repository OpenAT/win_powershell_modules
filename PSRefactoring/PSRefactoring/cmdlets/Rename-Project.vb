<System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Rename, "Project")>
Public Class Rename_Project
    Inherits System.Management.Automation.PSCmdlet
    <System.Management.Automation.Parameter(Position:=0, Mandatory:=True)>
    Public Property Path As String

    <System.Management.Automation.Parameter(Position:=1, Mandatory:=True)>
    Public Property NewName As String
    Protected Overrides Sub ProcessRecord()

        If Not System.IO.Directory.Exists(Path) Then
            Me.WriteObject("path is not valid")
            Exit Sub
        End If

        ren()


    End Sub

    Public Sub ren()
        Dim dirinfo = New System.IO.DirectoryInfo(Path)

        Dim old_name As String = dirinfo.Name

        rename_rec(Path, old_name, NewName)

    End Sub

    Private Sub rename_rec(dirname As String, old_name As String, new_name As String)

        For Each subdir In System.IO.Directory.GetDirectories(dirname)
            Dim dir_info As New System.IO.DirectoryInfo(subdir)
            If Not dir_info.Name.StartsWith(".") Then
                rename_rec(subdir, old_name, new_name)
            End If
        Next

        For Each file In System.IO.Directory.GetFiles(dirname)

            replace_content(file, old_name, new_name)

            Dim fileinfo As New System.IO.FileInfo(file)

            Dim new_file_name As String = fileinfo.Name.Replace(old_name, new_name)

            System.IO.File.Move(file, fileinfo.DirectoryName & "\" & new_file_name)


        Next

        Dim mdir_info As New System.IO.DirectoryInfo(dirname)

        If mdir_info.Name.Contains(old_name) Then
            Dim new_dir_name As String = mdir_info.Name.Replace(old_name, new_name)

            System.IO.Directory.Move(dirname, mdir_info.Parent.FullName & "\" & new_dir_name)
        End If
    End Sub

    Private Sub replace_content(file As String, old_char As String, new_char As String)

        Dim content As String

        Using str As New System.IO.StreamReader(file)

            content = str.ReadToEnd()

        End Using

        content = content.Replace(old_char, new_char)

        Using stw As New System.IO.StreamWriter(file, False)

            stw.Write(content)

        End Using

    End Sub

End Class