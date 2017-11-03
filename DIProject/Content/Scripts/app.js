$(document) //TODO dodać spiner albo coś bo przy bardzo dużych zdjęciach mocno zamula
    .ajaxStart(function () {
        window.showLoading({ allowHide: true });
    })
    .ajaxStop(function () {
        window.hideLoading();
    });

function CallControllerAndRefreshImg(url, data) {
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
        url: url,
        type: 'POST',
        data: data,
        dataType: 'text',
        contentType: false,
        processData: false,
        success: function (base64) {
            $('#img').attr('src', 'data:image/png;base64,' + base64);
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
    CallControllerAndRefreshImg('/DIP/upload', $formData);
});

$('#invert').click(function () {
    CallControllerAndRefreshImg('/DIP/invert', null);
});

$('#grayscale').click(function () {
    CallControllerAndRefreshImg('/DIP/SetGrayscale', null);
});

$('#brightness').on("change", function () {
    var value = $(this).val();
    CallControllerAndRefreshImg('/DIP/SetBrightness?brightness='+value, null);
});