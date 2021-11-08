function validateFile(oFileElement) {

    if (!oFileElement.files[0]) {
        showClientMessage("Atenção", "Você precisa informar um arquivo.");
        return false;
    }

    validateExtension(oFileElement.files[0]);
    validateSize(oFileElement.files[0]);
}


function validateExtension(file) {

    if (file.type !== "application/pdf") {
        showClientMessage("Atenção", "O tipo de arquivo precisa ser .PDF");
        return false;
    }
    return true;
}

function validateSize(file) {
    if (file.size / 1024 / 1024 > 10) {
        showClientMessage("Atenção", "O tamanho do arquivo não pode exceder 10MB");
        return false;
    }
}
