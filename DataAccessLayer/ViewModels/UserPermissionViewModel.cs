namespace DataAccessLayer.ViewModels;

public class UserPermissionViewModel
{
    public List<BusinessViewModel> BusinessList { get; set; }
    public UserViewmodel UserDetail { get; set; }
    public List<UserViewmodel> Users { get; set; }
    public List<int> RoleId { get; set; }
}