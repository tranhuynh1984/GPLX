using GPLX.Core.DTO.Request.Signature;

namespace GPLX.Core.Contracts.Signature
{
    public interface ISignatureConnect
    {
        string GetAccess(GetTokenBody body, string endpoint);

        string GetGroupId(string groupAdminEmail, string accessToken, string endpoint);

        CertResponse GetCertId(string accessToken, string endpoint);

        string ParseCert(string certBase64);

        string SignPdf(SignOpts ops, string endpoint);
        string SignHash(string certId, string fileName, string dataBase64, string accessToken, string endpoint);
    }
}
