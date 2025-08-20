function getDashboardData() {
    let params = setBusinessParameter("/Dashboard/GetDashboardData", GET, null, NODATA, null, getDashboardDataSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function getDashboardDataSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let dashboard = response.result;
            $("#dashboard-total-supplier").html(dashboard.totalSupplier)
            $("#dashboard-total-customer").html(dashboard.totalCustomer)

            //customer data
            let customerhtml = "";
            if (dashboard.customerAmount == 0) {
                customerhtml += `<div class="text-secondary fs-5">SETTLED UP</div>
                                                                    <div class="fs-5 fw-bold text-end ms-2">₹ ${Math.abs(dashboard.customerAmount)}</div>`
            }
            else {
                if (dashboard.customerType.toString() == 2) {
                    customerhtml += `<div class="text-secondary fs-5">YOU'LL GET</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-danger">₹ ${Math.abs(dashboard.customerAmount)}</div>`
                } else {
                    customerhtml += `<div class="text-secondary fs-5">YOU'LL GIVE</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-success">₹ ${Math.abs(dashboard.customerAmount)}</div>`
                }
            }
            $("#dashboad-customer-amount").html(customerhtml);

            //supplier data
            let supplierhtml = "";
            if (dashboard.supplierAmount == 0) {
                supplierhtml += `<div class="text-secondary fs-5">SETTLED UP</div>
                                                                    <div class="fs-5 fw-bold text-end ms-2">₹ ${Math.abs(dashboard.supplierAmount)}</div>`
            }
            else {
                if (dashboard.supplierType.toString() == 2) {
                    supplierhtml += `<div class="text-secondary fs-5">YOU'LL GET</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-danger">₹ ${Math.abs(dashboard.supplierAmount)}</div>`
                } else {
                    supplierhtml += `<div class="text-secondary fs-5">YOU'LL GIVE</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-success">₹ ${Math.abs(dashboard.supplierAmount)}</div>`
                }
            }
            $("#dashboad-supplier-amount").html(supplierhtml);

            //netbalance data
            let balanvehtml = "";
            if (dashboard.netBalance == 0) {
                balanvehtml += `<div class="text-secondary fs-5">SETTLED UP</div>
                                                                    <div class="fs-5 fw-bold text-end ms-2">₹ ${Math.abs(dashboard.netBalance)}</div>`
            }
            else {
                if (dashboard.netBalanceTye.toString() == 2) {
                    balanvehtml += `<div class="text-secondary fs-5">YOU'LL GET</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-danger">₹ ${Math.abs(dashboard.netBalance)}</div>`
                } else {
                    balanvehtml += `<div class="text-secondary fs-5">YOU'LL GIVE</div>
                                                                    <div class="fs-5 ms-2 fw-bold text-end text-success">₹ ${Math.abs(dashboard.netBalance)}</div>`
                }
            }
            $("#dashboard-netBalance-amount").html(balanvehtml);

            //top party
            let topPartyHtml = "";
            if (dashboard.topParty.length != 0) {
                for (i = 0; i < dashboard.topParty.length; i++) {
                    topPartyHtml += `<div class="d-flex justify-content-between align-items-center">
                                                                    <div>
                                                                        <div class="fs-5 text-truncate" title="${dashboard.topParty[i].partyName}" style="max-width: 10vw;"><span class="badge bg-secondary rounded-circle me-2" title="${dashboard.topParty[i].partyTypeString}">${dashboard.topParty[i].partyTypeString[0]}</span>${dashboard.topParty[i].partyName}</div>
                                                                    </div>`
                    if (Math.abs(dashboard.topParty[i].amount) == 0) {
                        topPartyHtml += `<div class="fs-5 fw-bold text-dark">₹ ${Math.abs(dashboard.topParty[i].amount)}</div></div>`
                    }
                    else if (dashboard.topParty[i].transactionType == 2) {
                        topPartyHtml += `<div class="fs-5 fw-bold text-danger">₹ ${Math.abs(dashboard.topParty[i].amount)}</div></div>`
                    } else {
                        topPartyHtml += `<div class="fs-5 fw-bold text-success">₹ ${Math.abs(dashboard.topParty[i].amount)}</div></div>`
                    }
                    if (i < dashboard.topParty.length - 1 || dashboard.topParty.length < 5) {
                        topPartyHtml += `<hr>`
                    }
                }
            }
            else {
                topPartyHtml += `<div class="d-flex justify-content-center align-items-center h-50">
                                                <div class="fs-5 text-center">No party found</div>
                                            </div>`
            }
            $("#dashboard-topparty").html(topPartyHtml);

            let recentTransactionHtml = "";
            if (dashboard.recentTransaction.length != 0) {
                for (i = 0; i < dashboard.recentTransaction.length; i++) {
                    recentTransactionHtml += `<div class="d-flex justify-content-between align-items-center">
                                                <div class="d-flex align-items-center">
                                                    <div class="fs-5 fw-bold me-2 text-truncate" title="${dashboard.recentTransaction[i].partyName}" style="max-width: 10vw;"><span class="badge bg-secondary rounded-circle me-2" title="${dashboard.recentTransaction[i].partyTypeString}">${dashboard.recentTransaction[i].partyTypeString[0]}</span>${dashboard.recentTransaction[i].partyName}</div>
                                                    <div> - ${dashboard.recentTransaction[i].createDate}</div>
                                                </div>`
                    if (dashboard.recentTransaction[i].transactionType == 2) {
                        recentTransactionHtml += `<div
                                                    class="fs-5 fw-bold text-danger">₹ ${dashboard.recentTransaction[i].transactionAmount}</div>
                                            </div>`
                    } else {
                        recentTransactionHtml += `<div
                                                    class="fs-5 fw-bold text-success">₹ ${dashboard.recentTransaction[i].transactionAmount}</div>
                                            </div>`
                    }
                    if (i < 2) {
                        recentTransactionHtml += `<hr>`
                    }
                }
            }
            else {
                recentTransactionHtml += `<div class="fs-5 text-center mt-5">No transaction found</div>`
            }
            $("#dashboard-recent-transaction").html(recentTransactionHtml)

            let dueHtml = "";
            if (dashboard.upcomingDue.length != 0) {
                for (i = 0; i < dashboard.upcomingDue.length; i++) {
                    dueHtml += `<div class="d-flex justify-content-between align-items-center">
                                        <div class="d-flex align-items-center">
                                            <div class="fs-5 me-2 fw-bold text-truncate" title="${dashboard.upcomingDue[i].partyName}" style="max-width: 10vw;"><span class="badge bg-secondary rounded-circle me-2" title="${dashboard.upcomingDue[i].partyTypeString}">${dashboard.upcomingDue[i].partyTypeString[0]}</span>${dashboard.upcomingDue[i].partyName}</div>
                                            <div> - Due Date: ${dashboard.upcomingDue[i].dueDateString}</div>
                                        </div>`

                    if (dashboard.upcomingDue[i].transactionType == 2) {
                        dueHtml += `<div
                                            class="fs-5 fw-bold text-danger">₹ ${dashboard.upcomingDue[i].transactionAmount}</div>
                                    </div>`
                    } else {
                        dueHtml += `<div
                                            class="fs-5 fw-bold text-success">₹ ${dashboard.upcomingDue[i].transactionAmount}</div>
                                    </div>`
                    }
                    if (i < dashboard.upcomingDue.length - 1 || dashboard.upcomingDue.length < 2) {
                        dueHtml += `<hr>`
                    }
                }
            }
            else {
                dueHtml += `<div class="fs-5 text-center mt-5">No transaction found</div>`
            }
            $("#dashboard-duedate").html(dueHtml)
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function partyRevenueSetColors(partyRevenueTemp) {
    partyRevenue = partyRevenueTemp;
    for (i = 0; i < partyRevenue.length; i++) {
        if (partyRevenue[i] < 0) {
            backgroundColor.push("rgb(255,0,0,0.2)");
            borderColor.push("rgb(255,0,0,1)")
            partyRevenue[i] = Math.abs(partyRevenue[i]);
        } else {
            backgroundColor.push("rgb(13,141,14,0.2)");
            borderColor.push("rgb(13,141,14,1)")
        }
    }
    showGraphs();
}

function getPartyrevenue(year) {
    let params = setBusinessParameter("/Dashboard/GetGraphDetails", GET, null, FORM_URL, { year: year.toString() }, getPartyRevenueSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function getPartyRevenueSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            partyRevenueSetColors(response.result)
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error")
        }
    }
}

function getYears() {
    let params = setBusinessParameter("/Dashboard/GetYearsForRevenue", GET, null, NODATA, null, getYearsSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function getYearsSuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            years = response.result;
            const currentDate = new Date();
            var yearSelector = document.getElementById("year-for-party-revenue")
            if (years.length <= 0) {
                years.push(currentDate.getFullYear().toString());
            }

            for (i = 0; i < years.length; i++) {
                yearSelector.innerHTML += `<option value="${years[i]}">${years[i]}</option>`
            }
            yearForGraph = currentDate.getFullYear().toString();

            if (!years.includes(yearForGraph)) {
                yearForGraph = years[0];
            }
            document.getElementById("year-for-party-revenue").value = yearForGraph;
            getPartyrevenue(yearForGraph);
        }
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error")
        }
    }
}

function showGraphs() {
    // Destroy all existing chart instances
    Chart.helpers.each(Chart.instances, function (instance) {
        instance.destroy();
    });
    var ctx = document.getElementById("ChartId").getContext("2d");

    // Revenue chart
    RevenueChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: Labels,
            datasets: [{
                label: "Revenue (₹)",
                data: partyRevenue,
                backgroundColor: backgroundColor,
                borderColor: borderColor,
                borderWidth: 2,
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    title: {
                        display: true,
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });

}
