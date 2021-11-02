function imageViewShow(e) {
    $(".image-viewer-full-image").attr("src", $(e).attr("src"));
    $('.image-viewer-container').show();
}
