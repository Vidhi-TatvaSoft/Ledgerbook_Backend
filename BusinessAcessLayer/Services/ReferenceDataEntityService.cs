using System.Linq.Expressions;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;

namespace BusinessAcessLayer.Services;

public class ReferenceDataEntityService : IReferenceDataEntityService
{
    private readonly IGenericRepo _genericRepository;

    public ReferenceDataEntityService(
        IGenericRepo genericRepository
    )
    {
        _genericRepository = genericRepository;
    }

    public List<ReferenceDataValues> GetReferenceValues(string EntityType)
    {
        return _genericRepository.GetAll<ReferenceDataValues>(
            predicate: x => x.EntityType.EntityType == EntityType && x.DeletedAt == null,
            includes: new List<Expression<Func<ReferenceDataValues, object>>>
            {
                x => x.EntityType
            }
        ).ToList();

    }

    public String GetReferenceValueById(int referenceDataId)
    {
        return _genericRepository.Get<ReferenceDataValues>(x => x.Id == referenceDataId && x.DeletedAt == null).EntityValue;
    }
}