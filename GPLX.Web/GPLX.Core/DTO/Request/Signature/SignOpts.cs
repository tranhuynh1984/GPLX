using System.IO;

namespace GPLX.Core.DTO.Request.Signature
{
    public class SignOpts
    {
        #region Các thông tin của cổng ký số
        public string EnterpriseAcc { get; set; }
        public string EnterpriseUser { get; set; }
        public string EnterprisePass { get; set; }

        public string EndpointToken { get; set; }
        public string EndpointQuery { get; set; }

        public string ClientId { get; set; }
        public string ClientSec { get; set; }
        public string Cert { get; set; }
        public string CertBase64 { get; set; }
        public string GroupId { get; set; }
        public string AccessToken { get; set; }

        #endregion

        public byte[] PdfContents
        {
            get
            {
                if (string.IsNullOrEmpty(RelativePath))
                    return null;
                if (File.Exists(FullPath))
                    return File.ReadAllBytes(FullPath);
                return null;
            }
        }

        public byte[] Backgrounds => File.ReadAllBytes("./Resources/logo/signature.jpg");

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(RelativePath))
                    return null;
                if (File.Exists(FullPath))
                    return new FileInfo(FullPath).Name;
                return null;
            }
        }
       
        /// <summary>
        /// Tên người ký
        /// </summary>
        public string Signer { get; set; }

        public string PhysicalPath { get; set; }
        /// <summary>
        /// Đường dẫn tương đối của file
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// Ký tự để tìm vị trí ký
        /// Thông thường là tên của chức vụ
        /// </summary>
        public string TextFilter { get; set; }
        /// <summary>
        /// Build đường dẫn đến file
        /// </summary>
        public string FullPath => $"{PhysicalPath}{RelativePath}";
        /// <summary>
        /// Đường dẫn file sau khi ký
        /// </summary>
        public string SignaturePath => FullPath.Replace(".pdf", "_signed.pdf");
        /// <summary>
        /// Kế toán trưởng MG
        /// </summary>
        public bool IsChiefMg { get; set; }

    }
    public class HashEntity
    {
        public string Data { get; set; }
    }
    public class HashSigned
    {
        public string Data { get; set; }
        public string Signature { get; set; }
        public int Code { get; set; }
    }
}
