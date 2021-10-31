
PNotify.defaultModules.set(PNotifyFontAwesome5, {});
PNotify.defaultModules.set(PNotifyMobile, {});
PNotify.defaultModules.set(PNotifyCountdown, {
    anchor: 'bottom',
});
PNotify.defaults.mode = "light";
PNotify.defaults.labels = { close: 'fechar', stick: 'Fixar', unstick: 'Soltar' };
PNotify.defaultStack.close();

function showClientMessage(title, text, icon = 'far fa-envelope', type = 'notice') {
    showMessage({
        title: title,
        text: text,
        messageIcon: icon,
        messageType: type
    })
}

function showMessage(message) {
    switch (message.messageType) {
        case 'notice':
            PNotify.notice({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenClose
            }); break;
        case 'info':
            PNotify.info({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenClose
            }); break;
        case 'success':
            PNotify.success({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenClose
            }); break;
        case 'error':
            PNotify.error({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenClose
            }); break;
        default:
    }

}
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

$(document).ajaxStart(function (a, b, c, d) {
    /*console.log(a);
    console.log(b);
    console.log(c);
    console.log(d);*/
});
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    console.log(event);
    console.log(jqxhr);
    console.log(settings);
    console.log(thrownError);
});
$(document).ajaxSend(function (a, b, c, d) {
    /* console.log(a);
     console.log(b);
     console.log(c);
     console.log(d);*/
});
$(document).ajaxStop(function (a, b, c, d) {
    /* console.log(a);
     console.log(b);
     console.log(c);
     console.log(d);*/
});

function clearPage() {
    $('#accordionExample').fadeOut();
    $('#fileProcessResultContainer').fadeOut();
    $('#accordionExample').empty();
    $('#fileProcessResultContainer').empty();
    $('#tips').fadeOut();
    $('#statusAlert').fadeOut();
}

function fileProcessResult(jsonResult) {
    console.log(jsonResult);
    var source = document.getElementById('fileProcessResultTemplate').innerHTML;
    const fileProcessResultTemplate = Handlebars.compile(source);
    var html = fileProcessResultTemplate(jsonResult);
    console.log(html);
    document.getElementById('processResultContainer').innerHTML = html;
    $('#processResultContainer').fadeIn();
}

function loading(status) {
    if (status)
        $('.modalload').fadeIn();
    else
        $('.modalload').fadeOut();
}

$(document).ready(function () {
    loading(true);
});



function toggleButtonById(btnId, enable) {
    var btn = document.getElementById(btnId);
    toggleButton(btn, enable);
}

