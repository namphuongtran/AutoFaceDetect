@Code
    ViewData("Title") = "Home Page"
End Code
<div class="wrapper">
    <div class="page-header">
        <h1>Hello,</h1>
        <h3>Please upload image file by click to button bellow</h3>
    </div>
    <div class="file-upload-section">
        <div class="btn-group" role="group">
            <span type="button" class="btn btn-default upload-file">
                <span class="glyphicon glyphicon-open"></span> |
                <span>Choose file</span>
                <input class="file-uploader" type="file" name="files[]" />
                <input type="hidden" id="allowedExtension" value="@(String.Join(";", Utility.PhotoExtensions))" />
                <span class="upload-progress"></span>
            </span>
        </div>
    </div>

    <div class="photo-section">
    </div>

    <script id="photoTemplate" type="text/x-jquery-tmpl">
        {{each photos}}
        <div class="photo-item" file-id="${$value.id}">
            <div class="row">
                <input type="hidden" class="hdLocation" value="${$value.location}" />
                <div class="col-md-6">
                    <div class="thumbnail">
                        <img src="${$value.location}" alt="" class="picture">
                    </div>
                </div>
                <div class="col-md-6">
                    <!-- <h3>Preview:</h3> -->
                    <div class="docs-preview clearfix">
                        <div class="img-preview preview-lg col-md-6"></div>
                        <div class="img-cropped preview-lg col-md-6"></div>
                    </div>
                </div>
                <div class="clearfix visible-md"></div>
            </div>
            <div class="row col-md-12">
                <div class="button-section text-center">
                    <a href="javascript:;" class="btn btn-primary btn-detect" role="button">Detect Face</a>
                    <a href="javascript:;" class="btn btn-default btn-crop" role="button">Crop</a>
                    <a href="javascript:;" class="btn btn-default btn-remove" role="button">Remove</a>
                </div>
            </div>
        </div>
        {{/each}}
    </script>
</div>