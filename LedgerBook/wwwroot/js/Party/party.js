function ManageParty(partyType) {
    setCookie(Party_Type, partyType, 1);
    window.location = "/Party/ManageBusiness";
}

function displayTotalAmounts() {
    let params = setBusinessParameter("/Party/GetTotalAmount", GET, null, FORM_URL, { partyType: getCookie(Party_Type) }, getTotalsSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function addPartyModal() {
    $("#save-party-modal-content").html($("#save-party-modal-innercontent").html());
    $("#add-party-modal-title").html("Add Party");
    if ($("#customer-layout-div").hasClass("d-none")) {
        $("#supplier-radio").prop("checked", true);
        $("#customer-radio").parent().addClass("d-none");
    } else if ($("#supplier-layout-div").hasClass("d-none")) {
        $("#supplier-radio").parent().addClass("d-none");
    } else {
        if (getCookie(Party_Type) == "Customer") {
            $("#customer-radio").prop("checked", true);
        } else {
            $("#supplier-radio").prop("checked", true);
        }
    }
    $(".party-amount-div").removeClass("d-none")
}

function saveParty(close) {
    if (validateSavePartyForm()) {
        let formData = new FormData();
        formData.append("PartyId", $("#party-id").val() == "" ? 0 : $("#party-id").val())
        formData.append("PartyName", $("#party-name").val())
        formData.append("Email", $("#party-email").val())
        formData.append("BusinessId", parseInt($("#currentBusinessId").html()))
        formData.append("Amount", $("#party-amount").val())
        formData.append("TransactionType", $("#party-transactionType").val())
        if ($("#customer-radio").prop("checked")) {
            formData.append("PartyTypeString", "Customer")
        } else {
            formData.append("PartyTypeString", "Supplier")
        }
        let params = setBusinessParameter("/Party/SaveParty", POST, null, FORMDATA, formData, savePartySuccess);
        $("body").addClass("loading");
        ajaxCall(params);
        if(close){
            ClosePartyModal()
        }
    }
}

function ClosePartyModal() {
    $("#save-party-modal").modal("hide")
}

function closeTransactionModal() {
    $("#transaction-entry-modal").modal("hide")
}

function closeDelteTransactionModal() {
    $("#delete-entry-modal").modal("hide")
}

function closeSettleUpModal() {
    $("#settleup-modal").modal("hide")
}

//display details of selected party
function displaySelectedParyDetails(partyId) {
    let params = setBusinessParameter("/Party/GetPartyDetails", GET, null, FORM_URL, { partyId: partyId }, partyDetailsSuccess);
    $("body").addClass("loading");
    ajaxCall(params);

    let parties = document.getElementsByClassName("party-list-div");
    for (i = 0; i < parties.length; i++) {
        parties[i].classList.remove("selected-party")
    }
    setTimeout(function () {
        document.getElementById(`party-id-${partyId}`).classList.add("selected-party");
    }, 300);
}

//display transaction entries of selected parties
function displayTransactionEntries(partyId) {
    let params = setBusinessParameter("/Party/GetTransationEntries", GET, null, FORM_URL, { partyId: partyId }, transactionEntriesSuccess);
    console.log(params)
    $("body").addClass("loading");
    ajaxCall(params);
}

function updateParty(partyId) {
    $("#save-party-modal-content").html($("#save-party-modal-innercontent").html());
    $("#add-party-modal-title").html("Update Party");
    if ($("#customer-layout-div").hasClass("d-none")) {
        $("#supplier-radio").prop("checked", true);
        $("#customer-radio").parent().addClass("d-none");
    } else if ($("#supplier-layout-div").hasClass("d-none")) {
        $("#supplier-radio").parent().addClass("d-none");
    }
    $(".party-amount-div").addClass("d-none")
    let params = setBusinessParameter("/Party/GetPartyDetails", GET, null, FORM_URL, { partyId: partyId }, getPartyDetailsToUpdateSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

//display time
function partyTimeDisplay() {
    var partyTimeHidden = document.getElementsByClassName("party-created-time-hidden");
    var partyTime = document.getElementsByClassName("party-created-time");
    for (i = 0; i < partyTimeHidden.length; i++) {
        var curdate = Date.now();
        var partyDate = partyTimeHidden[i].parentElement.children[1].innerHTML;
        var Dateonly = partyDate.split("T");
        var partyDay = Dateonly[0].split("-")[2];
        var partyMonth = Dateonly[0].split("-")[1];
        var partyYear = Dateonly[0].split("-")[0];
        var partyDateformated = partyYear + "-" + partyMonth + "-" + partyDay + "T" + Dateonly[1];
        var partyDateToCal = new Date(partyDateformated);
        var timeDiff = curdate - partyDateToCal;

        const timeDiffYear = Math.floor(timeDiff / (12 * 30 * 24 * 60 * 60 * 1000));
        const timeDiffMonth = Math.floor(timeDiff / (30 * 24 * 60 * 60 * 1000));
        const timeDiffday = Math.floor(timeDiff / (24 * 60 * 60 * 1000));
        const daysms = timeDiff % (24 * 60 * 60 * 1000);
        const timeDiffHours = Math.floor(daysms / (60 * 60 * 1000));
        const hoursms = timeDiff % (60 * 60 * 1000);
        const timeDiffMins = Math.floor(hoursms / (60 * 1000));
        const minutesms = timeDiff % (60 * 1000);
        const timeDiffSecs = Math.floor(minutesms / 1000);
        if (timeDiffYear != 0) {
            if (timeDiffYear == 1)
                partyTime[i].innerHTML = "a year ago"
            else
                partyTime[i].innerHTML = timeDiffYear + " year ago"
        }
        else if (timeDiffMonth != 0) {
            if (timeDiffMonth == 1)
                partyTime[i].innerHTML = "a month ago"
            else
                partyTime[i].innerHTML = timeDiffMonth + " months ago"
        } else if (timeDiffday != 0) {
            if (timeDiffday == 1)
                partyTime[i].innerHTML = "a day ago"
            else
                partyTime[i].innerHTML = timeDiffday + " days ago"
        } else if (timeDiffHours != 0) {
            if (timeDiffHours == 1)
                partyTime[i].innerHTML = "an hour ago"
            else
                partyTime[i].innerHTML = timeDiffHours + " hours ago"
        } else if (timeDiffMins != 0) {
            if (timeDiffMins == 1)
                partyTime[i].innerHTML = "a minute ago"
            else
                partyTime[i].innerHTML = timeDiffMins + " minutes ago"
        }
        else {
            partyTime[i].innerHTML = "a few seconds ago"
        }
    }
}

function displayAddTransactionModal(partyId, transactionType) {
    let params = setBusinessParameter("/Party/GetTransactionDetailById", GET, null, FORM_URL, { partyId: partyId, transactionType: transactionType }, AddTransactionModalSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function displayUpdateTransactionModal(partyId, transactionId) {
    let params = setBusinessParameter("/Party/GetTransactionDetailById", GET, null, FORM_URL, { partyId: partyId, transactionId: transactionId }, UpdateTransactionModalSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function deleteTransactionId(transactionId) {
    document.getElementById("delete-transaction-id").value = transactionId;
}

//delete transaction ajax
function deleteTransaction() {
    let transactionId = document.getElementById("delete-transaction-id").value;
    let params = setBusinessParameter("/Party/DeleteTransaction", POST, null, FORM_URL, { transactionId: transactionId }, deleteTransactionSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

//display modal fro settleup
function settleUpModal(netBalance, partyId) {
    $("#settleup-partyId").val(partyId);
    $("#settleup-netBalance").val(netBalance);
    $("#settleup-amount").html(Math.abs(netBalance).toString())
    if (netBalance < 0) {
        $("#settleup-type").html("You Paid")
    } else {
        $("#settleup-type").html("You Recieved")
    }
}

//settle up for party
function settleUp() {
    let netBalance = $("#settleup-netBalance").val();
    let partyId = $("#settleup-partyId").val();
    let params = setBusinessParameter("/Party/SettleUpParty", POST, null, FORM_URL, { netBalance: netBalance, partyId: partyId }, settleUpSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

//send reminder to party through email
function SendReminder(netBalance, partyId) {
    let params = setBusinessParameter("/Party/SettleUpParty", GET, null, FORM_URL, { netBalance: netBalance, partyId: partyId }, sendReminderSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}