
function getProfile(){
    $.ajax({
        url:"http://localhost:5207/api/User/getusers",
        type:"GET",
        success: function(res){
            console.log(res);
        }
    })
}

function Toaster(text, type = "success", closeButton = true, progressBar=true,timeOut="2000") {
    toastr.options = {
        "closeButton": closeButton,
        "progressBar": progressBar,
        "timeOut": timeOut
    };
    switch(type){
        case "sucess": toastr.success(text);
        break;      
        case "error": toastr.error(text);
        break;
        default: toastr.success(text);
    }
}