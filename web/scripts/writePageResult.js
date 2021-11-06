function writePageResult(pageProcessResult) {
    var source = document.getElementById('pageProcessResultTemplate').innerHTML;
    const pageResultTemplate = Handlebars.compile(source);
    var html = pageResultTemplate(pageProcessResult);
    var htmlObject = document.createElement('div');
    htmlObject.innerHTML = html;
    document.getElementById('accordionExample').append(htmlObject);
    $('#accordionExample').fadeIn();
    $('.spinner-border').fadeOut();
    $('html, body').animate({ scrollTop: 620 }, 1000);
}