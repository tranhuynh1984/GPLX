using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.Relationship
{
    public class RelationshipSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public string RelationshipCode { get; set; }
        public string RelationshipName { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public int Stt { get; set; }
        public int MaxStt { get; set; }
    }

    public class RelationshipSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<RelationshipSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
