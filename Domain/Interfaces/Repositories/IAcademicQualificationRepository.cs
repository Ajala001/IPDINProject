using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IAcademicQualificationRepository
    {
        Task<AcademicQualification> CreateAsync(AcademicQualification qualification);
        AcademicQualification Update(AcademicQualification qualification);
        void Delete(AcademicQualification qualification);
        Task<IEnumerable<AcademicQualification>> GetQualificationsAsync();
        Task<AcademicQualification> GetQualificationAsync(Expression<Func<AcademicQualification, bool>> predicate);
    }
}
