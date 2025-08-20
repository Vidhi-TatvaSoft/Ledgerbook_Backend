// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggle_fnction(demoicon, demopass_type) {
    let icon = document.getElementById(demoicon);
    let pass_type = document.getElementById(demopass_type);


    icon.classList.toggle('fa-eye');
    icon.classList.toggle('fa-eye-slash');

    const type = pass_type
        .getAttribute('type') === 'password' ?
        'text' : 'password';
    pass_type.setAttribute('type', type);

}

function pincodeValidation(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length > 6) {
        value = value.slice(0, 6);
    }
    input.value = value;
}

function removeWhiteSpace(input) {
    let value = input.value;

    value = value.replace(/\s/g, '');
    if (!value) {
        input.value = value;
    }
}

function trimInput(input) {
    let value = input.value;
    value = value.trim();
    input.value = value;
}

function changePasswordModal() {
    $('#change-password-modal').modal('show');
}

function changepasswordSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
        let userToken = response.result;
        if (userToken != null) {
            setCookie(User_Token, userToken, 1);
        }
        $(".btn-close").click();
         emptyInputValidation();
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

$(document).on("submit", "#change-password-form", function (e) {
    e.preventDefault();
    let form = $("#change-password-form")
    let isValidForm = validateChangePasswordForm();
    if (isValidForm) {
        let formData = new FormData();
        formData.append("OldPassword", $("#changepassword-old").val())
        formData.append("Password", $("#changepassword-new").val())
        formData.append("ConfirmPassword", $("#changepassword-confirm").val())
        let params = setParameter("/User/ChangePassword", POST, null, FORMDATA, formData, changepasswordSuccess);
        ajaxCall(params);
    }
})

function myProfile() {
    window.location = "/User/Profile";
}

function toasterFromLocalstorage() {
    let response = localStorage.getItem('Toaster');
    response = JSON.parse(response)
    if (response != null) {
        if (response.isSuccess) {
            Toaster(response.toasterMessage)
        } else {
            Toaster(response.toasterMessage, "error")
        }
        localStorage.removeItem('Toaster');
    }
}
