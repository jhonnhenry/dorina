
function fileProcessResult(jsonResult) {
    console.log(jsonResult);
    var source = document.getElementById('fileProcessResultTemplate').innerHTML;
    const fileProcessResultTemplate = Handlebars.compile(source);
    var html = fileProcessResultTemplate(jsonResult);
    console.log(html);
    document.getElementById('processResultContainer').innerHTML = html;
    $('#processResultContainer').fadeIn();
}