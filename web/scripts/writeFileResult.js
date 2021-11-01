function writeFileResult(fileProcessResult) {
    //console.log(fileProcessResult);

    var source = document.getElementById('fileProcessResultTemplate').innerHTML;
    const fileResultTemplate = Handlebars.compile(source);
    var html = fileResultTemplate(fileProcessResult);
    var htmlObject = document.createElement('div');
    htmlObject.innerHTML = html;
    document.getElementById('fileProcessResultContainer').append(htmlObject);
    $('#fileProcessResultContainer').fadeIn();
    $('#tips').fadeIn();
}