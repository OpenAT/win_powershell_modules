<System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Enter, "Impersonation")>
Public Class Enter_Impersonation
    Inherits System.Management.Automation.PSCmdlet
    <System.Management.Automation.Parameter(Position:=0, Mandatory:=True)>
    Public Property user As String

    <System.Management.Automation.Parameter(Position:=1, Mandatory:=True)>
    Public Property password As String
    Protected Overrides Sub ProcessRecord()

        If globals.current_imp_scop IsNot Nothing Then
            Me.WriteObject("already in an active impersonation context")
        Else
            Try
                globals.current_imp_scop = New dadi_impersonation.ImpersonationScope(user, password)
                Me.WriteObject(String.Format("user switched, current identity: {0}", Me.user))
            Catch ex As Exception
                globals.current_imp_scop = Nothing
                Me.WriteObject("error entering impersonation context")
            End Try
        End If

    End Sub

End Class