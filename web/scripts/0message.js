
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