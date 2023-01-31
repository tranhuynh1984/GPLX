using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPLX.Web.Models
{
    public class ContractUploadFileData
    {
        public string FileName { get; set; }
        public string ViewPath { get; set; }
        public float Size { get; set; }

    }

    public class ContractUploadResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ContractUploadFileData> Data { get; set; }
        public string FileUploadExtension { get; set; }

        public float FileUploadSize { get; set; }

        public void Add(string file, string hostView, float size)
        {
            if (Data == null)
                Data = new List<ContractUploadFileData>();
            Data.Add(new ContractUploadFileData
            {
                FileName = file,
                ViewPath = hostView,
                Size = size
            });
        }
    }
}
