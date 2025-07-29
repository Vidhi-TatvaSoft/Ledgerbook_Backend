using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IAddressService
{
    Task<int> SaveAddress(AddressViewModel addressVM, int userId);
    AddressViewModel GetAddressById(int addressId);
}