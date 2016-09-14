Public Class ApprenticeSample
    Private m_Apprentice As Inventor.ApprenticeServerComponent = Nothing

    Private Sub btnProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcess.Click
        lblStatus.Text = "Processing..."
        lblStatus.Refresh()

        Dim watch As New System.Diagnostics.Stopwatch
        watch.Start()

        Dim filename As String
        For Each filename In System.IO.Directory.GetFiles(Me.txtPath.Text, "*.ipt")
            ' Update the properties for this file.  If the update fails
            ' abort the processing.
            If Not UpdateProperties(filename) Then
                Exit Sub
            End If
        Next

        lblStatus.Text = "Elapsed time: " & watch.Elapsed.TotalSeconds & " seconds."
    End Sub

    Private Function UpdateProperties(ByVal InventorFilename As String) As Boolean
        ' Create an instance of Apprentice, if it hasn't already been created.
        If m_Apprentice Is Nothing Then
            Try
                m_Apprentice = New Inventor.ApprenticeServerComponent
            Catch ex As Exception
                MsgBox("Unable to start Apprentice.")

                ' Return False to stop further processing.
                Return False
            End Try
        End If

        ' Open the file in Apprentice.
        Dim apprenticeDoc As Inventor.ApprenticeServerDocument = Nothing
        Try
            apprenticeDoc = m_Apprentice.Open(InventorFilename)
        Catch ex As Exception
            MsgBox("Unable to open the file """ & InventorFilename & """ using Apprentice.")

            ' Return true so processing continues for the other files.
            Return True
        End Try

        ' Check to see if this document needs migration.
        If apprenticeDoc.NeedsMigrating Then
            MsgBox("This file, """ & InventorFilename & """ requires migration before it can be processed.")

            ' Return true so processing continues for the other files.
            Return True
        End If

        ' Get the "Company" property.
        Dim summaryPropSet As Inventor.PropertySet
        summaryPropSet = apprenticeDoc.PropertySets.Item("Inventor Document Summary Information")
        Dim companyProp As Inventor.Property
        companyProp = summaryPropSet.Item("Company")

        ' Change the value of the iProperty.
        companyProp.Value = "Widgets R Us"

        ' Update the cost property.
        Dim designTrackPropSet As Inventor.PropertySet
        designTrackPropSet = apprenticeDoc.PropertySets.Item("Design Tracking Properties")
        Dim costProp As Inventor.Property
        costProp = designTrackPropSet.Item("Cost")
        costProp.Value = costProp.Value * 1.2

        ' Save the changes.
        apprenticeDoc.PropertySets.FlushToFile()

        ' Close the document
        apprenticeDoc.Close()

        Return True
    End Function

    Private Sub txtPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPath.TextChanged
        lblStatus.Text = ""
    End Sub
End Class
