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
            let htmlContent = ""
            if (parties.length == 0) {
                htmlContent = `<div class="d-flex flex-column justify-content-center align-items-center " style="height: calc(100vh - 365px);">
                        <div><img src="/images/users.png" alt="" height="150px"></div>
                        <div class="fs-4 text-secondary">No Party Selected</div>
                    </div>`
            } else {
                parties.forEach(element => {
                    htmlContent += ` <div class="d-flex justify-content-between border cursor-pointer p-2 party-list-div" id="party-id-${element.partyId}" title="Select party"
                        onclick="displaySelectedParyDetails(${element.partyId})" >
                        <div class="d-flex gap-3">
                            <div class="iamge">
                                <div class="d-flex justify-content-center align-items-center fs-1 rounded-circle " style="height: 50px !important; width: 50px !important; background-color: #e0f1f3;"><span class="pb-1 fs-4 fw-bold" style="color: #2F8891;">${element.partyName[0].toUpperCase()}</span></div>
                            </div>
                            <div class="name-time">
                                <div class="name fs-5 fw-semibold">${element.partyName}</div>
                                <div class="party-created-time-hidden d-none">`
                    if (element.updatedAt != null) {
                        htmlContent += `${element.updatedAt}`
                    } else if (element.createdAt != null) {
                        htmlContent += `${element.createdAt}`
                    }
                    htmlContent += `</div>
                                    <div class="text-secondary party-created-time">1 day ago</div>
                                </div>
                            </div>

                            <div class="amount-type">`
                    if (element.amount == 0) {
                        htmlContent += `<div class="fs-5 fw-bold text-end">₹${Math.Abs(element.amount)}</div><div></div>`
                    }
                    else {
                        if (element.transactionType == "GAVE") {
                            htmlContent += `<div class="fs-5 fw-bold text-end text-danger" > ₹${Math.abs(element.amount)}</div >
                                <div class="text-secondary">YOU'LL GET</div>`
                        } else {
                            htmlContent += `<div class="fs-5 fw-bold text-end text-success" > ₹${Math.abs(element.amount)}</div >
                                 <div class="text-secondary">YOU'LL GIVE</div>`
                        }
                    }
                    htmlContent += `</div>
                                </div>`;

                });
            }
            document.getElementById("partyList-partial").innerHTML = htmlContent;
            partyTimeDisplay();
        }
    }
}

function savePartySuccess(response) {
    if (response.isSuccess) {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage)
        }
        if (response.result != null) {
            $("#add-party-modal-title").html("Update Party");
            $(".party-amount-div").addClass("d-none")
            $("#party-amount").val("");
            $("#party-id").val(response.result.partyId);
        }
        displayPartyList();
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error")
        }
        $(".btn-close").click();
        displayPartyList()
        RemoveValidations();
        emptyInputValidation("save-party-form-id");
    }
}


//displays party details and transaction details
function partyDetailsSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let party = response.result;
            // $("#displaySelectedParty").html($("#party-details-div").html());
            let htmlContent = "";
            htmlContent += `    <div class="ps-3 pt-3">
                                    <div class="party-details d-flex justify-content-between align-items-center mt-1 ">
                                        <div class="d-flex justify-content-start align-items-center gap-2">
                                            <div class="d-flex justify-content-center align-items-center fs-1 rounded-circle "
                                                style="height: 50px !important; width: 50px !important; background-color: #e0f1f3;"><span
                                                    class="pb-1 fs-4 fw-bold"
                                                    style="color: #2F8891;">${party.partyName[0].toUpperCase()}</span></div>
                                            <div class=" fs-4 fw-bold pb-2">${party.partyName}</div>
                                        </div>
                                        <div><i class="fa-solid fa-pen cursor-pointer" data-bs-toggle="modal" data-bs-target="#party-modal"
                                                title="Update party" onclick="updateParty(${party.partyId})"
                                                data-partyId="${party.partyId}"></i>
                                        </div>
                                    </div>
                                    <div id="transaction-entries-id">

                                    </div>

                                    <div class=" d-flex justify-content-center mt-3 w-100">
                                        <div class="btn btn-danger gave-button-position me-3" data-bs-toggle="modal"
                                            data-bs-target="#transaction-entry-modal"
                                            onclick="displayAddTransactionModal(${party.partyId}, 'GAVE')">You Gave </div>
                                        <div class="btn btn-success got-button-position" data-bs-toggle="modal"
                                            data-bs-target="#transaction-entry-modal"
                                            onclick="displayAddTransactionModal(${party.partyId}, 'GOT')">You Got</div>
                                    </div>
                                </div>`
            $("#displaySelectedParty").html(htmlContent)
            displayTransactionEntries(party.partyId);
        }

    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error")
        }
    }
}

function transactionEntriesSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            console.log("hbhjfbcsjkd")
            let transactions = response.result.transactionsList
            let htmlContent = "";
            if (transactions.length == 0) {
                htmlContent += ` <div class="d-flex flex-column justify-content-center align-items-center h-100">
                                    <div><img src="~/images/users.png" alt="" height="150px"></div>
                                    <div class="fs-4 text-secondary">No entries Added</div>
                                </div>`
            } else {
                if (response.result.netBalance != 0 || (response.result.netBalance > 0 && response.result.ispartyVerified)) {
                    htmlContent += `<div class="net-balance d-flex justify-content-between  mt-3 mb-4">`
                } else {
                    htmlContent += `<div class="net-balance d-flex justify-content-end  mt-3 mb-4">`
                }
                if (response.result.netBalance != 0) {
                    htmlContent += `<div class="fs-5 d-flex flex-column flex-xl-row align-items-center" style="width: fit-content;">`
                    if (response.result.netBalance > 0 && response.result.ispartyVerified) {
                        htmlContent += `<div class="btn btn-outline-danger text-nowrap p-1" style="max-height: fit-content; width: 100%;"
                                    onclick="SendReminder(${response.result.netBalance}, ${response.result.transactionsList[0].partyId})"
                                    data-amount="${response.result.netBalance}" data-transactionId="${response.result.transactionsList[0].partyId}">Send reminder
                                </div>`
                    }
                    if (response.result.netBalance != 0) {
                        if (response.result.ispartyVerified) {
                            htmlContent += `<div class="btn btn-outline-danger text-nowrap p-1 mt-2 mt-xl-0 ms-xl-2" data-bs-toggle="modal" data-bs-target="#settleup-modal"
                                    onclick="SettleUpModal(${response.result.netBalance},${response.result.transactionsList[0].partyId})">Mark as settled</div>`
                        } else {
                            htmlContent += `<div class="btn btn-outline-danger text-nowrap p-1 mt-2 mt-xl-0 ms-0"  data-bs-toggle="modal" data-bs-target="#settleup-modal"
                                    onclick="SettleUpModal(${response.result.netBalance},${response.result.transactionsList[0].partyId})">Mark as settled</div>`
                        }

                    }
                    htmlContent += `</div>`
                }
                htmlContent += `<div class="fs-5">
                                    <div class="text-semibold text-secondary ">NET BALANCE:</div>`
                if (response.result.netBalance == 0) {
                    htmlContent += `<div class="text-end">Settled up: <span class="text-dark">₹${response.result.netBalance}</span></div>`
                }
                else if (response.result.netBalance < 0) {
                    htmlContent += `<div class="text-end">You"ll Give: <span class="text-success">₹${Math.abs(response.result.netBalance)}</span></div>`
                }
                else {
                    htmlContent += `<div class="text-end">You"ll Get: <span class="text-danger">₹${Math.abs(response.result.netBalance)}</span></div>`
                }
                htmlContent += `</div>
                            </div>
                            <div class="row border-bottom p-2">
                                <div class="col-4 text-secondary fw-bold">ENTRIES</div>
                                <div class="col-3 text-center text-secondary fw-bold">YOU GAVE</div>
                                <div class="col-3 text-center text-secondary fw-bold">YOU GOT</div>
                                <div class="col-2 text-center text-secondary fw-bold">DELETE</div>
                            </div>
                            <div class="p-0 transaction-entry-scroll">`

                transactions.forEach(entry => {
                    htmlContent += `<div class="row border-bottom p-2  cursor-pointer d-flex align-items-center" data-bs-toggle="modal" title="Update transaction"
                                data-bs-target="#transaction-entry-modal" onclick="displayUpdateTransactionModal(${entry.transactionId})">
                                <div class="col-4  d-flex flex-column">
                                    <div class="fw-bold entry-date"></div>
                                    <div class="fw-bold entry-date-hidden d-none">${entry.createDate}</div>
                                    <div class="text-secondary">BALANCE : ${entry.balance}</div>`
                    if (entry.dueDateString != null) {
                        htmlContent += `<div class="text-secondary"> Due Date : ${entry.dueDateString} </div>`
                    }
                    htmlContent += `</div>`
                    if (entry.TransactionType == 2) {
                        htmlContent += `<div class="col-3 text-center text-danger fw-bold fs-5">₹${entry.transactionAmount}</div>
                            <div class="col-3 text-center text-success fw-bold fs-5">-</div>`
                    }
                    else {
                        htmlContent += `<div class="col-3 text-center text-danger fw-bold fs-5">-</div>
                            <div class="col-3 text-center text-success fw-bold fs-5">₹${entry.transactionAmount}</div>`

                    }
                    htmlContent += `<div class="col-2 text-center text-secondry fw-bold fs-5"><i class="fa-solid fa-trash-can text-black"
                                        title="Delete transaction" data-bs-toggle="modal" data-bs-target="#delete-entry-modal"
                                        onclick="deleteTransactionId(${entry.transactionId})"></i></div>
                            </div>`
                })
                htmlContent += `</div>`
            }
        }
    } else {
        if (response.toasterMessage != null) {
            Toaster(response.toasterMessage, "error")
        }
    }
}