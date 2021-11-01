"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/fileprocess")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function startConnection() {
    await connection.start().then(function () {
        loading(false);
        $('.fileProcessPage').fadeIn();
        console.log("SignalR Connected.");
    }).catch(function (err) {
        showClientMessage('A não!','Não foi possível se comunicar com nosso servidor! Tente verificar sua conexão com a internet.','error'        );
        setTimeout(function () { startConnection(); }, 6000);
        return console.error(err.toString());
    });
};

connection.onclose(async () => {
    loading(true);
    await startConnection();
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

/******************/
/******************/
startConnection();
/******************/
/******************/

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
    //console.log(message);
    //console.log(parameter);
    connection.invoke("SendMessage", message, parameter)
        .catch(function (err) {
            showClientMessage('A não!', 'Algum erro não esperado ocorreu ao se comunicar com nosso servidor! Tente verificar sua conexão com a internet.', 'error');
            return console.error(err.toString());
        });
    return true;
}
