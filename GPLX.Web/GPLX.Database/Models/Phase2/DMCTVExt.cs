using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMCTVExt : UpdateTime
    {
        public string MaBS { get; set; }

        public Guid ExtId { get; set; }
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
