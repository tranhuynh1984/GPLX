
namespace GPLX.Core.DTO.Request.Users
{
    public class UserSignatureConfigRequest
    {
        public int UserId { get; set; }
        public string SignatureAcc  { get; set; }
        public string SignaturePass  { get; set; }
        public string SignatureConfirmPass  { get; set; }
    }
}
