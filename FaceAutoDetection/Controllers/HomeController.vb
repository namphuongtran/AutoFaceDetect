Imports System.IO

Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function

    Function About() As ActionResult
        ViewData("Message") = "Your application description page."

        Return View()
    End Function

    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."

        Return View()
    End Function

    <HttpPost>
    Public Function UploadPhoto() As JsonResult
        Dim message As String = Nothing
        Dim location As String = Nothing
        Dim fileName As String = Nothing
        Dim downloadUrl As String = Nothing
        Dim [error] As String = Nothing
        Dim success As String = Nothing

        Dim allowedFileSize As Double = Utility.AllowedPhotoFileSize ' In Megabytes
        Dim uploadedFiles As New List(Of Object)()
        Dim fileCount As Integer = Request.Files.Count
        Dim temporaryUploadFolderPath As String = String.Empty
        Dim folderReady As Boolean = True

        If fileCount > 0 Then
            Try
                temporaryUploadFolderPath = Path.Combine(Server.MapPath("~"), Utility.PhotoFolder)
                If Not Directory.Exists(temporaryUploadFolderPath) Then
                    Directory.CreateDirectory(temporaryUploadFolderPath)
                End If
            Catch ex As Exception
                folderReady = False
                message = "AN ERROR HAS OCCURED [DETAIL]: " + ex.Message
                Logger.WriteLog(ex.Message)
            End Try

            If folderReady Then
                For i As Integer = 0 To fileCount - 1
                    downloadUrl = Nothing
                    fileName = Nothing
                    location = Nothing
                    [error] = Nothing
                    success = Nothing

                    Dim file As HttpPostedFileBase = Request.Files(i)
                    'HttpPostedFileBase file = Request.Files[0];
                    Dim fileSize As Long = file.InputStream.Length / (1024 * 1024)
                    ' In Megabytes
                    fileName = file.FileName

                    Dim fileExtension As String = Path.GetExtension(file.FileName)

                    Dim validFileExtension As Boolean = Utility.PhotoExtensions.Any(Function(ext)
                                                                                        If Not ext.StartsWith(".") Then
                                                                                            ext = "." + ext
                                                                                        End If
                                                                                        Return String.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase)

                                                                                    End Function)
                    If Not validFileExtension Then
                        [error] = [String].Format("THE UPLOADED FILE EXTENSION NOT ALLOW", fileExtension)
                    ElseIf allowedFileSize > 0 AndAlso allowedFileSize < fileSize Then
                        ' If uploaded file's size is too long
                        [error] = String.Format("NEWS UPLOADED FILE TOO BIG SELECT MALLER FILE", allowedFileSize)
                    Else
                        Try
                            fileName = String.Concat(Utility.RemoveDiacriticals(Path.GetFileNameWithoutExtension(file.FileName)), "_", DateTime.Now.ToString("yyyyMMddHHmmss"), fileExtension)

                            Dim strFileLocation As String = Path.Combine(temporaryUploadFolderPath, fileName)

                            file.SaveAs(strFileLocation)

                            location = Path.Combine(Utility.PhotoFolder, fileName).ToLower().Replace("\"c, "/"c)

                            Dim id As String = Convert.ToBase64String(Encoding.UTF8.GetBytes((location)))

                            downloadUrl = String.Format("/Download.ashx?id={0}", id)

                            success = String.Format("FILE WAS UPLOADED SUCCESSFULLY {0}", file.FileName)
                        Catch ex As IOException
                            [error] = "AN_ERROR_HAS_OCCURED" + " [DETAIL]: " + ex.Message
                        End Try
                    End If

                    uploadedFiles.Add(New With {
                        Key .location = location,
                        Key .fileName = fileName,
                        Key .url = downloadUrl,
                        Key .[error] = [error],
                        Key .success = success
                    })
                Next
            End If
        Else
            message = "NOT_FOUND_ANY_FILE_TO_UPLOAD"
        End If

        Return Json(New With {
            Key .files = uploadedFiles,
            Key .errorMessage = message
        })

    End Function

    <HttpPost>
    Function SaveImageCropped(imageData As String) As JsonResult
        Dim fileName As String
        Dim message As String
        Dim temporaryUploadFolderPath As String = String.Empty
        Dim folderReady As Boolean = True
        Dim isSuccess As Boolean = False

        If Not String.IsNullOrEmpty(imageData) Then
            Try
                temporaryUploadFolderPath = Path.Combine(Server.MapPath("~"), Utility.PhotoFolder)
                If Not Directory.Exists(temporaryUploadFolderPath) Then
                    Directory.CreateDirectory(temporaryUploadFolderPath)
                End If
            Catch ex As Exception
                folderReady = False
                message = "AN ERROR HAS OCCURED [DETAIL]: " + ex.Message
                Logger.WriteLog(ex.Message)
            End Try

            Dim fileExtension As String = ".png"
            fileName = String.Concat("CroppedImage_", DateTime.Now.ToString("yyyyMMddHHmmss"), fileExtension)
            Dim strFileLocation As String = Path.Combine(temporaryUploadFolderPath, fileName)

            Using fs As New FileStream(strFileLocation, FileMode.Create)
                Using bw As New BinaryWriter(fs)
                    Dim data As Byte() = Convert.FromBase64String(imageData)
                    bw.Write(data)
                    bw.Close()
                    isSuccess = True
                End Using
				fs.Close()
            End Using
        End If

        Return Json(New With {Key .isSuccess = isSuccess})

    End Function


End Class
