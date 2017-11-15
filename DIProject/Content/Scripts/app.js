var h1 = document.getElementsByTagName('h1')[0],
    miliseconds = 0, seconds = 0, minutes = 0, hours = 0,
    t;

function add() {
    miliseconds++;
    if (miliseconds >= 1000) {
        miliseconds = 0;
        seconds++;
        if (seconds >= 60) {
            seconds = 0;
            minutes++;
            if (minutes >= 60) {
                minutes = 0;
                hours++;
            }
        }
    }
    h1.textContent = (hours ? (hours > 9 ? hours : "0" + hours) : "00") + ":" + (minutes ? (minutes > 9 ? minutes : "0" + minutes) : "00") + ":" + (seconds > 9 ? seconds : "0" + seconds) + "." + (miliseconds>9? miliseconds :"0" + miliseconds);
    timer();
}

function timer() {
    t = setTimeout(add, 1);
}


$(document)
    .ajaxStart(function () {
        h1.textContent = "00:00:00.000";
        seconds = 0; minutes = 0; hours = 0;
        window.showLoading({ allowHide: true });
        timer();
    })
    .ajaxStop(function () {
        clearTimeout(t);
        window.hideLoading();
    });

$(document).ready(function() {
    var type = $('#type').val();
    if (type != "setpixels") {
        $('#filter').prop("disabled", true);
    }
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