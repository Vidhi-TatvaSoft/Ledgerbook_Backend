function getBusinessesSuccess(response) {
    if (response.isSuccess) {
        if (response.result.length == 0) {
            document.getElementById("businss-cards").innerHTML = `<div class="d-flex flex-column justify-content-center align-items-center fs-1 text-secondary mt-4 ">
                        <img src="/images/book-icon.png" class="cursor-pointer" height="200px"  title="Add business" onclick="addBusinessModal()">No Businesses
                    </div>`;
        } else {
            response.result.forEach(element => {
                let innerhtml = `
                <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="card item-card border-0 shadow ">
                        <div class="card-body text-center p-3">
                            
                            <img src="@((Business.LogoPath) == null ? "/images/book-icon.png" : Business.LogoPath)"
                                class="card-img-top rounded img-fluid mb-3 cursor-pointer" style="height: 100px; object-fit: contain;">
                            <h5 class="card-title fw-bold text-color">@Business.BusienssName</h5>
                            <div class="card-text d-flex justify-content-center px-3">
                                <div class="text-secondary text-truncate" style="max-width: 100% !important;"
                                    title="@Business.OwnerName"><span class="text-dark fw-bold"> Owner : </span> @Business.OwnerName</div>
                            </div>

                            <div class="mt-3">
                                <div class="d-flex gap-2">
                                    <a class="btn btn-primary  w-100" onclick="ManageBusinessDetails(@Business.BusinessId)">
                                        Manage Business
                                    </a>
                                    @if (Business.CanEditDelete)
                                    {
                                        <button class="btn btn-outline-primary" onclick="updateBusiness(@Business.BusinessId)"
                                            title="Update business" 
                                            data-BusinessId=@Business.BusinessId><i class="fa-solid fa-pen"></i></button>
                                        @if (Business.CanDelete)
                                        {
                                            <button class="btn btn-outline-danger" onclick="deleteBusinessmodal(@Business.BusinessId)"
                                                title="Delete business" data-bs-toggle="modal"
                                                data-bs-target="#deleteBusiness-confirmation-modal" data-BusinessId=@Business.BusinessId><i
                                                    class="fa-solid fa-trash-can"></i></button>
                                        }
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`
            });
        }
    } else {
        if (response.toasterMessage != null)
            Toaster(response.toasterMessage, "error");
    }
}

function getBusinessByIdSuccess(response){
    console.log(response);
    if(response.httpStatusCode == 206 && response.result.businessCategories != null && response.result.businessTypes != null ){
        let categories = "";
        response.result.businessCategories.forEach(element => {
            let option = `<option value="business-category-${element.id}">${element.entityValue}</option>`
            categories += option
        })
        let types = "";
        response.result.businessTypes.forEach(element => {
            let option = `<option value="business-type-${element.id}">${element.entityValue}</option>`
            types += option
        })
        $("#business-categories").html(categories)
        $("#business-types").html(types)
    }
    if(response.isSuccess){
        if(response.result != null){
            let business = response.result;
            $("#business-businessId").val(business.businessId);
            $("#business-businessName").val(business.businessName);
            $("#business-mobileNumber").val(business.businessId);
            $("#business-addressLine1").val(business.addressLine1);
            $("#business-addressLine2").val(business.addressLine2);
            $("#business-city").val(business.city);
            $("#business-pincode").val(business.pincode);
            $("#business-GSTIN").val(business.gSTIN);
            $("#business-isActive").val(business.isActive);
            $("#business-categories").val(`business-category-${business.businescategoryId}`);
            $("#business-types").val(`business-type-${business.businesTypeId}`);
        }
    }
}

function SaveBusinessSuccess(response){
    if(response.isSuccess){

    }
}