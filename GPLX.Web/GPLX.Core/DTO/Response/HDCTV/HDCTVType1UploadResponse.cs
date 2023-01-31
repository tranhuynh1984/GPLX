using System;
using System.Collections.Generic;
using GPLX.Core.Enum;

namespace GPLX.Core.DTO.Response.HDCTV
{

    public class ImportToDBResponse
    {
        public List<HdctvImportExcelBaseResponse> SuccessList { get; set; } = new List<HdctvImportExcelBaseResponse>();
        public List<ImportDBErrorObj> ErrorList { get; set; } = new List<ImportDBErrorObj>();

        public void AddSuccessItem(HdctvImportExcelBaseResponse item)
        {
            if (SuccessList == null) SuccessList = new List<HdctvImportExcelBaseResponse>();
            SuccessList.Add(item);
        }
        public void AddErrorItem(ImportDBErrorObj item)
        {
            if (ErrorList == null) ErrorList = new List<ImportDBErrorObj>();
            ErrorList.Add(item);
        }
    }
    
    public class ImportDBErrorObj
    {
        public string Key { get; set; }
        public string Reason { get; set; }
    }
    public class HdctvImportExcelResponse
    {
        public string ErrorFile { get; set; }
        public List<HdctvImportExcelBaseResponse> ImportData { get; set; }

        public void AddImportData(HdctvImportExcelBaseResponse data)
        {
            if (ImportData == null) ImportData = new List<HdctvImportExcelBaseResponse>();
            
            ImportData.Add(data);
        }
    }
    public class HdctvImportExcel<T> where T : HdctvImportExcelBaseResponse
    {
        public int SubId { get; set; }
        public string ErrorFile { get; set; }
        public List<T> ImportData { get; set; } = new List<T>();
        public int CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

        public void AddImportData(T data)
        {
            if (ImportData == null) ImportData = new List<T>();
            
            ImportData.Add(data);
        }
    }
    
    public class HdctvImportExcelBaseResponse
    {
        public int SubId { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; } = 1;
        public string IsActiveName { get; set; } = GlobalEnums.GetStatusName(1);
        /// <summary>
        /// Đường dẫn file excel
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public string RequestCode { get; set; }
    }
    
    public class HdctvType1UploadResponse : HdctvImportExcelBaseResponse
    {
        public string MaCP { get; set; }
        public string TenCP { get; set; }
        public float BP1 { get; set; }
        public float BP2 { get; set; }
        public float BP3 { get; set; }
        public float BP4 { get; set; }
        public float BP5 { get; set; }
        public float BP6 { get; set; }
        public float BP7 { get; set; }
        public float BP8 { get; set; }
        public float BP9 { get; set; }
        public float BP10 { get; set; }
        public float BP11 { get; set; }
    }
    
    public class HdctvType2UploadResponse : HdctvImportExcelBaseResponse
    {
        public string MaCP { get; set; }
        public string TenCP { get; set; }
        public float FixedPrice { get; set; }
    }
}
