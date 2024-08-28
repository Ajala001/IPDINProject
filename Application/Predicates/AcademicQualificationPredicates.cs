using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Application.Predicates
{
    public static class AcademicQualificationPredicates
    {
        public static Expression<Func<AcademicQualification, bool>> ById(Guid id)
        {
            return qualification => qualification.Id == id;
        }
    }
}
