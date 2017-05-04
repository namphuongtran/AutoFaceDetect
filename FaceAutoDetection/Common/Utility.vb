Public Class Utility
    Public Shared ReadOnly Property PhotoExtensions() As String()
        Get
            Return ConfigurationManager.AppSettings("PhotoExtensions").ToLower().Split(","c)
        End Get
    End Property

    Public Shared ReadOnly Property AllowedPhotoFileSize() As Double
        Get
            Dim len As Double = 0
            If Double.TryParse(ConfigurationManager.AppSettings("PhotoFileSize"), len) Then
                Return len
            End If
            Return len
        End Get
    End Property

    Public Shared ReadOnly Property TemporaryUploadFolder() As String
        Get
            Return Hosting.HostingEnvironment.MapPath("~/TemporaryUploadFolder")
        End Get
    End Property

    Public Shared Function RemoveDiacriticals(fileName As String) As String
        Dim nfd As String = fileName.Normalize(NormalizationForm.FormD)
        Dim retval As New StringBuilder(nfd.Length)
        For Each ch As Char In nfd
            If (ch >= "\u0300" AndAlso ch <= "\u036f") OrElse (ch >= "\u1dc0" AndAlso ch <= "\u1de6") OrElse (ch >= "\ufe20" AndAlso ch <= "\ufe26") OrElse (ch >= "\u20d0" AndAlso ch <= "\u20f0") Then
                Continue For
            End If

            retval.Append(ch)
        Next
        Return retval.ToString()
    End Function

    Public Shared ReadOnly Property PhotoFolder() As String
        Get
            Return ConfigurationManager.AppSettings("PhotoPath")
        End Get
    End Property
End Class
