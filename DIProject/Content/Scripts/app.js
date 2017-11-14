$(document)
    .ajaxStart(function () {
        window.showLoading({ allowHide: true });
    })
    .ajaxStop(function () {
        window.hideLoading();
    });

function CallControllerAndRefreshImg(urlRest, data) {
    var url = '';
    var type = $('#type').val();
    if (type == 'setpixels') {
        url = '/DIP';
    } else {
        url = '/DIPLockBits';
    }
    $.ajax({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    console.log(percentComplete);
                }
            }, false);
            xhr.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    console.log(percentComplete);
                }
            }, false);
            return xhr;
        },
        url: url + urlRest,
        type: 'POST',
        data: data,
        dataType: 'text',
        contentType: false,
        processData: false,
        success: function (base64) {
            $('#img').attr('src', 'data:image/png;base64,' + base64);
            var currentHeight = $('#img').height;
            var currentWidth = $('#img').width;
            $('#currentHeight').val(currentHeight);
            $('#currentWidth').val(currentWidth);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(xhr.responseText);
            alert(thrownError);
        }
    });
}

$('#upload').click(function () {
    var $file = document.getElementById('file'),
        $formData = new FormData();

    if ($file.files.length > 0) {
        for (var i = 0; i < $file.files.length; i++)
            $formData.append('file-' + i, $file.files[i]);
    }
    CallControllerAndRefreshImg('/upload', $formData);
});

$('#invert').click(function () {
    CallControllerAndRefreshImg('/invert', null);
});

$('#grayscale').click(function () {
    CallControllerAndRefreshImg('/SetGrayscale', null);
});

$('#brightness').on("change", function () {
    var value = $(this).val();
    CallControllerAndRefreshImg('/SetBrightness?brightness=' + value, null);
});

$('#filter').change(function () {
    var value = $(this).val();
    CallControllerAndRefreshImg('/SetColorFilter?colorFilter=' + value, null);
});

$('#contrast').on("change", function () {
    var value = $(this).val();
    CallControllerAndRefreshImg('/SetContrast?contrast=' + value, null);
});

$('.rotate').click(function () {
    var value = $(this).attr("id");
    CallControllerAndRefreshImg('/RotateFlip?rotateFlip=' + value, null);
});

$('#resizeButton').click(function () {
    var width = $('#width').val();
    var height = $('#height').val();
    CallControllerAndRefreshImg('/Resize?newWidth=' + width + '&newHeight=' + height, null);
});