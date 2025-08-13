using Microsoft.AspNetCore.Authorization;

namespace LedgerBookWebApi.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string permission) : base()
    {
        Policy = $"{permission}";
    }
}
