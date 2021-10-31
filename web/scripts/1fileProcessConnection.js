"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/fileprocess").build();

connection.start().then(function () {
    loading(false);
    $('.fileProcessPage').fadeIn();
}).catch(function (err) {
    PNotify.error({
        title: 'A não!',
        text: 'Não foi possível se comunicar com nosso servidor! Tente verificar sua conexão com a internet.',
        hide: false
    });
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (comunication) {
    showMessage(comunication.message);
});

connection.on("UpdateStatus", function (progress, comunication) {
    $('#statusAlert').fadeIn();
    $('#statusAlert').html(comunication);

    if ($('.progress-bar').html() != progress + '%') {
        $('.progress').fadeIn();
        $('.progress-bar').css('width', progress + '%');
        $('.progress-bar').html(progress + '%');
    }
});

connection.on("FinalizeProcess", function (comunication) {
    $('.progress').fadeOut();
});

connection.on("WritePageResult", function (pageProcessResult) {
    writePageResult(pageProcessResult);
});

connection.on("WriteFileResult", function (fileProcessResult) {
    writeFileResult(fileProcessResult);
});


function isProcessInProgress(filename) {
    return connection.invoke("IsProcessInProgress", filename);
}

function finalizeProcess(filename) {
    return connection.invoke("FinalizeProcess", filename)
        .then(function (result) {
            return result
        });
}

function sendMessageToServer(message, parameter) {
    connection.invoke("SendMessage", message, parameter)
        .catch(function (err) {
            PNotify.error({
                title: 'A não!',
                text: 'Algum erro não esperado ocorreu ao se comunicar com nosso servidor! Tente verificar sua conexão com a internet.',
                hide: false
            });
            return console.error(err.toString());
        });
    return true;
}