function toggleButton(btn, enable) {
    if (btn) {
        if (enable) {
            $(btn).removeAttr("disabled");
            $(btn).removeClass("disabled");
            $(btn).css("cursor", "pointer");
        } else {
            $(btn).attr("disabled", true);
            $(btn).addClass("disabled");
            $(btn).css("cursor", "not-allowed");
        }
    }
}
function AJAXSubmit(oFormElement) {

    if ($('#theFile').val() == '') {
        showClientMessage("Atenção", "Você precisa informar um arquivo.");
        return false;
    }

    var fullPath = $('#theFile').val();
    var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
    var filename = fullPath.substring(startIndex);
    filename = filename.substring(1);

    if (!XMLHttpRequest.prototype.sendAsBinary) {
        XMLHttpRequest.prototype.sendAsBinary = function (sData) {
            var nBytes = sData.length, ui8Data = new Uint8Array(nBytes);
            for (var nIdx = 0; nIdx < nBytes; nIdx++) {
                ui8Data[nIdx] = sData.charCodeAt(nIdx) & 0xff;
            }
            this.send(ui8Data);
        };
    }

    async function submitRequest(oTarget) {
        var nFile, sFieldType, oField, oSegmReq, oFile, bIsPost = oTarget.method.toLowerCase() === "post";
        this.contentType = bIsPost && oTarget.enctype ? oTarget.enctype : "application\/x-www-form-urlencoded";
        this.technique = bIsPost ?
            this.contentType === "multipart\/form-data" ? 3 : this.contentType === "text\/plain" ? 2 : 1 : 0;
        this.receiver = oTarget.action;
        this.status = 0;
        this.segments = [];
        for (var nItem = 0; nItem < oTarget.elements.length; nItem++) {
            oField = oTarget.elements[nItem];
            if (!oField.hasAttribute("name")) { continue; }
            sFieldType = oField.nodeName.toUpperCase() === "INPUT" ? oField.getAttribute("type").toUpperCase() : "TEXT";
            if (sFieldType === "FILE" && oField.files.length > 0) {
                if (this.technique === 3) {
                    /* enctype is multipart/form-data */
                    for (nFile = 0; nFile < oField.files.length; nFile++) {
                        oFile = oField.files[nFile];
                        oSegmReq = new FileReader();
                        /* (custom properties:) */
                        oSegmReq.segmentIdx = this.segments.length;
                        oSegmReq.owner = this;
                        /* (end of custom properties) */
                        oSegmReq.onload = pushSegment;
                        this.segments.push("Content-Disposition: form-data; name=\"" +
                            oField.name + "\"; filename=\"" + oFile.name +
                            "\"\r\nContent-Type: " + oFile.type + "\r\n\r\n");
                        this.status++;
                        oSegmReq.readAsBinaryString(oFile);
                    }
                }
            }
        }
        processStatus(this);
    }


    function processStatus(oData) {
        if (oData.status > 0) { return; }
        submitData(oData);
    }

    function pushSegment(oFREvt) {
        this.owner.segments[this.segmentIdx] += oFREvt.target.result + "\r\n";
        this.owner.status--;
        processStatus(this.owner);
    }

    function ajaxSuccess() {
        loading(false);
        if (JSON.parse(this.responseText).hash) {
            sendMessageToServer('startFileProcess', JSON.parse(this.responseText).hash);
        }
    }

    function submitData(oData) {
        /* the AJAX request... */
        var oAjaxReq = new XMLHttpRequest();
        oAjaxReq.submittedData = oData;
        oAjaxReq.onload = ajaxSuccess;
        if (oData.technique === 0) {
            /* method is GET */
            oAjaxReq.open("get", oData.receiver.replace(/(?:\?.*)?$/,
                oData.segments.length > 0 ? "?" + oData.segments.join("&") : ""), true);
            oAjaxReq.send(null);
        } else {
            /* method is POST */
            oAjaxReq.open("post", oData.receiver, true);
            if (oData.technique === 3) {
                /* enctype is multipart/form-data */
                var sBoundary = "---------------------------" + Date.now().toString(16);
                oAjaxReq.setRequestHeader("Content-Type", "multipart\/form-data; boundary=" + sBoundary);
                oAjaxReq.sendAsBinary("--" + sBoundary + "\r\n" +
                    oData.segments.join("--" + sBoundary + "\r\n") + "--" + sBoundary + "--\r\n");
            } else {
                /* enctype is application/x-www-form-urlencoded or text/plain */
                oAjaxReq.setRequestHeader("Content-Type", oData.contentType);
                oAjaxReq.send(oData.segments.join(oData.technique === 2 ? "\r\n" : "&"));
            }
        }
    }

    isProcessInProgress(filename).then(function (progress) {
        if (progress > 0) {
            PNotify.notice({
                title: "Atenção",
                text: "O processo de verificação do seu arquivo já foi iniciado. Deseja cancelar?",
                icon: 'fas fa-question-circle',
                hide: false,
                closer: false,
                sticker: false,
                destroy: true,
                stack: new PNotify.Stack({
                    dir1: 'down',
                    modal: true,
                    firstpos1: 25,
                    overlayClose: false
                }),
                modules: new Map([
                    [PNotify.defaultModules],
                    [PNotifyConfirm, {
                        confirm: true,
                        buttons: [{
                            text: 'Sim',
                            primary: true,
                            click: function (notice) {
                                finalizeProcess(filename).then(function (progress) {
                                    console.log(progress);
                                    notice.update({
                                        title: 'Ok',
                                        text: 'O Processamento do seu arquivo foi cancelado.',
                                        icon: true,
                                        closer: true,
                                        sticker: true,
                                        type: 'error',
                                        hide: true,
                                        modules: new Map(PNotify.defaultModules)
                                    });
                                });
                            }
                        },
                        {
                            text: 'Não',
                            click: notice => notice.update({
                                title: 'Ok',
                                text: 'Aguarde a conclusão do processamento.',
                                icon: true,
                                closer: true,
                                sticker: true,
                                type: 'info',
                                hide: true,
                                modules: new Map(PNotify.defaultModules)
                            })
                        }]
                    }]])
            });
            return false;
        }
        else {
            loading(true);
            clearPage();
            submitRequest(oFormElement);
        }
    });
}
function writeFileResult(fileProcessResult) {
    console.log(fileProcessResult);

    var source = document.getElementById('fileProcessResultTemplate').innerHTML;
    const fileResultTemplate = Handlebars.compile(source);
    var html = fileResultTemplate(fileProcessResult);
    var htmlObject = document.createElement('div');
    htmlObject.innerHTML = html;
    document.getElementById('fileProcessResultContainer').append(htmlObject);
    $('#fileProcessResultContainer').fadeIn();
    $('#tips').fadeIn();
}
function writePageResult(pageProcessResult) {
    var source = document.getElementById('pageProcessResultTemplate').innerHTML;
    const pageResultTemplate = Handlebars.compile(source);
    var html = pageResultTemplate(pageProcessResult);
    var htmlObject = document.createElement('div');
    htmlObject.innerHTML = html;
    document.getElementById('accordionExample').append(htmlObject);
    $('#accordionExample').fadeIn();
}