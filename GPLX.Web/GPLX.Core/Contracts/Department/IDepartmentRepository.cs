using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.Department;
using GPLX.Core.DTO.Response.Department;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Department
{
    public interface IDepartmentRepository
    {
        Task<DepartmentSearchResponse> SearchAsync(int length, int size, DepartmentSearchRequest request);
        Task<DepartmentSearchResponseData> GetById(int id);
        Task<DepartmentCreateResponse> Create(DepartmentCreateRequest request);
        Task<DepartmentCreateResponse> ChangeStatus(DepartmentCreateRequest request);
        Task<IList<Departments>> GetAll(int status);
    }
}
