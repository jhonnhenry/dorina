
function loading(status) {
    if (status)
        $('.modalload').fadeIn();
    else
        $('.modalload').fadeOut();
}

$(document).ready(function () {
    loading(true);
});


function copyToClipboard() {
    var copyText = document.getElementById("copyToClipboardInput");
    copyText.select();
    copyText.setSelectionRange(0, 99999); /* For mobile devices */
    navigator.clipboard.writeText(copyText.value);
    showClientMessage('Sucesso!', 'O texto do arquivo foi copiado para a área de transferência.', 'info');
}

