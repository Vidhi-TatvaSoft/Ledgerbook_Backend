function ManageParty(partyType) {
    setCookie(Party_Type, partyType, 1);
    window.location = "/Party/ManageBusiness";
}

function addPartyModal(){
    $("#save-party-modal-content").html($("#save-party-modal-innercontent").html());
    if($("#customer-layout-div").hasClass("d-none")){
        $("#supplier-radio").prop("checked",true);
        $("#customer-radio").parent().addClass("d-none");
    }else if ($("#supplier-layout-div").hasClass("d-none")){
        $("#supplier-radio").parent().addClass("d-none");
    }
    $(".party-amount-div").removeClass("d-none")
}

function saveParty(){
    if(validateSavePartyForm()){
        let formdata 
    }
}