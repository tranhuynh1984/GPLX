using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Entities
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitType { get; set; }

        public IList<PositionModel> PositionModels { get; set; }
    }
}
