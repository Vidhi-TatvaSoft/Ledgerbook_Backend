function getBusinessesSuccess(response) {
    if (response.isSuccess) {
        if (response.result.length == 0) {
            document.getElementById("business-cards").innerHTML = `<div class="d-flex flex-column justify-content-center align-items-center fs-1 text-secondary mt-4 ">
                        <img src="/images/book-icon.png" class="cursor-pointer" height="200px"  title="Add business"  data-bs-toggle="modal" data-bs-target="#add-business-modal" onclick="displayBusinessDetails()">No Businesses
                    </div>`;
        } else {
            let cards = document.getElementById("business-cards");
            cards.innerHTML = "";
            response.result.forEach(element => {
                if (element.logoPath == null) {
                    element.logoPath = "/images/book-icon.png"
                } else {
                    element.logoPath = BASE_URL.replace("api", element.logoPath);
                }
                let innerhtml = `
                <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="card item-card border-0 shadow ">
                        <div class="card-body text-center p-3">
                            
                            <img src="${element.logoPath}" class="card-img-top rounded img-fluid mb-3 cursor-pointer" style="height: 100px; object-fit: contain;">
                            <h5 class="card-title fw-bold text-color">${element.businessName}</h5>
                            <div class="card-text d-flex justify-content-center px-3">
                                <div class="text-secondary text-truncate" style="max-width: 100% !important;"
                                    title="${element.ownerName}"><span class="text-dark fw-bold"> Owner : </span> ${element.ownerName}</div>
                            </div>

                            <div class="mt-3">
                                <div class="d-flex gap-2">
                                    <a class="btn btn-primary  w-100" onclick="manageBusinessDetails(${element.businessId})">
                                        Manage Business
                                    </a>`
                if (element.canEditDelete) {
                    innerhtml += `<button class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#add-business-modal" onclick="updateBusiness(${element.businessId})" title="Update business" data-BusinessId=${element.businessId}> <i class="fa-solid fa-pen"></i></button>`
                }
                if (element.canDelete) {
                    innerhtml += `<button class="btn btn-outline-danger" onclick="deleteBusinessmodal(${element.businessId})"
                            title="Delete business" data-bs-toggle="modal"
                            data-bs-target="#deleteBusiness-confirmation-modal" data-BusinessId=${element.businessId}><i
                                class="fa-solid fa-trash-can"></i></button>`
                }
                innerhtml += `</div>
                            </div>
                        </div>
                    </div>
                </div> `
                cards.innerHTML += innerhtml;
            });
        }
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function getBusinessByIdSuccess(response) {
    if (response.result != null) {
        if (response.result.businessCategories != null && response.result.businessTypes != null) {
            let categories = "";
            response.result.businessCategories.forEach(element => {
                let option = `<option value="business-category-${element.id}"> ${element.entityValue}</option> `
                categories += option
            })
            let types = "";
            response.result.businessTypes.forEach(element => {
                let option = `<option value="business-type-${element.id}"> ${element.entityValue}</option> `
                types += option
            })
            $("#business-categories").html(categories)
            $("#business-types").html(types)
        }
        let business = response.result;
        $("#business-businessId").val(business.businessId);
        $("#business-businessName").val(business.businessName);
        $("#business-mobileNumber").val(business.mobileNumber == 0 ? "" : business.mobileNumber);
        $("#business-addressLine1").val(business.addressLine1);
        $("#business-addressLine2").val(business.addressLine2);
        $("#business-city").val(business.city);
        $("#business-pincode").val(business.pincode);
        $("#business-GSTIN").val(business.gstin);
        if (business.isActive) {
            $("#business-isActive").attr("checked", "checked")
        } else {
            $("#business-isActive").removeAttr("checked")
        }
        if (business.businescategoryId != 0) {
            $("#business-categories").val(`business-category-${business.businescategoryId}`);
        } if (business.businessTypeId != 0) {
            $("#business-types").val(`business-type-${business.businessTypeId}`);
        }
        if (response.result.businessLogoAttachment != null && response.result.businessLogoAttachment.businesLogoPath != null) {
            $('#add-business-uploadedimage').attr('src', BASE_URL.replace("api", response.result.businessLogoAttachment.businesLogoPath)).width(50).height(50);
            document.getElementById("add-business-uploadedimage").classList.remove("d-none");
        }

    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
    }
}

//add business response success function
function saveBusinessSuccess(response) {
    if (response.isSuccess) {
            let businessId = $("#business-businessId").val() == "" ? 0 : $("#business-businessId").val();
            if (businessId == 0) {
                console.log(addBusinessClose)
                if(addBusinessClose){
                    if (response.toasterMessage != null) {
                        Toaster(response.toasterMessage);
                    }
                    addBusinessClose = false;
                }
                $("#user-details-confirmation-modal").modal("show");
            } else {
                console.log("hiiiiiiiiiiiiiiiiiiiiiiii",response.toasterMessage)
                if (response.toasterMessage != null) {
                    Toaster(response.toasterMessage);
                }
            }
        $("#add-business-modal-header").html("Update Business")
        $("#user-details-btn").removeClass("d-none")
        $("#business-image").val("");
        if (response.result.businessLogoAttachment != null && response.result.businessLogoAttachment.businesLogoPath != null) {
            $("add-business-uploadedimage").attr('src', response.result.businessLogoAttachment.businesLogoPath).width(50).height(50);
        }
        $("#business-businessId").val(response.result.businessId);
        if (response.result.isNewBusiness) {
            $("#user-details-confirmation-modal").modal("show");
        }
        displayBusinessCards();
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
        $("btn-close").click();
        displayBusinessCards();
    }
}

//user details success
function getAllUserDetailsSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            if (response.result.length == 0) {
                document.getElementById("user-details-table-body").innerHTML = `<tr>
                                        <td colspan="5" class="text-center fs-4 text-secondary fw-semibold">No data available...
                                        </td>
                                    </tr>`;
            } else {
                let userTableBody = document.getElementById("user-details-table-body");
                userTableBody.innerHTML = "";
                let innerHTML = "";
                response.result.forEach(element => {
                    innerHTML += `<tr class="">
                                            <td class="text-nowrap">${element.firstName} ${element.lastName}</td>
                                            <td class="text-nowrap">${element.email}</td>
                                            <td class="text-nowrap">${element.mobileNumber}</td>
                                            <td class="text-nowrap text-center">${element.roleName}
                                            </td>
                                            <td class="text-nowrap">`

                    if (element.canEdit) {
                        if (element.isActive) {
                            innerHTML += `<i class="fa-solid fa-circle-check text-green me-3 fs-5 cursor-pointer" data-bs-toggle="modal" data-bs-target="#inactive-user-modal" onclick="toggleActive(${element.userId})" id="active-user-${element.userId}" title="Inactivate user" ></i>`
                        } else {
                            innerHTML += `<i class="fa-solid fa-circle-xmark text-danger me-3 fs-5 cursor-pointer" data-bs-toggle="modal" data-bs-target="#inactive-user-modal" onclick="toggleActive(${element.userId})" id="active-user-${element.userId}" title="Activate user" ></i>`
                        }
                        innerHTML += ` 
                                        <i class="fa-solid fa-pen text-black me-3 mt-2 cursor-pointer" id="update-user-icon" data-bs-toggle="modal" data-bs-target="#add-user-modal" title="Update user" onclick="addEditUserInBusiness(${element.userId})" data-userId="${element.userId}"></i>
                                        <i class="fa-solid fa-trash-can text-danger cursor-pointer" data-bs-toggle="modal"
                                            title="Delete user" data-bs-target="#delete-user-modal"
                                            onclick="deleteUserModal(${element.userId})"></i>`
                    } else {
                        if (element.isActive) {
                            innerHTML += `<i class="fa-solid fa-circle-check text-green-50 me-3 fs-5 cursor-pointer" title="You dont have permission to edit this user"></i>`
                        } else {
                            innerHTML += `<i class="fa-solid fa-circle-xmark text-danger-50 me-3 fs-5 cursor-pointer" title="You dont have permission to edit this user"></i>`
                        }
                        innerHTML += `<i class="fa-solid fa-pen text-black-50 me-3 mt-2 cursor-pointer" title="You dont have permission to edit this user"></i>
                            <i class="fa-solid fa-trash-can text-danger-50 cursor-pointer" title="You dont have permission to delete this user"></i>`
                    }
                    innerHTML += `</td>
                                        </tr>`

                })
                userTableBody.innerHTML += innerHTML;

            }
        }
    }
    else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
        $("btn-close").click();
        displayBusinessCards();
    }
}

