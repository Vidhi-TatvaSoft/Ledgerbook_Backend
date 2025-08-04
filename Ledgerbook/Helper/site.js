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



function PincodeValidation(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length > 6) {
        value = value.slice(0, 6);
    }
    input.value = value;
}

function RemoveWhiteSpace(input) {
    let value = input.value;

    value = value.replace(/\s/g, '');
    if (!value) {
        input.value = value;
    }
}

function TrimInput(input) {
    let value = input.value;
    value = value.trim();
    input.value = value;
}

function HandleResponse(response, replacePartialViewId) {
    if (response) {
        if (response.redirectUrl) {
            window.location.href = response.redirectUrl;
        } else {
            $(replacePartialViewId).html(response.html);
        }
        if (response.message) {
            if (response.errorType == 1) {
                Toaster(response.message);
            } else {
                Toaster(response.message, "error");
            }
        }
    }
}


function AjaxComplete(event, xhr, settings) {
    if (xhr.getResponseHeader("X-Error") == "true") {
        console.log("ajaxxxx")
        const response = xhr.responseJSON;
        if (response && response.error) {
            console.log(response)

            $('.modal').modal('hide');
            $('.btn-close').click();
            $('.modal-backdrop').remove();
            $(document.body).removeClass("modal-open");
            $(document.body).removeAttr("style")

            Toaster(response.error, "error");
        } else {
            $('.modal').modal('hide');
            $('.btn-close').click();
            Toater('An unexpected error occurred.', "error");
        }
    }
}

function changePasswordModal() {
    $.ajax({
        url: "/User/RenderChangePassword",
        type: "GET",
        success: function (response) {
            IsHtmlDoc(response);
            $("#changepassword-body-id").html(response)
            // HandleResponse(response,"#changepassword-body-id")
        }
    })
}

// $(document).on("submit", "#change-password-form", function (e) {
//     e.preventDefault();
//     let form = $("#change-password-form")
//     if (form.valid()) {
//         let formData = new FormData(this);

//         $.ajax({
//             url: "/User/ChangePassword",
//             type: "POST",
//             contentType: false,
//             processData: false,
//             data: formData,
//             success: function (response) {
//                 IsHtmlDoc(response.toString());
//                 if (response.success) {
//                     Toaster(response.message);
//                     $('.btn-close').click();
//                 } else {
//                     Toaster(response.message, "error");
//                 }
//             }
//         })
//     }
// })


function MyProfile() {
    window.location = "/Ledgerbook/User/Profile.html";
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


function SetuserNameImage() {
    let userName = getCookie(User_Name);
    let profilePhoto = getCookie(Profile_Photo) == "" ? "/Ledgerbook/Images/user.png" : BASE_URL.replace("api", getCookie(Profile_Photo));
    console.log(profilePhoto)
    $("#businesscard-layout-userName").html(userName);
    $("#businesscard-layout-profilePhoto").attr("src", profilePhoto);
}