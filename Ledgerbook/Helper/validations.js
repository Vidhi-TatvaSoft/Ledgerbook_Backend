function emailValidation(input) {
    let value = input.value.replace(/\s/g, '');
    const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (value == "") {
        $(".emailValidationMessage").html("Email is required.");
    } else if (!regex.test(value)) {
        $(".emailValidationMessage").html("Please enter a valid Email.");
    } else if (value.length > 255) {
        $(".emailValidationMessage").html("Email can not exceed 255 characters.");
    } else {
        $(".emailValidationMessage").html("");
    }
}

function passwordValidation(input) {
    let value = input.value.replace(/\s/g, '');
    const regex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
    if (value == "") {
        $(".passwordValidationMessage").html("Password is required.");
    } else if (value.length < 8) {
        $(".passwordValidationMessage").html("Password must be at least 8 characters long.");
    } else if (value.length > 128) {
        $(".passwordValidationMessage").html("Password can not exceed 128 characters.");
    } else if (!regex.test(value)) {
        $(".passwordValidationMessage").html("Password must contain at least one uppercase letter, number, special character and should not contain whitespace.");
    } else {
        $(".passwordValidationMessage").html("");
    }
}

function cnfpasswordValidation(input, passwordinputId) {
    let value = input.value.replace(/\s/g, '');
    console.log(passwordinputId)
    let password = document.getElementById(`${passwordinputId}`).value.replace(/\s/g, '');
    const regex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
    if (value == "") {
        $(".cnfpasswordValidationMessage").html("Confirm Password is required.");
    } else if (value != password) {
        $(".cnfpasswordValidationMessage").html("Password and Confirm Password should be same.");
    } else {
        $(".cnfpasswordValidationMessage").html("");
    }
}

function nameValidation(input, type) {
    let value = input.value;
    const regex = /^[a-zA-Z]+$/
    if (value == "") {
        $(`.${type}NameValidationMessage`).html(`${type} Name is required.`);
    } else if (value.includes(" ") || !regex.test(value)) {
        $(`.${type}NameValidationMessage`).html(`${type} Name must contain only alphabets.`);
    } else if (value.length > 50) {
        $(`.${type}NameValidationMessage`).html(`${type} Name can not exceed 50 characters.`);
    } else {
        $(`.${type}NameValidationMessage`).html("");
    }
}

function NumberValidation(input) {
    let value = input.value.replace(/\D/g, '');
    const regex = /^[a-zA-Z]+$/
    if (value == "") {
        $(".numberValidationMessage").html("");
    } else if (value.length < 10) {
        $(".numberValidationMessage").html("Mobile Number must be 10 digits long.");
    } else if (value.length > 10) {
        value = value.slice(0, 10);
        input.value = value;
    } else {
        $(".numberValidationMessage").html("");
    }
}



function validateLoginForm() {
    let email = document.getElementById("login-email")
    let password = document.getElementById("login-password")

    emailValidation(email);
    passwordValidation(password);
    if ($(".emailValidationMessage").html() == "" && $(".passwordValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateRegisterForm() {
    let email = document.getElementById("register-email")
    let password = document.getElementById("register-password")
    let firstName = document.getElementById("register-FirstName");
    let lastname = document.getElementById("register-LastName");
    let cnfPassword = document.getElementById("register-confirmPassword")
    emailValidation(email);
    passwordValidation(password);
    nameValidation(firstName, "First")
    nameValidation(lastname, "Last")
    cnfpasswordValidation(cnfPassword, 'register-password')
    if ($(".emailValidationMessage").html() == "" && $(".passwordValidationMessage").html() == "" && $(".FirstNameValidationMessage").html() == "" && $(".LastNameValidationMessage").html() == "" && $(".cnfpasswordValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateForgotPasswordForm() {
    let email = document.getElementById("forgotpassword-email")

    emailValidation(email);
    if ($(".emailValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateResetPasswordForm() {
    let password = document.getElementById("resetpassword-password")
    let cnfPassword = document.getElementById("resetpassword-confirmPassword")
    passwordValidation(password);
    cnfpasswordValidation(cnfPassword, 'resetpassword-password')
    if ($(".passwordValidationMessage").html() == "" && $(".cnfpasswordValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateProfileForm() {
    let firstName = document.getElementById("profile-FirstName");
    let lastname = document.getElementById("profile-LastName");
    let mobileNumber = document.getElementById("profile-mobilenumber");

    nameValidation(firstName,'First')
    nameValidation(lastname,'Last')
    NumberValidation(mobileNumber)
    if ($(".numberValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function RemoveValidations() {
    $(".validationMessage").html("");
}