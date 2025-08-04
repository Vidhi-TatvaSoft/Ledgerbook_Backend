const FORM_URL = "application/x-www-form-urlencoded";
const APPLICATION_JSON = "application/json";
const MULTIPART_FORMDATA = "multipart/form-data";
const FORMDATA = "false";
const NODATA = "no data";
const GET = "GET";
const POST = "POST";

function setParameter(url, type, headers, contentType, data, successFunction) {
    data = data == null ? null : data;
    url = `${BASE_URL}${url}`
    if (headers == null) {
        headers = {
            "UserToken": getCookie(User_Token)
        }
    }

    return {
        url: url,
        type: type,
        data: data,
        headers: headers,
        contentType: contentType,
        successFunction: successFunction
    }
}

function ajaxCall(params) {

    let ajaxcallObj = {
        url: `${params.url}`,
        type: params.type,
        data: params.data,
        headers: params.headers,
        contentType: params.contentType,
        processData: false,
        success: function (response) {
            params.successFunction(response);
        },
        error: function (response) {
            if (response.toasterMessage != null) {
                Toaster(response.toasterMessage, "error")
            }
        }
    }

    switch (params.contentType) {
        case FORM_URL:
            ajaxcallObj.contentType = params.contentType;
            ajaxcallObj.processData = true;
            ajaxcallObj.data = $.param(params.data);
            break;
        case FORMDATA:
            ajaxcallObj.contentType = false;
            ajaxcallObj.processData = false;
            ajaxcallObj.data = params.data;
            break;
        case NODATA:
            ajaxcallObj.contentType = false;
            ajaxcallObj.processData = false;
        default:
            break;
    }

    if (params.data == null) {
        ajaxcallObj = {
            url: `${params.url}`,
            type: params.type,
            headers: params.headers,
            success: function (response) {
                params.successFunction(response);
            },
            error: function (response) {
                if (response.toasterMessage != null) {
                    Toaster(response.toasterMessage, "error")
                }
            }
        }
    }

    console.log("ajax", ajaxcallObj);

    $.ajax(ajaxcallObj);
}