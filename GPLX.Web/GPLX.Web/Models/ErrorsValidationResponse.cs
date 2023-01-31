using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPLX.Web.Models
{
    public class ErrorsValidationResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public List<string> Errors { get; set; }
    }
}