//when user click on update business button
function updateBusiness(businessId) {
    $("#business-businessId").val(businessId)
    displayBusinessDetails();
    $("#add-business-modal-header").html("Update Business")
    $("#user-details-btn").removeClass("d-none")
}

//function add user in business
function addEditUserInBusiness(userId) {
    emptyInputValidation('add-newuser-form-id');
    let businessId = $("#business-businessId").val();
    let params = setParameter("/Business/GetuserDetailById", GET, null, FORM_URL, { businessId: businessId, userId: userId }, getUserByIdSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function getUserByIdSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let userdetail = response.result;
            $("#user-details-userId").val(userdetail.userId);
            $("#user-details-personalId").val(userdetail.personalDetailId == null ? 0 : userdetail.personalDetailId);
            $("#user-details-lastName").val(userdetail.lastName);
            $("#user-details-firstName").val(userdetail.firstName);
            $("#user-details-email").val(userdetail.email);
            $("#user-details-mobileNumber").val(userdetail.mobileNumber == 0 ? "" : userdetail.mobileNumber);
            document.getElementById("user-details-roleList").innerHTML = "";
            userdetail.allRoles.forEach(element => {
                let innerHTML = "";
                innerHTML += `<div class="form-check me-2">
                            <input class="form-check-input user-detail-role-checkbox" type="checkbox" id="user-details-role-${element.roleId}"
                                data-roleId="${element.roleId}">
                            <label class="form-check-label" for="user-details-role-${element.roleId}">
                                ${element.roleName}
                            </label>
                        </div>`
                document.getElementById("user-details-roleList").innerHTML += innerHTML;
            })
            if (userdetail.userId != 0) {
                $("#user-details-email").prop("disabled", true);
                let selectedRoles = []
                for (j = 0; j < userdetail.roles.length; j++) {
                    selectedRoles.push(userdetail.roles[j].roleId);
                    $(`#user-details-role-${userdetail.roles[j].roleId}`).prop("checked", true);
                }
            } else {
                $("#user-details-email").prop("disabled", false);
            }

        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
    }
}

