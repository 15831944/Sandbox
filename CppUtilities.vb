Imports System
Imports EnvDTE
Imports EnvDTE80
Imports EnvDTE90
Imports EnvDTE90a
Imports EnvDTE100
Imports System.Diagnostics

Public Module CppUtilities

    '=====================================================================
    ' If the currently open document is a CPP or an H file, attempts to
    ' switch between the CPP and the H file.
    '=====================================================================
    Public Sub SwitchBetweenSourceAndHeader()
        Dim currentDocument As String
        Dim targetDocument As String

        currentDocument = ActiveDocument.FullName

        If currentDocument.EndsWith(".cpp", StringComparison.InvariantCultureIgnoreCase) Then
            targetDocument = Left(currentDocument, Len(currentDocument) - 3) + "h"
            OpenDocument(targetDocument)
        ElseIf currentDocument.EndsWith(".h", StringComparison.InvariantCultureIgnoreCase) Then
            targetDocument = Left(currentDocument, Len(currentDocument) - 1) + "cpp"
            OpenDocument(targetDocument)
        End If

    End Sub

    '=====================================================================
    ' Given a document name, attempts to activate it if it is already open,
    ' otherwise attempts to open it.
    '=====================================================================
    Private Sub OpenDocument(ByRef documentName As String)
        Dim document As EnvDTE.Document
        Dim activatedTarget As Boolean
        activatedTarget = False

        For Each document In Application.Documents
            If document.FullName = documentName And document.Windows.Count > 0 Then
                document.Activate()
                activatedTarget = True
                Exit For
            End If
        Next
        If Not activatedTarget Then
            Application.Documents.Open(documentName, "Text")
        End If
    End Sub

End Module

Public Module ConditionalBreakPoint
    Sub AddBreakPoint()
        DTE.Debugger.Breakpoints.Add("f2")
    End Sub
End Module
