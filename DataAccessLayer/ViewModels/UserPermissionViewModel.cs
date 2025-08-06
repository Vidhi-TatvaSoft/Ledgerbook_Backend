namespace DataAccessLayer.ViewModels;

public class UserPermissionViewModel
{
    public int BusinessId{ get; set; }
    public List<UserViewmodel> Users { get; set; }
}