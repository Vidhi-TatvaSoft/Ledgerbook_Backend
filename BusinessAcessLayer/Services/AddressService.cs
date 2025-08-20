using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Services;

public class AddressService : IAddressService
{
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepository;
    public AddressService(LedgerBookDbContext context, IGenericRepo genericRepo)
    {
        _context = context;
        _genericRepository = genericRepo;
    }

    public async Task<int> SaveAddress(AddressViewModel addressVM, int userId)
    {
        Address address = new();
        address.AddressLine1 = addressVM.AddressLine1;
        address.AddressLine2 = addressVM.AddressLine2;
        address.AddressType = (byte)EnumHelper.SourceType.Business;
        address.City = addressVM.City ;
        address.Pincode = addressVM.Pincode;
        address.CreatedAt = DateTime.UtcNow;
        address.CreatedById = userId;
        await _genericRepository.AddAsync(address);
        return address.Id;

    }

    public AddressViewModel GetAddressById(int addressId)
    {
        return _genericRepository.GetAll<Address>(a => a.Id == addressId && a.DeletedAt == null).Select(a => new AddressViewModel
        {
            AddressId = a.Id,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Pincode = a.Pincode
        }).ToList().FirstOrDefault()!;
    }
}