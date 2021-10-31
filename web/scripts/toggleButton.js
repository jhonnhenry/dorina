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