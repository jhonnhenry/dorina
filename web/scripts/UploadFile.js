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