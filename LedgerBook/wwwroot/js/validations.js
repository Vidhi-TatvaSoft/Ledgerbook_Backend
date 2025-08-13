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

function curPasswordValidation(input) {
    let value = input.value.replace(/\s/g, '');
    const regex = /^^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
    if (value == "") {
        $(".curPasswordValidationMessage").html("Current Password is required.");
    } else if (value.length < 8) {
        $(".curPasswordValidationMessage").html("Current Password must be at least 8 characters long.");
    } else if (value.length > 128) {
        $(".curPasswordValidationMessage").html("Current Password can not exceed 128 characters.");
    } else if (!regex.test(value)) {
        $(".curPasswordValidationMessage").html("Current Password must contain at least one uppercase letter, number, special character and should not contain whitespace.");
    } else {
        $(".curPasswordValidationMessage").html("");
    }
}

function cnfpasswordValidation(input, passwordinputId) {
    let value = input.value.replace(/\s/g, '');
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

///////////business page validations///////////////////
function buisnessNameValidation(input) {
    let value = input.value;
    if (value == "") {
        $(`.businessNameValidationMessage`).html(`Business Name is required.`);
    } else if (value.length > 100) {
        $(`.businessNameValidationMessage`).html(`Business Name can not exceed 100 characters.`);
    } else {
        $(`.businessNameValidationMessage`).html("");
    }
}

function numberValidationReq(input) {
    let value = input.value.replace(/\D/g, '');
    if (value == "") {
        $(".numberValidationMessage").html("Mobile Number is required.");
    } else if (value.length < 10) {
        $(".numberValidationMessage").html("Mobile Number must be 10 digits long.");
    } else if (value.length > 10) {
        value = value.slice(0, 10);
        input.value = value;
    } else {
        $(".numberValidationMessage").html("");
    }
}

function GSTINValidation(input) {
    let value = input.value.replace(/\s/g, '');
    const regex = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/;
    if (value == "") {
        $(".GSTINValidationMessage").html("");
    } else if (!regex.test(value)) {
        $(".GSTINValidationMessage").html("Enter valid GST Number.");
    } else {
        $(".GSTINValidationMessage").html("");
    }
}

function pincodeValidation(input) {
    let value = input.value.replace(/\D/g, '');
    if (value == "") {
        $(".pincodeValidationMessage").html("");
    } else if (value.length < 6) {
        $(".pincodeValidationMessage").html("Pincode must be 6 digits long.");
    } else if (value.length > 6) {
        value = value.slice(0, 6);
        input.value = value;
    } else {
        $(".pincodeValidationMessage").html("");
    }
}

///////////party  validations///////////////////
function partyNameValidation(input) {
    let value = input.value;
    if (value == "") {
        $(`.partyNameValidationMessage`).html(`Party Name is required.`);
    } else if (value.length > 100) {
        $(`.partyNameValidationMessage`).html(`Party Name can not exceed 100 characters.`);
    } else {
        $(`.partyNameValidationMessage`).html("");
    }
}

function MinAmount(input) {
    let value = parseFloat(input.value);
    if (value <= 0.0) {
        $(".amountValidationMessage").text("Amount should be greater than 0")
    }
    else if (value > 10000) {
        $(".amountValidationMessage").text("Amount should be greater than 0")
    } else {
        $(".amountValidationMessage").text("")
    }
}

function MinAmountRequire(input) {
    if (input.value == "") {
        $(".amountValidationMessage").text("Amount is required")
        return;
    }
    let value = parseFloat(input.value);
    console.log(value)
   
    if (value <= 0.0) {
        $(".amountValidationMessage").text("Amount should be greater than 0")
    }
    else if (value > 10000) {
        $(".amountValidationMessage").text("Amount should be greater than 0")
    } else {
        $(".amountValidationMessage").text("")
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

    nameValidation(firstName, 'First')
    nameValidation(lastname, 'Last')
    NumberValidation(mobileNumber)
    if ($(".numberValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateChangePasswordForm() {
    let oldPassword = document.getElementById("changepassword-old");
    let newPassword = document.getElementById("changepassword-new");
    let confPassword = document.getElementById("changepassword-confirm");

    curPasswordValidation(oldPassword)
    passwordValidation(newPassword)
    cnfpasswordValidation(confPassword, "changepassword-new")
    if ($(".passwordValidationMessage").html() == "" && $(".cnfpasswordValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateSaveBusinessForm() {
    let businessName = document.getElementById("business-businessName");
    let number = document.getElementById("business-mobileNumber");
    let GSTIN = document.getElementById("business-GSTIN");
    let pincode = document.getElementById("business-pincode");

    buisnessNameValidation(businessName)
    numberValidationReq(number)
    GSTINValidation(GSTIN);
    pincodeValidation(pincode);

    if ($(".emailValidationMessage").html() == "" && $(".numberValidationMessage").html() == "" && $(".FirstNameValidationMessage").html() == "" && $(".LastNameValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateSaveUserForm() {
    let email = document.getElementById("user-details-email")
    let firstname = document.getElementById("user-details-firstName")
    let lastname = document.getElementById("user-details-lastName")
    let mobile = document.getElementById("user-details-mobileNumber")

    emailValidation(email)
    nameValidation(firstname, 'First');
    nameValidation(lastname, 'Last')
    numberValidationReq(mobile);

    if ($(".businessNameValidationMessage").html() == "" && $(".numberValidationMessage").html() == "" && $(".GSTINValidationMessage").html() == "" && $(".pincodeValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }

}

//////////validate party forms//////////
function validateSavePartyForm() {
    let email = document.getElementById("party-email")
    let name = document.getElementById("party-name")
    let amount = document.getElementById("party-amount")

    emailValidation(email)
    partyNameValidation(name);
    MinAmount(amount)

    if ($(".amountValidationMessage").html() == "" && $(".emailValidationMessage").html() == "" && $(".partyNameValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }
}

function validateSaveTransactionForm() {
    let amount = document.getElementById("transaction-amount")
    MinAmountRequire(amount)

    if ($(".amountValidationMessage").html() == "") {
        return true;
    } else {
        return false;
    }

}

function RemoveValidations() {
    $(".validationMessage").html("");
}