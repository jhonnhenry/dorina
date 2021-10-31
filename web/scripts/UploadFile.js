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