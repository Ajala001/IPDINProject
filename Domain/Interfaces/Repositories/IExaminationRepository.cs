using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IExaminationRepository
    {
        Task<Examination> CreateAsync(Examination examination);
        Examination Update(Examination examination);
        void Delete(Examination examination);
        Task<IEnumerable<Examination>> GetExaminationsAsync();
        Task<IEnumerable<Examination>> SearchExaminationAsync(string courseTitle, string courseCode);
        Task<Examination> GetExamination(Expression<Func<Examination, bool>> predicate);
    }
}
