$(function () {
    var Index = $('#AdsIndex');
    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            Index.html(data);
            Index.find('.modal').modal('show');
        })
    })