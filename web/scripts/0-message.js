
PNotify.defaultModules.set(PNotifyFontAwesome5, {});
PNotify.defaultModules.set(PNotifyMobile, {});
PNotify.defaultModules.set(PNotifyCountdown, {
    anchor: 'bottom',
});
PNotify.defaults.mode = "light";
PNotify.defaults.labels = { close: 'fechar', stick: 'Fixar', unstick: 'Soltar' };
PNotify.defaultStack.close();

function showClientMessage(title, text, type = 'notice', icon = 'fas fa-envelope',) {
    showMessage({
        title: title,
        text: text,
        messageIcon: icon,
        messageType: type
    })
}

function showMessage(message) {
    if (typeof window.maxOpenWait === 'undefined') {
        window.maxOpenWait = new PNotify.Stack({
            dir1: 'down',
            dir2: 'left',
            firstpos1: 25,
            firstpos2: 25,
            modal: false,
            /*
            maxOpen: 3,
            maxStrategy: 'wait',
            */
            maxOpen: Infinity
        });
    }
    switch (message.messageType) {
        case 'notice':
            PNotify.notice({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenWait
            }); break;
        case 'info':
            PNotify.info({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenWait
            }); break;
        case 'success':
            PNotify.success({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenWait
            }); break;
        case 'error':
            PNotify.error({
                title: message.title,
                text: message.text,
                icon: message.messageIcon,
                stack: window.maxOpenWait
            }); break;
        default:
    }

}