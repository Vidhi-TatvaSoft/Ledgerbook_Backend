using DataAccessLayer.Models;

namespace BusinessAcessLayer.Interface;

public interface IReferenceDataEntityService
{
    List<ReferenceDataValues> GetReferenceValues(string EntityType);
    String GetReferenceValueById(int referenceDataId);
}