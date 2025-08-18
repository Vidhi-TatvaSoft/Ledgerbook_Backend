function getPartyCountSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            $("#report-customer-count").html(response.result.customerCount)
            $("#report-supplier-count").html(response.result.supplierCount)
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function displayEntriesSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            $("#report-entry-gave").html(`₹ ${response.result.youGave}`)
            $("#report-entry-got").html(`₹ ${response.result.youGot}`)
            if (response.result.netBalance < 0) {
                $("#report-entry-net").addClass("text-danger");
            } else {
                $("#report-entry-net").addClass("text-success");
            }
            $("#report-entry-net").html(`₹ ${response.result.netBalance}`)

            let transactionList = response.result.transactionsList;
            let htmlContetnt = "";
            if (transactionList.length != 0) {
                transactionList.forEach(element => {
                    htmlContetnt += `<tr>
                            <td class="text-start text-nowrap">${element.createDate}</td>
                            <td class="text-start text-nowrap">${element.partyName}</td>
                            <td class="text-center text-nowrap">-</td>`

                    if (element.transactionType == 2) {
                        htmlContetnt += `<td class="text-end text-nowrap">₹ ${element.transactionAmount}</td>
                                <td class="text-end text-nowrap">₹ 0</td>`
                    }
                    else {
                        htmlContetnt += `<td class="text-end text-nowrap">₹ 0</td>
                                <td class="text-end text-nowrap">₹ ${element.transactionAmount}</td>`
                    }
                    htmlContetnt += `</tr>`
                });
            } else {
                htmlContetnt += `<tr>
                                <td colspan="5" class="text-center fs-4 ">
                                    No entries found
                                </td>
                            </tr>`
            }
            $("#report-transaction-entries").html(htmlContetnt)
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function getSerachOptionsSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let htmlContetnt = "";
            let parties = response.result;
            console.log(parties)
            parties.forEach(element => {
                htmlContetnt += `<div class="w-100 p-2 ps-2 border-bottom" data-partyid="${element.partyId}" onclick="searchByParty(${element.partyId},'${element.partyName}')">${element.partyName}</div>`
            });
            $("#party-search-options").html(htmlContetnt);
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}