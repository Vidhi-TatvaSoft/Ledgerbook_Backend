function rolePermissionSuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage);
        }
        let cookies = response.result;
        if (cookies.customerPermission) {
            $("#customer-layout-div").removeClass("d-none")
        } if (cookies.supplierPermission) {
            $("#supplier-layout-div").removeClass("d-none")
        }
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function getAllPartiesSuccess(response) {
    console.log(response)
    if (response.isSuccess) {
        console.log("hii")
        if (response.result != null) {
            let parties = response.result;
            let htmlContent;
            if (parties.length == 0) {
                 htmlContent = `<div class="d-flex flex-column justify-content-center align-items-center " style="height: calc(100vh - 365px);">
                        <div><img src="/images/users.png" alt="" height="150px"></div>
                        <div class="fs-4 text-secondary">No Party Selected</div>
                    </div>`
            }
            document.getElementById("partyList-partial").innerHTML = htmlContent;
        }
    }
}