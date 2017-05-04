Imports System.Web.Optimization

Public Module BundleConfig
    ' For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    Public Sub RegisterBundles(ByVal bundles As BundleCollection)

        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-1.11.1.min.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"))

        ' Use the development version of Modernizr to develop with and learn from. Then, when you're
        ' ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"))

        bundles.Add(New ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js",
                  "~/Scripts/respond.js"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/cropper.css",
                  "~/Content/site.css"))

        bundles.Add(New ScriptBundle("~/bundles/fileupload").Include(
                  "~/Scripts/jquery.ui.widget.js",
                  "~/Scripts/jquery.fileupload.js",
                  "~/Scripts/jquery.iframe-transport.js"))

        bundles.Add(New ScriptBundle("~/bundles/detect").Include(
                  "~/Scripts/jquery.facedetection.js",
                  "~/Scripts/ccv.js",
                  "~/Scripts/cascade.js"))

        bundles.Add(New ScriptBundle("~/bundles/cropper").Include(
                  "~/Scripts/cropper.js"))

        bundles.Add(New ScriptBundle("~/bundles/tmpl").Include(
                  "~/Scripts/jquery.tmpl.js"))

        bundles.Add(New ScriptBundle("~/bundles/main").Include(
                  "~/Scripts/main.js"))
    End Sub
End Module

