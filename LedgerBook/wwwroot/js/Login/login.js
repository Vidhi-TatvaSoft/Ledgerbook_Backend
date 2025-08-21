function logout() {
    deleteAllCookies();
    window.location = LOGIN_LINK;
}

function loginSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
        let cookies = response.result;
        if (cookies.userToken != null) {
            setCookie(User_Token, cookies.userToken, 1);
        }
        if (cookies.userName != null) {
            setCookie(User_Name, cookies.userName, 1);
        } if (cookies.profilePhoto != null) {
            setCookie(Profile_Photo, cookies.profilePhoto, 1);
        }
        removeValidations();
        window.location = BUSINESS_PAGE_LINK;
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function registerSuccess(response) {
    if (response.isSuccess) {
        const toaster = {
            isSuccess: response.isSuccess,
            toasterMessage: response.toasterMessage
        };
        localStorage.setItem('Toaster', JSON.stringify(toaster));
        removeValidations();
        window.location = LOGIN_LINK
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function verifyEmailSuccess(response) {
    if (response.isSuccess) {
        const toaster = {
            isSuccess: response.isSuccess,
            toasterMessage: response.toasterMessage
        };
        localStorage.setItem('Toaster', JSON.stringify(toaster));
        removeValidations();
        window.location = LOGIN_LINK
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error")
        }
    }
}

function verifyResetToken(response) {
    if (!response.isSuccess) {
        const toaster = {
            isSuccess: response.isSuccess,
            toasterMessage: response.toasterMessage
        };
        localStorage.setItem('Toaster', JSON.stringify(toaster));
        removeValidations()
        window.location = LOGIN_LINK
    } else {
        $("#resetpassword-email").val(response.result)
    }
}