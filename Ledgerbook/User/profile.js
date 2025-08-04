function getProfileSuccess(response) {
    console.log(response);
    $("#profile-FirstName").val(response.result.firstName);
    $("#profile-LastName").val(response.result.lastName);
    $("#profile-email").val(response.result.email);
    if (response.result.mobileNumber != 0)
        $("#profile-mobilenumber").val(response.result.mobileNumber);
    console.log(response.result.attachmentViewModel.businesLogoPath)
    if (response.result.attachmentViewModel.businesLogoPath != null) {
        $("#uploadedimage").attr('src', BASE_URL.replace("api", response.result.attachmentViewModel.businesLogoPath))
    }
}

function setProfileSuccess(response) {
    if (response.isSuccess) {
        console.log(response)
        let cookies = response.result;
        if (cookies.userToken != null) {
            setCookie(User_Token, cookies.userToken, 1);
        }
        if (cookies.userName != null) {
            setCookie(User_Name, cookies.userName, 1);
        } if (cookies.profilePhoto != null) {
            setCookie(Profile_Photo, cookies.profilePhoto, 1);
        }
        const toaster = {
            isSuccess: response.isSuccess,
            toasterMessage: response.toasterMessage
        };
        localStorage.setItem('Toaster', JSON.stringify(toaster));
        RemoveValidations();
        if (getCookie(Business_Token)) {
            window.location = "/Ledgerbook/Party/Managebusiness.html"
        } else {
            window.location = "/Ledgerbook/Business/Index.html"
        }
    } else {
        Toaster(response.toasterMessage, "error");
    }
}