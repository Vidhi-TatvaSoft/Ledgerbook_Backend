using System.Linq.Expressions;
using System.Threading.Tasks;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessAcessLayer.Services;

public class ReferenceDataEntityService : IReferenceDataEntityService
{
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepository;

    public ReferenceDataEntityService(
        LedgerBookDbContext context,
        IGenericRepo genericRepository
    )
    {
        _context = context;
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