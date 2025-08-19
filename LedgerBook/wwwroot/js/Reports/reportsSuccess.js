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

function generatePdfSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let reportdata = response.result;
            console.log(reportdata)
            $("#reportpdf-business-name").html(reportdata.businessname)
            $("#reportpdf-startdate").html(reportdata.startdate)
            $("#reportpdf-enddate").html(reportdata.endDate)
            $(".reportpdf-youGave").html(reportdata.youGave)
            $(".reportpdf-youGot").html(reportdata.youGot)
            $("#reportpdf-net").html(reportdata.netBalance)
            $("#reportpdf-partyName").html(reportdata.partyName)
            $("#reportpdf-timePeriod").html(reportdata.timePeriod)
            $("#reportpdf-count").html(reportdata.transactionsList.length)

            let entries = reportdata.transactionsList;
            let htmlContetnt = "";
            if (entries.length != 0) {
                entries.forEach(element => {
                    htmlContetnt += `<tr>
                            <td style="text-align: center;  border: 2px solid gray; padding: 10px;">${element.createDate}</td>
                            <td style="text-align: center;  border: 2px solid gray; padding: 10px;">${element.partyName}</td>
                            <td style="text-align: center;  border: 2px solid gray; padding: 10px;">${element.description}</td>`
                    if (element.transactionType == 2) {
                        htmlContetnt += `<td  style="text-align: center;  border: 2px solid gray; padding: 10px; color: gray; background-color: rgb(252, 240, 240);">${element.transactionAmount}</td>
                                <td style="text-align: center;  border: 2px solid gray; padding: 10px; color: gray; background-color: rgb(218, 245, 238);">-</td>`
                    } else {
                        htmlContetnt += ` <td  style="text-align: center;  border: 2px solid gray; padding: 10px; color: gray; background-color: rgb(252, 240, 240);">-</td>
                                <td style="text-align: center;  border: 2px solid gray; padding: 10px; color: gray; background-color: rgb(218, 245, 238);">${element.transactionAmount}</td>`
                    }
                    htmlContetnt += `</tr>`
                })
            }else{
                htmlContetnt += `<tr>
                        <td colspan="5" class="text-center " style="font-size: 20px; padding: 10px;">
                            No entries found
                        </td>
                    </tr>`
            }
            $("#reportpdf-entries").html (htmlContetnt);

            const element = document.getElementById('report-pdf-html').innerHTML;

            const options = {
                margin: [0, 0, 0, 0],
                filename: `TransactionReport_${reportdata.startdate}_To_${reportdata.endDate}`,
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 1.5 },
                jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
            };

            html2pdf().set(options).from(element).save();

        }
    }
}