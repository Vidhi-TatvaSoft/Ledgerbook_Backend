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

function GeneratePdf2() {
    // // const { jsPDF } = window.jspdf;
    // // const doc = new jsPDF();
    // // doc.text(element, 0, 0);
    // // doc.save("document2.pdf");


    // const { jsPDF } = window.jspdf;
    //     const doc = new jsPDF();

    //     doc.html(element, {
    //         callback: function (doc) {
    //             // Save the PDF
    //             doc.save('myDocument.pdf');
    //         },
    //         x: 0,
    //         y: 0,
    //         html2canvas: {
    //             // scale: 0.8 // Adjust scale for better rendering if needed
    //         }
    //     });
    const element = document.getElementById('abc').innerHTML;

    const options = {
        margin: [0, 0, 0, 0],
        filename: 'document.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 1.5 },
        jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
    };

    html2pdf().set(options).from(element).save();
}

function GeneratePdf() {
    console.log(searchTextId)
    $.ajax({
        url: `${BASE_URL}/Reports/GetReportPdfDatatest`,
        type: "POST",
        headers: {
            "Authorization": getCookie(User_Token),
            "BusinessToken": getCookie(Business_Token)
        },
        data: { partytype:partytype, htmlContent : JSON.stringify(document.getElementById("report-pdf-html").innerHTML) },
        xhrFields: {
            responseType: 'blob' //binary large object -- to handle binary response
        },
        success: function (data, status, xhr) {
            console.log("ok")
            let filename = `TransactionReport_${startDate}_to_${endDate}.xlsx`;
            
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
            Toaster("Something went wrong. Please try again later.", "error")
            
        }

    })
}


function GenerateExcel() {
    console.log(searchTextId)
    $.ajax({
        url: `${BASE_URL}/Reports/GetReportExcelData`,
        type: "GET",
        headers: {
            "Authorization": getCookie(User_Token),
            "BusinessToken": getCookie(Business_Token)
        },
        data: { partytype, timePeriod: $("#timeperiod-report").val(), searchPartyId: searchTextId, startDate, endDate },
        xhrFields: {
            responseType: 'blob' //binary large object -- to handle binary response
        },
        success: function (data, status, xhr) {

            let filename = `TransactionReport_${startDate}_to_${endDate}.xlsx`;
            
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
            Toaster("Something went wrong. Please try again later.", "error")
            
        }

    })
}