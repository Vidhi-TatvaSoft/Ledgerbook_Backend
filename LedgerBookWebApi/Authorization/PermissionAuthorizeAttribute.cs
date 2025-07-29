using Microsoft.AspNetCore.Authorization;

namespace LedgerBook.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string permission) : base()
    {
        Policy = $"{permission}";
    }
}
