function loginWithCookie() {
    let params = setParameter("/Login/Login", GET, null, NODATA, null, LoginSuccess);
    ajaxCall(params);
}

function LoginFormSubmit() {
    console.log("login form")
    let formData = new FormData();
    formData.append("Email", $("#login-email").val())
    formData.append("Password", $("#login-password").val())
    let params = setParameter("/Login/LoginAsync", POST, null, FORMDATA, formData, LoginSuccess);
    ajaxCall(params);
}

function Logout() {
    deleteAllCookies();
    window.location = "/Login/Login.html";
}

function LoginSuccess(response) {
    console.log(response)
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
        window.location.href = "http://127.0.0.1:5500/Ledgerbook/Business/Index.html";
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
        window.location = "/Login/Login.html"
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
        window.location = "/Login/Login.html"
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
        window.location = "/Login/Login.html"
    } else {
        $("#resetpassword-email").val(response.result)
    }
}