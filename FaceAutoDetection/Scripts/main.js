/**
* Class FaceAutoDetect
*/
function FaceAutoDetect() {
    FaceAutoDetect.prototype.init.apply(this, arguments);
}

FaceAutoDetect.prototype = {
    init: function (containerId) {
        if (containerId == null || containerId.length <= 0) {
            typeof (console) != 'undefined' && console.log && cons.log('Not found `@containerId`');
            return;
        }

        this.$container = $(containerId);
        if ($.isEmptyObject($.template('photoTemplate')))
            $.template('photoTemplate', $('#photoTemplate', this.$container));

        this.fileUpload();
    },

    fileUpload: function () {
        var _this = this;
        // Register file upploader
        $('.file-uploader', this.$container).fileupload({
            url: baseUri + 'Home/UploadPhoto',
            dataType: 'json',
            autoUpload: false,
            pasteZone: null,
            dropZone: null,
            singleFileUploads: false,
            // Pass authorization token to pass through the authentication module            
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                //self.$attachmentTable.find('tr[id="' + that.rowId + '"]').find('.upload-progress').text(progress + '%');
                var $progress = $(this).siblings('.upload-progress').first();
                $progress.text(progress + '%');
                if (progress == 100) {
                    $progress.text('');
                }
            },
            add: function (e, data) {
                if (data.files.length > 0) {
                    var extension = data.files[0].name.toLowerCase().split('.').pop(),
                        photoExts = $(this).next('#allowedExtension').val().toLowerCase().split(';');
                    //var fileSize = data.files[0].size;
                    if (photoExts.indexOf(extension) == -1 /* || fileSize>(2*1024*1024)/*2MB*/) {
                        alert('Please upload file with following extension ' + photoExts.join());
                    }
                    else {
                        var jqXHR = data.submit();
                    }
                }
            },
            done: function (e, data) {
                if (data.result.files.length > 0) {
                    // Files with success status go top
                    data.result.files.sort(function (f1, f2) {
                        return f1.success === f2.success ? 0 : f1.success ? -1 : 1;
                    });

                    var uploadSuccessArr = $.grep(data.result.files, function (f) { return f.success && !f.error; }),
                        $photoSection = $('.photo-section', _this.$container);

                    $photoSection.empty();
                    $.each(uploadSuccessArr, function (i, file) {
                        $.tmpl('photoTemplate', {
                            photos: [{
                                location: baseUri + file.location,
                                id: 0,
                                url: file.url,
                                fileName: file.fileName
                            }]
                        }).appendTo($photoSection);
                    });

                    //bind event face detecting auto
                    $photoSection.find('.photo-item').each(function () {
                        var $picture = $(this).find('.picture'),
                            $face = $(this).find('.face'),
                            $cropBoxData = {
                                width: 1,
                                height: 1,
                                left: 1,
                                top: 1
                            };

                        $(this).find('.btn-detect').click(function (e) {
                            e.preventDefault();
                            $face.remove();
                            // Face detection
                            $picture.faceDetection({
                                async: true,
                                complete: function (faces) {
                                    $cropBoxData = _this.detectCompleted(faces, $picture);
                                    // Initialize cropper
                                    _this.cropImage($picture, $cropBoxData);
                                    //$('.cropper-crop-box', _this.$container).css({ 'z-index': 99 });
                                },
                                error: function (code, message) {
                                    alert('Error: ' + message);
                                }
                            });
                        });

                        $(this).find('.btn-crop').click(function (e) {
                            e.preventDefault();
                            // Cropper and save image croped
                            var imageData = _this.$cropper.cropper('getImageData'),
                                strImageData = JSON.stringify(imageData),
                                $canvasData = _this.$cropper.cropper('getCroppedCanvas'),
                                imageUrl = $canvasData.toDataURL(),
                                blobImageData = imageUrl.replace('data:image/png;base64,', '');

                            $('.img-cropped', _this.$container).append($('<img/>').attr('src', imageUrl));
                            // Use `jQuery.ajax` method
                            $.ajax({
                                url: baseUri + 'Home/SaveImageCropped',
                                method: "POST",
                                data: { imageData: blobImageData },
                                cache: false,
                                dataType: 'json',
                                success: function (data) {
                                    if (data && data.isSuccess) {
                                        alert('Save cropped image successfully.');
                                    }
                                },
                                error: function () {
                                    alert('Save cropped image error');
                                }
                            });
                        });

                        $(this).find('.btn-remove').click(function (e) {
                            e.preventDefault();
                            window.location.href = window.location.href;
                        });

                    });
                }
                else if (data.result.errorMessage) {
                    alert(data.result.message);
                }
            }
        });
    },

    detectCompleted: function (faces, $picture) {
        var marg = 20,
            detectedBox = {
                width: null,
                height: null,
                left: null,
                top: null
            };
        if (faces.length > 0) {
            for (var i = 0; i < faces.length; i++) {
                if (faces[i].confidence != null) {
                    var left = (faces[i].x - marg),
                        top = (faces[i].y - marg),
                        width = (faces[i].width + (marg * 2)),
                        height = (faces[i].height + (marg * 2));

                    detectedBox.width = width * faces[i].scaleX;
                    detectedBox.height = height * faces[i].scaleY;
                    detectedBox.top = top * faces[i].scaleY;
                    detectedBox.left = left * faces[i].scaleX;

                    $('<div />', {
                        'class': 'face-img',
                        'css': {
                            'left': left * faces[i].scaleX + 'px',
                            'top': top * faces[i].scaleY + 'px',
                            'width': width * faces[i].scaleX + 'px',
                            'height': height * faces[i].scaleY + 'px',
                            'z-index': -1
                        }
                    })
                    .appendTo($picture.closest('div'));
                }
            }
        }
        return detectedBox;
    },

    removePhoto: function () {
        // Register handling attachment
        $('.photo-section', this.$container).find('.photo-item').off('click').on('click', function (evt) {
            evt.preventDefault();
            var $target = $(evt.target);
            if ($target.hasClass('btn-remove')) {
                var fileId = $target.closest('.photo-item').attr('file-id');
                //If file is uploaded file then remove immediate
                if (fileId <= 0) {
                    $.ajax({
                        type: 'POST',
                        url: baseUri + 'Home/RemovePhoto',
                        data: { fileToDelete: $target.closest('.photo-item').find('.hdLocation').val() },
                        success: function (data) {
                            // No matter if has error while deleting temporary file on server
                            $target.closest('.photo-item').remove();
                        }
                    })
                }
                else {
                    //Confirm before delete
                    if (confirm('Are you sure want to delete this item?')) {
                        $target.closest('.photo-item').remove();
                    }
                }
            }
        });
    },

    cropImage: function ($image, $cropBoxData) {
        //option cropper
        var options = {
            aspectRatio: 1 / 1,
            preview: '.img-preview',
            crop: function (e) {
                var $dataX = Math.round(e.x);
                var $dataY = Math.round(e.y);
                var $dataHeight = Math.round(e.height);
                var $dataWidth = Math.round(e.width);
                var $dataRotate = e.rotate;
                var $dataScaleX = e.scaleX;
                var $dataScaleY = e.scaleY;
            },
            built: function () {
                // Width and Height params are number types instead of string
                $(this).cropper("setCropBoxData", { width: $cropBoxData.width, height: $cropBoxData.height, left: $cropBoxData.left, top: $cropBoxData.top });
            }
        };

        //crop here
        this.$cropper = $image.on({
            ready: function (e) {
                console.log(e.type);
            },
            cropstart: function (e) {
                console.log(e.type, e.action);
            },
            cropmove: function (e) {
                console.log(e.type, e.action);
            },
            cropend: function (e) {
                console.log(e.type, e.action);
            },
            crop: function (e) {
                console.log(e.type, e.x, e.y, e.width, e.height, e.rotate, e.scaleX, e.scaleY);
            },
            zoom: function (e) {
                console.log(e.type, e.ratio);
            }
        }).cropper(options);

        // Keyboard
        $(document.body).on('keydown', function (e) {

            if (!$image.data('cropper') || this.scrollTop > 300) {
                return;
            }

            switch (e.which) {
                case 37:
                    e.preventDefault();
                    $image.cropper('move', -1, 0);
                    break;

                case 38:
                    e.preventDefault();
                    $image.cropper('move', 0, -1);
                    break;

                case 39:
                    e.preventDefault();
                    $image.cropper('move', 1, 0);
                    break;

                case 40:
                    e.preventDefault();
                    $image.cropper('move', 0, 1);
                    break;
            }

        });
    }
};

$(document).ready(function () {
    var faceAutoDetectObj = new FaceAutoDetect('.wrapper');
});