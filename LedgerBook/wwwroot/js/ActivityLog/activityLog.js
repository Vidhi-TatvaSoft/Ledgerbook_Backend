//display activities
function displayActivities(activityData) {
    $("#activities-display-div").html("");
    let params = setBusinessParameter("/ActivityLog/GetActivities", POST, null, APPLICATION_JSON, JSON.stringify(activityData), displayActivitySuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function displayActivitySuccess(response) {
    if (response.isSuccess) {
        if (response.result != null) {
            let activities = response.result.items;
            let htmlContent = ""
            if (activities.length != 0) {
                activities.forEach(activity => {
                    htmlContent += `<tr>
                                                <td class="text-nowrap ">${activity.activityDate}</td>
                                                <td class="text-nowrap text-center">${activity.activityTime}</td>
                                                <td class="text-nowrap">${activityMessage(activity)}</td>
                                            </tr>`
                })
            } else {
                htmlContent += `<tr>
                                    <td colspan="3" class="fs-4 fw-bold text-center">No activities found</td>
                                </tr>`
            }
            $("#activities-display-div").html(htmlContent)
        }
        currentPage = activityData.pageNumber;
        activityData.pageSize = parseInt(activityData.pageSize);
        var totalRecords = response.result.totalCount;
        $("#activity-totalCount").val(totalRecords);
        var startItem = totalRecords === 0 ? 0 : (currentPage - 1) * activityData.pageSize + 1;
        var endItem = Math.min(currentPage * activityData.pageSize, totalRecords);

        $("#pagination-text").text("Showing " + startItem + " - " + endItem + " of " + totalRecords);
        $("#left-arrow").prop("disabled", currentPage === 1);
        $("#right-arrow").prop("disabled", currentPage * activityData.pageSize >= totalRecords);
        $("#checkboxForSelectAllSection").prop("checked", false);
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function getActivityBusinesses() {
    let params = setBusinessParameter("/ActivityLog/GetAllBusiness", GET, null, NODATA, null, getActivityBusinessesSuccess);
    $("body").addClass("loading");
    ajaxCall(params);
}

function getActivityBusinessesSuccess(response) {
    if (response.isSuccess) {
        let htmlContent = "";
        htmlContent += `<option value="0">All</option>`
        if (response.result != null) {
            response.result.forEach(business => {
                htmlContent += `<option value="${business.businessId}">${business.busienssName}</option>`
            })
        }
        $("#business-select").html(htmlContent);
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function getActivityPartySuccess(response) {
    if (response.isSuccess) {
        let htmlContent = `<option value="0">All</option>`;
        if (response.result != null) {
            if (response.result != null) {
                response.result.forEach(party => {
                    htmlContent += `<option value="${party.id}">${party.partyName}</option>`
                })
            }
        }
        $("#party-entity-select").html(htmlContent);
    } else {
        if (response.toasterMessgae != null) {
            Toaster(response.toasterMessgae, "error");
        }
    }
}

function activityMessage(activity) {
    let message = "";
    switch (activity.entityTypeString) {
        case "User":
            switch (activity.actionString) {
                case "Add":
                    message = String.Format("{0} has been {1}.", "User", "added");
                    break;
                case "Update":
                    message = String.Format("{0} has been {1} by {2}.", "User", "updated", activity.createdByName);
                    break;
                case "Delete":
                    break;
            }
            break;
        case "Business":
            if (activity.subEntityTypeString != "") {
                switch (activity.subEntityTypeString) {
                    case "Party":
                        switch (activity.actionString) {
                            case "Add":
                                message = String.Format(PartyActivity, activity.partyType, activity.partyName, "added", activity.entityTypeName, activity.createdByName);
                                break;
                            case "Update":
                                message = String.Format(PartyUpdateActivity, activity.partyType, activity.partyName, activity.entityTypeName, "updated", activity.createdByName);
                                break;
                            case "Delete":
                                break;
                        }
                        break;
                    case "Transaction":
                        if (activity.isSettled) {
                            message = String.Format(TransactionActivity, activity.partyType, activity.partyName, activity.entityTypeName, "mark as paid", activity.createdByName);
                        } else {
                            switch (activity.actionString) {
                                case "Add":
                                    message = String.Format(TransactionAddActivity, activity.partyType, activity.partyName, activity.transactionAMount, activity.entityTypeName, activity.createdByName);
                                    break;
                                case "Update":
                                    message = String.Format(TransactionActivity, activity.partyType, activity.partyName, activity.entityTypeName, "updated", activity.createdByName);
                                    break;
                                case "Delete":
                                    message = String.Format(TransactionActivity, activity.partyType, activity.partyName, activity.entityTypeName, "deleted", activity.createdByName);
                                    break;
                            }
                        }
                        break;
                    case "Role":
                        switch (activity.actionString) {
                            case "Add":
                                message = String.Format(AddUserInBusinessActivity, activity.subEntityTypeName, activity.createdByName);
                                break;
                            case "Update":
                                if (activity.isActive || activity.isInactive) {
                                    if (activity.isActive) {
                                        message = String.Format(UserInBusinessActivity, activity.subEntityTypeName, "activated", activity.createdByName);

                                    } else {
                                        message = String.Format(UserInBusinessActivity, activity.subEntityTypeName, "inactivated", activity.createdByName);
                                    }
                                } else {
                                    message = String.Format(UserInBusinessActivity, activity.subEntityTypeName, "updated", activity.createdByName);
                                }
                                break;
                            case "Delete":
                                message = String.Format(UserInBusinessActivity, activity.subEntityTypeName, "deleted", activity.createdByName);
                                break;
                        }
                        break;
                }
            } else {
                switch (activity.actionString) {
                    case "Add":
                        message = String.Format(BusinessActivity, "Business", "added", activity.createdByName);
                        break;
                    case "Update":
                        message = String.Format(BusinessActivity, "Business", "updated", activity.createdByName);
                        break;
                    case "Delete":
                        message = String.Format(BusinessActivity, "Business", "deleted", activity.createdByName);
                        break;
                }
            }
            break;
    }
    return message;
}