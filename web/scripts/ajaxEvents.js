$(document).ajaxStart(function (a, b, c, d) {
    /*console.log(a);
    console.log(b);
    console.log(c);
    console.log(d);*/
});
$(document).ajaxSend(function (a, b, c, d) {
    /* console.log(a);
     console.log(b);
     console.log(c);
     console.log(d);*/
    document.querySelectorAll('button[type="submit"]').forEach(function (btn) {
        toggleButton(btn, false);
    });
});
$(document).ajaxStop(function (a, b, c, d) {
    /* console.log(a);
     console.log(b);
     console.log(c);
     console.log(d);*/
    document.querySelectorAll('button[type="submit"]').forEach(function (btn) {
        toggleButton(btn, true);
    });
});
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    console.log(event);
    console.log(jqxhr);
    console.log(settings);
    console.log(thrownError);
});