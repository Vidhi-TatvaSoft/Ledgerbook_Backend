function Logout() {
    deleteAllCookies();
    window.location = "/Login/Login";
}

function LoginSuccess(response) {
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
        RemoveValidations();
        window.location = "/Business/Index";
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
        RemoveValidations();
        window.location = "/Login/Login"
    } else {
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
        RemoveValidations();
        window.location = "/Login/Login"
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
        RemoveValidations()
        window.location = "/Login/Login"
    } else {
        $("#resetpassword-email").val(response.result)
    }
}