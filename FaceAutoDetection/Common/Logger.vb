Imports System.IO

Public Class Logger
    Public Shared Sub WriteLog(message As String)
        Dim sw As StreamWriter = Nothing
        Try
            sw = New StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\FAD.txt", True)
            sw.WriteLine(Convert.ToString(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": ") & message)
            sw.Flush()
            sw.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
