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

function loading(status) {
    if (status)
        $('.modalload').fadeIn();
    else
        $('.modalload').fadeOut();
}

$(document).ready(function () {



});



$(document).ready(function () {

});

async function AJAXSubmit(oFormElement) {
    loading(true);
    const formData = new FormData(oFormElement);
    try {
        await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        }).then(function (response) {
            loading(false);
            response.json().then(function (jsonresult) {
                console.log(jsonresult.result)
                document.getElementById("json").textContent = JSON.stringify(jsonresult.result, undefined, 2);
            });
        });

    } catch (error) {
        loading(false);
        console.error('Error:', error);
    }
}