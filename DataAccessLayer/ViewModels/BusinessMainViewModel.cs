using DataAccessLayer.Models;

namespace DataAccessLayer.ViewModels;

public class BusinessMainViewModel
{

    public BusinessMainViewModel()
    {
        BusinessDetailViewModel = new BusinessDetailViewModel();
        UserPermissionViewModel = new UserPermissionViewModel();
    }
    public List<BusinessViewModel> Businesses { get; set; }
    public BusinessDetailViewModel BusinessDetailViewModel { get; set; }
    public UserPermissionViewModel UserPermissionViewModel { get; set; }

    public string BusinessViewModelString { get; set; }

    public string MappingRoles{ get; set; }


}   