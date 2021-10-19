
showInPopUp = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
}

function updateIndex(url) {
    var jqxhr = $.ajax({
        url: url,
        type: 'GET',
        success: function (res) {
            var final;
            final = $(res).ready().find("#view-all").html();
            console.log(res);
            console.log(final);
            $('#view-all').html(final);
        }
    });
}

function closePopUp() {
    $('#form-modal .modal-body').html('');
    $('#form-modal .modal-title').html('');
    $('#form-modal').modal('hide');
}

createInPopUp = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html);
                    closePopUp();
                } else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: function (err) {
                console.log(err);
            }
        })
        return false;
    } catch (e) {
        console.log(e);
    }
}

deleteInPopUp = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                $('#view-all').html(res.html);
                closePopUp();
            },
            error: function (err) {
                console.log(err);
            }
        })
        return false;
    } catch (e) {
        console.log(e);
    }
}

editInPopUp = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    updateIndex(res.url);
                    closePopUp();;
                } else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: function (err) {
                console.log(err);
            }
        })
        return false;
    } catch (e) {
        console.log(e);
    }
}