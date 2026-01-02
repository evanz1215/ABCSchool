using Domain.Entities;

namespace Applocation.Features.Schools;

public interface ISchoolService
{
    Task<int> CreateAsync(School school);

    Task<int> UpdateAsync(School school);

    Task<int> DeleteAsync(School school);

    Task<School> GetByIdAsync(int schoolId);

    Task<List<School>> GetAllAsync();

    Task<School> GetByNameAsync(string name);
}