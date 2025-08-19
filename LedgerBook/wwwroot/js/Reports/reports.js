function getPartyCounts() {
    let params = setBusinessParameter("/Reports/GetPartyCounts", GET, null, NODATA, null, getPartyCountSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function SelectOption(tag) {
    partytype = tag.getAttribute("data-party");
    var allclasses = document.getElementsByClassName("allParty");
    for (var i = 0; i < allclasses.length; i++) {
        allclasses[i].classList.remove("party-selected");
    }
    tag.classList.add("party-selected");
    $("#partySearch-report").val("");
    $("#timeperiod-report").val("This year");
    displayReportTransactionEntries(partytype, searchTextId, startDate, endDate)
}

function displayReportTransactionEntries(partyType, searchTextId, startDate, endDate) {
    let params = setBusinessParameter("/Reports/GetReportTransactionEntries", GET, null, FORM_URL, { partyType, searchPartyId: searchTextId, startDate, endDate }, displayEntriesSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}

//serach party and transaction by party
function searchByParty(partyId, partyName) {
    console.log("hiii")
    $("#partySearch-report").val(partyName);
    searchTextId = partyId;
    console.log(partyId)
    displayReportTransactionEntries(partytype, searchTextId, startDate, endDate)

}

function GeneratePdf() {
    let params = setBusinessParameter("/Reports/GetReportPdfData", GET, null, FORM_URL, { partytype, timePeriod: $("#timeperiod-report").val(), searchPartyId: searchTextId, startDate, endDate }, generatePdfSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function GeneratePdf2() {
    console.log("oko", searchTextId)
    $.ajax({
        url: `${BASE_URL}/Reports/GetReportPdfDatatest`,
        type: "GET",
        headers: {
            "Authorization": getCookie(User_Token),
            "BusinessToken": getCookie(Business_Token)
        },
        contentType: "application/x-www-form-urlencoded",
        processData: true,
        data: { partytype: partytype },
        xhrFields: {
            responseType: 'blob' //binary large object -- to handle binary response
        },
        success: function (data, status, xhr) {
            console.log("ok")
            let filename = `TransactionReport.pdf`;

            let disposition = xhr.getResponseHeader('Content-Disposition');
            console.log(disposition)
            if (disposition && disposition.indexOf('filename') !== -1) {
                let matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition); //ExportOrderDataToExcel filename From disposition
                if (matches && matches[1]) {
                    filename = matches[1].replace(/['"]/g, ''); // Remove quotes if present
                }
            }

            let blob = new Blob([data], { type: xhr.getResponseHeader('Content-Type') });
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob); //timePeriod url points to blob obj
            link.download = filename; //Download file name
            document.body.appendChild(link); //appendChild so that it Cancelled be cliked
            link.click();
            document.body.removeChild(link);

            console.log("Export Successfully");

        }, error: function (res) {
            console.log(res)
            Toaster("Something went wrong. Please try again later.", "error")

        }

    })
}

function GenerateExcel() {
    console.log(searchTextId)
    let startDateTemp = convertDateFormat(startDate)
    let endDateTemp = convertDateFormat(endDate);
    $.ajax({
        url: `${BASE_URL}/Reports/GetReportExcelData`,
        type: "GET",
        headers: {
            "Authorization": getCookie(User_Token),
            "BusinessToken": getCookie(Business_Token)
        },
        data: { partytype, timePeriod: $("#timeperiod-report").val(), searchPartyId: searchTextId, startDate, endDate },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data, status, xhr) {

            let filename = `TransactionReport_${startDateTemp}_To_${endDateTemp}.xlsx`;

            let disposition = xhr.getResponseHeader('Content-Disposition');
            console.log(disposition)
            if (disposition && disposition.indexOf('filename') !== -1) {
                let matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition); 
                if (matches && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }

            let blob = new Blob([data], { type: xhr.getResponseHeader('Content-Type') });
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob); 
            link.download = filename; 
            document.body.appendChild(link); 
            link.click();
            document.body.removeChild(link);

            console.log("Export Successfully");

        }, error: function (res) {
            Toaster("Something went wrong. Please try again later.", "error")

        }

    })
}


function convertDateFormat(dateString) {
    const date = new Date(dateString);
  
    const day = date.getDate();
    const year = date.getFullYear();
    const monthNames = [
      "January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    const month = monthNames[date.getMonth()];
  
    return `${day} ${month} ${year}`;
  }