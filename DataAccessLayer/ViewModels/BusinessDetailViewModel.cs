using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLayer.ViewModels;

public class BusinessDetailViewModel
{
    public BusinessItem BusinessItem { get; set; }

    public List<ReferenceDataValues> BusinessCategories { get; set; }

    public List<ReferenceDataValues> BusinessTypes { get; set; }

    public List<RoleViewModel> Roles { get; set; }
}