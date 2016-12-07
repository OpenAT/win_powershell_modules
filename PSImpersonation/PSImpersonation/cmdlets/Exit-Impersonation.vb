
<System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Exit, "Impersonation")>
Public Class Exit_Impersonation
    Inherits System.Management.Automation.PSCmdlet

    Protected Overrides Sub ProcessRecord()

        If globals.current_imp_scop Is Nothing Then
            Me.WriteObject("no active impersonation context available")
        Else
            globals.current_imp_scop.Dispose()
            globals.current_imp_scop = Nothing
            Me.WriteObject("impersonation exitet, switched back to origin user")
        End If

    End Sub

End Class