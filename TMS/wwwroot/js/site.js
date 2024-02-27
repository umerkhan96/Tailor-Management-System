// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var waitingPopup;
function showLoader() {
    waitingPopup = Swal.fire({
        title: "Waiting for response from server!",
        html: "please wait!<br>",
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        },
    });
}

function hideLoader() {
    waitingPopup.close();
}

function showMessage(title, text, icon, callback) {
    Swal.fire({
        text: text,
        title: title,
        icon: icon
    }).then((flag) => {
        if (flag) {
            if (callback) {
                callback();
            }
        }
    })
}

function showAlert(title, text, icon = 'success', showCancel = false, cancelButtonText = "No", confirmButtonText = "Yes", callbackSuccess, callbackCancel) {
    Swal.fire({
        title: title,
        text: text,
        icon: icon,
        allowOutsideClick: false,
        showCancelButton: showCancel,
        cancelButtonText: cancelButtonText,
        confirmButtonText: confirmButtonText
    }).then((result) => {
        if (result.isConfirmed && callbackSuccess) {
            callbackSuccess();
        } else if (result.isDismissed && callbackCancel) {
            callbackCancel();
        }
    });
}