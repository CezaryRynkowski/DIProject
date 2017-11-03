$('#upload').click(function () {
    var $file = document.getElementById('file'),
        $formData = new FormData();

    if ($file.files.length > 0) {
        for (var i = 0; i < $file.files.length; i++) {
            $formData.append('file-' + i, $file.files[i]);
        }
    }

    $.ajax({
        url: '/DIP/upload',
        type: 'POST',
        data: $formData,
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
});