function saveUserDetails() {
    if (validateSaveUserForm()) {
        let roles = $(".user-detail-role-checkbox");
        let selectedRoles = [];
        for (i = 0; i < roles.length; i++) {
            if (roles[i].checked) {
                selectedRoles.push(roles[i].getAttribute("data-roleId"));
            }
        }
        if (selectedRoles.length == 0) {
            Toaster(constant.RoleRequireMessage, "error");
            return;
        }
        let UserDetails = {
            UserId: $("#user-details-userId").val(),
            PersonalDetailId: $("#user-details-personalId").val(),
            FirstName: $("#user-details-firstName").val(),
            lastName: $("#user-details-lastName").val(),
            Email: $("#user-details-email").val(),
            MobileNumber: $("#user-details-mobileNumber").val(),
        }
        let businessId = $("#business-businessId").val();
        let userDetailsViewModel = {
            UserDetails: UserDetails,
            SelectedRoles: selectedRoles,
            BusinessId: businessId
        }
        let params = setParameter("/Business/SaveUserDetails", POST, null, APPLICATION_JSON, JSON.stringify(userDetailsViewModel), saveUserDetailsSuccess);
        $("body").addClass("loading");
        ajaxCall(params);

    }
}

function saveUserDetailsSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
            $(".usermodal-close-btn").click();
            removeValidations();
            displayUserDetails();
        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
            $(".usermodal-close-btn").click();
            $(".btn-close").click();
            removeValidations();
        }
    }
}

//delete user modal display
function deleteUserModal(userId) {
    $("#delete-user-modal-userId").val(userId);
}

//delete user submit
function deleteUser() {
    let userId = $("#delete-user-modal-userId").val();
    let businessId = $("#business-businessId").val();
    let params = setParameter("/Business/DeleteUserFromBusiness", GET, null, FORM_URL, { userId: userId, businessId: businessId }, deleteUserSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function deleteUserSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
    }
    $(".user-delete-modal-close-btn").click();
    displayUserDetails();
}

function toggleActive(userId) {
    let userActiveIcon = document.getElementById(`active-user-${userId}`);
    if (userActiveIcon.classList.contains("fa-circle-check")) {
        let modalbody = document.getElementById("inactive-user-modal").innerHTML.toString().replace("{{status}}", "Inactive").replace("{{statusbody}}", "inactivate");
        document.getElementById("inactive-user-modal").innerHTML = modalbody
    } else {
        let modalbody = document.getElementById("inactive-user-modal").innerHTML.toString().replace("{{status}}", "Active").replace("{{statusbody}}", "activate");
        document.getElementById("inactive-user-modal").innerHTML = modalbody
    }
    $("#userId-active").val(userId);
}

function activeInactiveUser() {
    $("body").addClass("loading");
    let userId = $("#userId-active").val()
    let businessId = $("#business-businessId").val();
    let isActive = document.getElementById(`active-user-${userId}`).classList.contains("fa-circle-check") ? false : true
    let params = setParameter("/Business/activeInactiveUser", GET, null, FORM_URL, { userId: userId, isActive: isActive, businessId: businessId }, activeInactiveUserSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function activeInactiveUserSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
    }
    displayUserDetails();
    $(".user-inactive-modal-close-btn").click();

}

function deleteBusinessmodal(businessId) {
    $("#businessId-delete").val(businessId);
}

//delete business submit
function deleteBusinessYes() {
    let businessId = $("#businessId-delete").val();
    let params = setParameter(`/Business/DeleteBusiness/${businessId}`, POST, null, NODATA, null, deleteBusinessSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function deleteBusinessSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error");
        }
    }
    $(".user-delete-modal-close-btn").click();
    displayBusinessCards();
}

function closeBusinessModal() {
    $("#add-business-modal").modal("hide");
}

function setBusinessCookiesSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
        let cookies = response.result;
        if (cookies.businessId != null) {
            setCookie(Business_Id, cookies.businessId, 1);
        }
        if (cookies.businessToken != null) {
            setCookie(Business_Token, cookies.businessToken, 1);
        } if (cookies.allBusinesses != null) {
            setCookie(All_Businesses, cookies.allBusinesses, 1);
        }
        removeValidations();
        window.location = DASHBOARD_LINK;
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function manageBusinessDetails(businessId) {
    let params = setParameter(`/Business/GetBusinessData/${businessId}`, GET, null,NODATA, null, setBusinessCookiesSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}
