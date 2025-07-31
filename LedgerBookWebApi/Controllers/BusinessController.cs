using BusinessAcessLayer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class BusinessController : BaseController
{
    public BusinessController(
        ILoginService loginService,
        IActivityLogService activityLogService
    ) : base(loginService, activityLogService)
    {

    }
    
}
