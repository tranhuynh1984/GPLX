using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Aspose.Cells;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response.Signature;
using GPLX.Infrastructure.Extensions;
using Newtonsoft.Json;
using Org.BouncyCastle.Operators;
using Serilog;
using SignService.Common.HashSignature.Interface;
using SignService.Common.HashSignature.Pdf;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Path = System.IO.Path;
using Rectangle = iTextSharp.text.Rectangle;

namespace GPLX.Web.Signature
{
    public class SignatureConnect
    {
        /// <summary>
        /// Lấy token để ký số
        /// </summary>
        /// <param name="body"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public string GetAccess(GetTokenBody body, string endpoint)
        {
            try
            {
                using var wc = new WebClient
                {
                    Headers = { [HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded" }
                };
                string response = wc.UploadString(endpoint, "POST", $"username={body.username}&password={body.password}&client_id={body.client_id}&client_secret={body.client_secret}&grant_type=password");
                var content = JsonConvert.DeserializeObject<GetTokenResponse>(response);
                return content.access_token;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy GroupId theo admin email
        /// </summary>
        /// <param name="groupAdminEmail"></param>
        /// <param name="accessToken"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public string GetGroupId(string groupAdminEmail, string accessToken, string endpoint)
        {
            var req = new RequestMessage
            {
                RequestID = Guid.NewGuid().ToString(),
                ServiceID = "UserAccount",
                FunctionName = "GetProfile"
            };
            var body = JsonConvert.SerializeObject(req);

            using WebClient wc = new WebClient();
            try
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                string response = wc.UploadString(endpoint, "POST", body);

                var content = JsonConvert.DeserializeObject<ResponseMessage>(response);
                if (content != null)
                {
                    var str = JsonConvert.SerializeObject(content.Content);
                    Account acc = JsonConvert.DeserializeObject<Account>(str);
                    if (acc?.Groups != null)
                    {
                        foreach (var group in acc.Groups)
                        {
                            if (groupAdminEmail.Equals(group.AdminEmail))
                            {
                                return group.ID;
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", e.Message);

                return string.Empty;
            }
        }
        /// <summary>
        /// Load cert
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public CertResponse GetCertId(string accessToken, string endpoint)
        {

            var req = new RequestMessage
            {
                RequestID = Guid.NewGuid().ToString(),
                ServiceID = "Certificate",
                FunctionName = "GetAccountCertificateByEmail",
                Parameter = new CertParameter
                {
                    PageIndex = 0,
                    PageSize = 10
                }
            };
            var body = JsonConvert.SerializeObject(req);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                    string response = wc.UploadString(endpoint, "POST", body);

                    var content = JsonConvert.DeserializeObject<ResponseMessage>(response);
                    if (content != null)
                    {
                        var str = JsonConvert.SerializeObject(content.Content);
                        CertResponse acc = JsonConvert.DeserializeObject<CertResponse>(str);
                        if (acc != null && acc.Count > 0)
                        {
                            return acc;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "{0}", ex.Message);

                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certBase64"></param>
        /// <returns></returns>
        public string ParseCert(string certBase64)
        {
            if (string.IsNullOrEmpty(certBase64))
            {
                return certBase64;
            }
            certBase64 = certBase64.Replace("-----BEGIN CERTIFICATE-----\n", "");
            certBase64 = certBase64.Replace("\n-----END CERTIFICATE-----", "");
            certBase64 = certBase64.Replace("\r\n", "");
            return certBase64;
        }

        /// <summary>
        /// Ký file PDF
        /// </summary>
        /// <param name="ops"></param>
        /// <returns></returns>
        public SignatureResponse SignPdf(SignOpts ops)
        {
            try
            {
                var rt = new SignatureResponse();
                int _marginTop = 5;
                float height = 200f;
                float width = 200;

                float x = 0;
                float y = 0;
                int pageSigner = 0;
                float fontSize = 16;
                bool matchPositionSigner = false;


                #region Hardcode document

                // hard-code: trường hợp kế toán trưởng có 2 loại
                // KTT đơn vị
                // KTT MG
                // nhưng trong excel mẫu thì quy chung là KTT
                // suy ra không xác định được vị trí ký
                // tạm thời hard-code case này, tìm giải pháp bổ sung sau

                #endregion

                if (ops.TextFilter.StartsWith("KTT", StringComparison.OrdinalIgnoreCase))
                    ops.TextFilter = "Kế toán trưởng";
                if (ops.IsChiefMg && ops.TextFilter.Contains("MG", StringComparison.CurrentCultureIgnoreCase))
                    ops.TextFilter = $"{ops.TextFilter} MG";

                using (var reader = new PdfReader(ops.PdfContents))
                {
                    for (int pageNum = 1; pageNum <= reader.NumberOfPages; ++pageNum)
                    {
                        var myLocate = new MyLocationTextExtractionStrategy(ops.TextFilter, CompareOptions.IgnoreCase);
                        PdfTextExtractor.GetTextFromPage(reader, 1, myLocate);

                        var textStrategy = new TextWithFontExtractionStategy();
                        string font = PdfTextExtractor.GetTextFromPage(reader, 1, textStrategy);
                        var filterSeparators = ops.TextFilter.Split(" ");

                        var checks = new List<StartEndMatcher>();
                        bool matched = false;

                        string sFirstWord = filterSeparators[0];
                        if (filterSeparators.Length > 1)
                        {
                            foreach (var t in myLocate.matchLocators)
                            {
                                int start = 0, end = 0;
                                if (sFirstWord.Equals(t.Word, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    start = t.Counter;
                                    bool found = true;
                                    for (int j = 0; j < filterSeparators.Length; j++)
                                    {
                                        var fNext = myLocate.matchLocators.FirstOrDefault(c => c.Counter == t.Counter + j);
                                        if (fNext == null || !fNext.Word.Equals(filterSeparators[j],
                                            StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            found = false;
                                            break;
                                        }

                                        end = fNext.Counter;
                                    }

                                    if (found)
                                    {
                                        var matcher = myLocate.matchLocators.Where(xc => xc.Counter >= start && xc.Counter <= end)
                                            .OrderBy(xc => xc.Counter).ToList();
                                        var endRc = myLocate.matchLocators.FirstOrDefault(c => c.Counter == end);
                                        foreach (var m in matcher)
                                        {
                                            checks.Add(new StartEndMatcher
                                            {
                                                word = m.Word,
                                                start = myLocate.matchLocators.IndexOf(m),
                                                end = myLocate.matchLocators.IndexOf(endRc),
                                            });
                                        }
                                    }

                                }
                            }

                            if (checks.Count == 0)
                            {
                                rt.IsError = true;
                                rt.Message = "Không tìm thấy vị trí ký, vui lòng sử dụng mẫu trên phần mềm!";
                                // không tìm thấy vị trí ký
                                return rt;
                            }
                        }
                        else
                        {
                            matched = true;
                            x = myLocate.matchLocators[0].llx;
                            y = myLocate.matchLocators[0].lly;
                        }

                        bool isGeneralSigner = sFirstWord.Equals("Tổng", StringComparison.CurrentCultureIgnoreCase);
                        foreach (var startEndMatcher in checks)
                        {
                            // nếu từ đầu tiên trong cụm từ tìm kiếm được ở PDF
                            // trùng với ký tự đầu tiên trong từ khóa

                            if (!startEndMatcher.word.Equals(sFirstWord, StringComparison.CurrentCultureIgnoreCase))
                                continue;

                            if (startEndMatcher.start == 0 && myLocate.matchLocators[startEndMatcher.start].Word
                                .Equals(filterSeparators[0], StringComparison.OrdinalIgnoreCase))
                            {
                                //previous
                                int counter = myLocate.matchLocators[startEndMatcher.start].Counter;
                                var oPrevious = myLocate.allLocators.FirstOrDefault(x => x.Counter == counter - 1);
                                if (filterSeparators.Any(xc => xc.Equals(oPrevious?.Word, StringComparison.CurrentCultureIgnoreCase)) || checks.Count == filterSeparators.Length)
                                    matched = true;
                            }

                            if (!matched && startEndMatcher.start > 0)
                            {
                                int counter = myLocate.matchLocators[startEndMatcher.start].Counter;
                                var oPrevious = myLocate.allLocators.FirstOrDefault(x => x.Counter == counter - 1);

                                if (isGeneralSigner && oPrevious.Word.Equals("PHÓ", StringComparison.CurrentCultureIgnoreCase))
                                    continue;
                                if (filterSeparators.Any(cx => cx.Equals(oPrevious?.Word, StringComparison.CurrentCultureIgnoreCase)) || checks.Count == filterSeparators.Length)
                                    matched = true;
                            }

                            if (matched)
                            {
                                var oStart = myLocate.matchLocators[startEndMatcher.start];
                                var oEnd = myLocate.matchLocators[startEndMatcher.end];


                                x = (oStart.llx + oEnd.urx) / 2;
                                y = myLocate.matchLocators[startEndMatcher.start].lly;
                                matchPositionSigner = true;
                                break;
                            }
                        }

                        if (!matched)
                            matchPositionSigner = false;

                        var line = font.Split("<br />").FirstOrDefault(cx => cx.Contains(ops.TextFilter, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
                        var rgxFont = new Regex("(font-size:[0-9.]+)");
                        var rgxSizeOnly = new Regex("[0-9.]+");
                        string fMatch = rgxFont.Match(line).Value;
                        float defaultFontSize = 14;
                        if (!string.IsNullOrEmpty(fMatch))
                            defaultFontSize = rgxSizeOnly.Match(fMatch).Value.ToFloat() / 0.75f;
                        var percent = defaultFontSize / 14;
                        fontSize = defaultFontSize;

                        height = height * percent;

                        var imgSignature = Image.GetInstance(ops.Backgrounds);
                        width = imgSignature.Width;
                        if (imgSignature.Height > height)
                            width = width * (height / imgSignature.Height);
                        pageSigner = pageNum;
                    }
                    reader.Close();
                }

                if (!matchPositionSigner)
                {
                    rt.IsError = true;
                    rt.Message = "Không tìm thấy vị trí ký số!";
                    return rt;
                }

                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA1SignatureDescription),
                   "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                );
                IHashSigner signer = HashSignerFactory.GenerateSigner(ops.PdfContents, ops.CertBase64, null, HashSignerFactory.PDF);
                signer.SetHashAlgorithm(SignService.Common.HashSignature.Common.MessageDigestAlgorithm.SHA1);

                #region Optional -----------------------------------
                // Property: Lý do ký số
                ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                // Hình ảnh hiển thị trên chữ ký (mặc định là logo VNPT)
                var imgBytes = ops.Backgrounds;
                if (imgBytes.Length > 0)
                    ((PdfHashSigner)signer).SetCustomImage(imgBytes);

                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                // Nội dung text trên chữ ký (OPTIONAL)
                //((PdfHashSigner)signer).SetLayer2Text($"Duyệt kế hoạch bởi: \n {ops.Signer}\nNgày ký: {DateTime.Now:dd/MM/yyyy HH:mm:ss} \nTổ chức xác thực: VNPT \nCertication Authority");
                ((PdfHashSigner)signer).SetFontSize((int)(fontSize + 1));
                ((PdfHashSigner)signer).SetFontColor("0000ff");
                // Kiểu chữ trên chữ ký
                ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                // Font chữ trên chữ ký
                ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);

                //Hiển thị chữ ký và vị trí bên dưới dòng _textFilter cách 1 đoạn _marginTop, độ rộng _width, độ cao _height

                var sigRectangle = new PdfSignatureView
                {
                    Rectangle =
                        $"{(int)(x - width / 2f)},{(int)(y - _marginTop - height)},{(int)(x - width / 2f + width)},{(int)(y - _marginTop)}",
                    Page = pageSigner
                };
                ((PdfHashSigner)signer).AddSignatureView(sigRectangle);


                // Signature widget border type (OPTIONAL)
                ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.NONE);
                #endregion -----------------------------------------

                var hashValue = signer.GetSecondHashAsBase64();

                //signHash
                var byteHash = SignHash(ops.Cert, ops.FileName, hashValue, ops.AccessToken, ops.EndpointQuery);


                if (string.IsNullOrEmpty(byteHash))
                {
                    Log.Error("Sign error");
                    return null;
                }

                if (!signer.CheckHashSignature(byteHash))
                {
                    Log.Error("Signature not match");
                    return null;
                }

                // ------------------------------------------------------------------------------------------

                // 3. Package external signature to signed file
                byte[] signed = signer.Sign(byteHash);
                File.WriteAllBytes(ops.SignaturePath, signed);
                rt.IsError = false;
                rt.SignaturePath = ops.SignaturePath;
                return rt;
            }
            catch (Exception e)
            {
                Log.Error(e, "exception {0}", e.Message);
                return new SignatureResponse
                {
                    IsError = true,
                    Message = "Lỗi ký số, vui lòng liên hệ quản trị viên!"
                };
            }
        }

        public SignatureResponse Signature(SignOpts signOp)
        {
            try
            {
                if (string.IsNullOrEmpty(signOp.EnterpriseUser) || string.IsNullOrEmpty(signOp.EnterprisePass))
                    return new SignatureResponse
                    {
                        IsError = true,
                        Message = "Tài khoản chưa cấu hình thông tin chữ ký số!"
                    };

                string access = GetAccess(new GetTokenBody
                {
                    username = signOp.EnterpriseUser,
                    client_id = signOp.ClientId,
                    client_secret = signOp.ClientSec,
                    grant_type = "password",
                    password = signOp.EnterprisePass
                }, signOp.EndpointToken);

                if (string.IsNullOrEmpty(access))
                    return new SignatureResponse
                    {
                        IsError = true,
                        Message = "Lỗi ký số, vui lòng liên hệ quản trị viên!"
                    };

                var certs = GetCertId(access, signOp.EndpointQuery);
                if (certs == null)
                    return new SignatureResponse
                    {
                        IsError = true,
                        Message = "Lỗi ký số, vui lòng liên hệ quản trị viên!"
                    };
                var cert = certs.Items[0].ID;
                var certBase64 = ParseCert(certs.Items[0].CertBase64);
                var groupId = GetGroupId(signOp.EnterpriseAcc, access, signOp.EndpointQuery);
                if (string.IsNullOrEmpty(groupId))
                    return new SignatureResponse
                    {
                        IsError = true,
                        Message = "Lỗi ký số, vui lòng liên hệ quản trị viên!"
                    };

                signOp.GroupId = groupId;
                signOp.CertBase64 = certBase64;
                signOp.Cert = cert;
                signOp.AccessToken = access;

                return SignPdf(signOp);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", signOp);
                return new SignatureResponse
                {
                    IsError = true,
                    Message = "Lỗi ký số, vui lòng liên hệ quản trị viên!"
                };
            }
        }

        public string SignHash(string certId, string fileName, string dataBase64, string accessToken, string endpoint)
        {
            var req = new RequestMessage
            {
                RequestID = Guid.NewGuid().ToString(),
                ServiceID = "SignServer",
                FunctionName = "SignHash",
                Parameter = new SignParameter
                {
                    CertID = certId,
                    FileName = fileName,
                    DataBase64 = dataBase64,
                }
            };

            var body = JsonConvert.SerializeObject(req);
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                    string response = wc.UploadString(endpoint, "POST", body);

                    var content = JsonConvert.DeserializeObject<ResponseMessage>(response);
                    if (content != null)
                    {
                        var str = JsonConvert.SerializeObject(content.Content);
                        SignResponse acc = JsonConvert.DeserializeObject<SignResponse>(str);
                        if (acc != null)
                        {
                            return acc.SignedData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "{0}", ex.Message);
                    return null;
                }

            }
            return null;
        }

        /// <summary>
        /// Ký điện tử
        /// </summary>
        /// <param name="file"></param>
        /// <param name="signatureImages"></param>
        /// <param name="signer"></param>
        /// <param name="textFilter"></param>
        /// <param name="creatorSignature">Người lập</param>
        /// <param name="isChiefMg">trường hợp đặc biệt - kế toán trưởng MG</param>
        /// <returns></returns>
        public SignatureResponse SingNormal(string file, byte[] signatureImages, string signer, string textFilter, bool creatorSignature = false, bool isChiefMg = false)
        {
            try
            {
                string sBeforeDateSignature = "Ngày ký";

                #region Hardcode document

                // hard-code: trường hợp kế toán trưởng có 2 loại
                // KTT đơn vị
                // KTT MG
                // nhưng trong excel mẫu thì quy chung là KTT
                // suy ra không xác định được vị trí ký
                // tạm thời hard-code case này, tìm giải pháp bổ sung sau

                #endregion

                if (creatorSignature)
                {
                    textFilter = "Người lập";
                    sBeforeDateSignature = "Ngày lập";
                }
                signer = $"{signer}\r\n{sBeforeDateSignature} : {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                if (textFilter.StartsWith("KTT", StringComparison.OrdinalIgnoreCase))
                    textFilter = "Kế toán trưởng";
                if (isChiefMg && textFilter.Contains("MG", StringComparison.CurrentCultureIgnoreCase))
                    textFilter = $"{textFilter} MG";

                var rt = new SignatureResponse();
                string outputFile = Path.GetRandomFileName();
                var ff = new FileInfo(file);
                outputFile = $"{ff.DirectoryName}\\{outputFile}.pdf";
                int _marginTop = 5;
                float height = 120f;
                using var reader = new PdfReader(file);
                for (int pageNum = 1; pageNum <= reader.NumberOfPages; ++pageNum)
                {
                    var myLocate = new MyLocationTextExtractionStrategy(textFilter, CompareOptions.OrdinalIgnoreCase);
                    PdfTextExtractor.GetTextFromPage(reader, 1, myLocate);

                    var textStrategy = new TextWithFontExtractionStategy();
                    string font = PdfTextExtractor.GetTextFromPage(reader, 1, textStrategy);
                    var filterSeparators = textFilter.Split(" ");

                    var checks = new List<StartEndMatcher>();
                    float pointX = 0;
                    float pointY = 0;
                    bool matched = false;


                    string sFirstWord = filterSeparators[0];
                    if (filterSeparators.Length > 1)
                    {
                        foreach (var t in myLocate.matchLocators)
                        {
                            int start = 0, end = 0;
                            if (sFirstWord.Equals(t.Word, StringComparison.CurrentCultureIgnoreCase))
                            {
                                start = t.Counter;
                                bool found = true;
                                for (int j = 0; j < filterSeparators.Length; j++)
                                {
                                    var fNext = myLocate.matchLocators.FirstOrDefault(c => c.Counter == t.Counter + j);
                                    if (fNext == null || !fNext.Word.Equals(filterSeparators[j],
                                        StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        found = false;
                                        break;
                                    }

                                    end = fNext.Counter;
                                }

                                if (found)
                                {
                                    var matcher = myLocate.matchLocators.Where(x => x.Counter >= start && x.Counter <= end)
                                        .OrderBy(x => x.Counter).ToList();
                                    var endRc = myLocate.matchLocators.FirstOrDefault(c => c.Counter == end);
                                    foreach (var m in matcher)
                                    {
                                        checks.Add(new StartEndMatcher
                                        {
                                            word = m.Word,
                                            start = myLocate.matchLocators.IndexOf(m),
                                            end = myLocate.matchLocators.IndexOf(endRc),
                                        });
                                    }
                                }

                            }
                        }

                        //for (int i = 0; i < myLocate.matchLocators.Count; i++)
                        //{
                        //    foreach (var f in filterSeparators)
                        //    {
                        //        if (f.Equals(myLocate.matchLocators[i].Word, StringComparison.CurrentCultureIgnoreCase))
                        //        {
                        //            string backWord = i > 0 ? myLocate.matchLocators[i - 1].Word : f;
                        //            var oCheck = checks.FirstOrDefault(x =>
                        //                x.word.Equals(backWord, StringComparison.CurrentCultureIgnoreCase) && !x.founded);
                        //            if (oCheck != null)
                        //            {
                        //                var iOf = checks.IndexOf(oCheck);
                        //                if ((i == oCheck.start + 1 || i == oCheck.end + 1) && !oCheck.word.Equals(f, StringComparison.CurrentCultureIgnoreCase))
                        //                {
                        //                    oCheck.end = i;
                        //                    if (oCheck.end - oCheck.start + 1 == filterSeparators.Length)
                        //                        oCheck.founded = true;
                        //                    checks[iOf] = oCheck;
                        //                }
                        //                else
                        //                    checks.Remove(oCheck);
                        //            }
                        //            else
                        //            {
                        //                checks.Add(new StartEndMatcher
                        //                {
                        //                    word = f,
                        //                    start = i
                        //                });
                        //            }
                        //        }
                        //    }
                        //}

                        if (checks.Count == 0)
                        {
                            rt.IsError = true;
                            rt.Message = "Không tìm thấy vị trí ký, vui lòng sử dụng mẫu trên phần mềm!";
                            // không tìm thấy vị trí ký
                            return rt;
                        }
                    }
                    else
                    {
                        matched = true;
                        pointX = myLocate.matchLocators[0].llx;
                        pointY = myLocate.matchLocators[0].lly;
                    }

                    bool isGeneralSigner = sFirstWord.Equals("Tổng",StringComparison.CurrentCultureIgnoreCase);
                    //todo: 
                    foreach (var startEndMatcher in checks)
                    {
                        // nếu từ đầu tiên trong cụm từ tìm kiếm được ở PDF
                        // trùng với ký tự đầu tiên trong từ khóa
                        if (!startEndMatcher.word.Equals(sFirstWord, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        if (startEndMatcher.start == 0 && myLocate.matchLocators[startEndMatcher.start].Word
                            .Equals(filterSeparators[0], StringComparison.OrdinalIgnoreCase))
                        {
                            //previous
                            int counter = myLocate.matchLocators[startEndMatcher.start].Counter;
                            var oPrevious = myLocate.allLocators.FirstOrDefault(x => x.Counter == counter - 1);
                            if (isGeneralSigner && oPrevious.Word.Equals("PHÓ", StringComparison.CurrentCultureIgnoreCase))
                                continue;
                            if (filterSeparators.Any(x => x.Equals(oPrevious?.Word, StringComparison.CurrentCultureIgnoreCase)) || checks.Count == filterSeparators.Length)
                                matched = true;
                        }

                        if (!matched && startEndMatcher.start > 0)
                        {
                            int counter = myLocate.matchLocators[startEndMatcher.start].Counter;
                            var oPrevious = myLocate.allLocators.FirstOrDefault(x => x.Counter == counter - 1);
                            bool previous = filterSeparators.Any(x =>
                                x.Equals(oPrevious?.Word, StringComparison.CurrentCultureIgnoreCase));

                            if(isGeneralSigner && oPrevious.Word.Equals("PHÓ",StringComparison.CurrentCultureIgnoreCase))
                                continue;

                            if (previous || checks.Count == filterSeparators.Length || isGeneralSigner)
                                matched = true;
                        }

                        if (matched)
                        {
                            var oStart = myLocate.matchLocators[startEndMatcher.start];
                            var oEnd = myLocate.matchLocators[startEndMatcher.end];


                            pointX = (oStart.llx + oEnd.urx) / 2;
                            pointY = myLocate.matchLocators[startEndMatcher.start].lly;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        rt.IsError = true;
                        rt.Message = "Không tìm thấy vị trí ký, vui lòng sử dụng biểu mẫu trên phần mềm!";
                        // không tìm thấy vị trí ký
                        return rt;
                    }

                    #region Calculate percent responsive

                    var line = font.Split("<br />").FirstOrDefault(cx => cx.Contains(textFilter, StringComparison.CurrentCultureIgnoreCase)) ?? string.Empty;
                    var rgxFont = new Regex("(font-size:[0-9.]+)");
                    var rgxSizeOnly = new Regex("[0-9.]+");
                    string fMatch = rgxFont.Match(line).Value;
                    float defaultFontSize = 16;
                    if (!string.IsNullOrEmpty(fMatch))
                        defaultFontSize = rgxSizeOnly.Match(fMatch).Value.ToFloat() / 0.75f;

                    if (defaultFontSize > 16)
                        defaultFontSize = 16;
                    float percent = defaultFontSize / 16;

                    #endregion


                    #region set image signature
                    height = height * percent;



                    byte[] buffers;
                    using (var ms = new MemoryStream())
                    {
                        var stamper = new PdfStamper(reader, ms);
                        string timeNewsRoman = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                        BaseFont bf = BaseFont.CreateFont(timeNewsRoman, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                        float signSize = defaultFontSize + 1;
                        Font f = new Font(bf, signSize, Font.NORMAL)
                        {
                            Color = new BaseColor(Color.FromArgb(11, 107, 181))
                        };

                        float singerWidth = f.GetCalculatedBaseFont(true).GetWidthPoint(signer, signSize);
                        var width = singerWidth;


                        var rectangleImgSignature = new Rectangle(pointX - width / 2, pointY - _marginTop - height,
                            pointX - width / 2 + width, pointY - _marginTop);

                        var pdfContent = stamper.GetOverContent(pageNum);

                        #region Debug xác định tọa độ khung

                        //var centerReg = new Rectangle(pointX, pointY)
                        //{
                        //    BorderWidth = 1,
                        //    BorderColor = BaseColor.YELLOW,
                        //    Border = iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER
                        //};

                        //var leftRag = new Rectangle(oStart.llx, pointY)
                        //{
                        //    BorderWidth = 1,
                        //    BorderColor = BaseColor.BLUE,
                        //    Border = iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER
                        //};

                        //var right = new Rectangle(oEnd.urx, pointY)
                        //{
                        //    BorderWidth = 1,
                        //    BorderColor = BaseColor.RED,
                        //    Border = iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER
                        //};

                        //pdfContent.Rectangle(centerReg);
                        //pdfContent.Rectangle(leftRag);
                        //pdfContent.Rectangle(right);

                        #endregion

                        float originWidth = 0;
                        if (!creatorSignature)
                        {
                            var imgSignature = Image.GetInstance(signatureImages);
                            originWidth = imgSignature.Width;
                            if (imgSignature.Height > height)
                            {
                                originWidth = originWidth * (height / imgSignature.Height);
                                width = originWidth;
                            }

                            imgSignature.Alignment = Element.ALIGN_CENTER;
                            rectangleImgSignature = new Rectangle(pointX - width / 2f, pointY - _marginTop - height, pointX - width / 2f + width, pointY - _marginTop);
                            imgSignature.ScaleAbsolute(rectangleImgSignature);
                            imgSignature.SetAbsolutePosition(pointX - width / 2f, pointY - _marginTop - height);
                            pdfContent.AddImage(imgSignature);
                        }


                        var sigRectangle = rectangleImgSignature;
                        // người lập thì không có ảnh chữ ký
                        if (!creatorSignature)
                        {
                            if (originWidth < singerWidth)
                            {
                                float plus = (singerWidth - originWidth) / 2;
                                sigRectangle.Left = sigRectangle.Left - plus;
                                sigRectangle.Right = sigRectangle.Right + plus;
                            }
                            sigRectangle.Top = sigRectangle.Top - (percent < 1 ? 25 + 25 * percent : 25);
                            sigRectangle.Bottom = sigRectangle.Bottom - (percent < 1 ? 25 + 25 * percent : 25);
                        }

                        //sigRectangle.BorderWidth = 1;
                        //sigRectangle.BorderColor = BaseColor.RED;
                        //sigRectangle.Border = iTextSharp.text.Rectangle.LEFT_BORDER |
                        //                      iTextSharp.text.Rectangle.RIGHT_BORDER |
                        //                      iTextSharp.text.Rectangle.BOTTOM_BORDER;

                        ColumnText cto = new ColumnText(pdfContent);
                        Phrase myText = new Phrase(signer, f);
                        cto.SetSimpleColumn(myText, sigRectangle.Left, sigRectangle.Top, sigRectangle.Right, sigRectangle.Bottom - 200, defaultFontSize + 2, Element.ALIGN_CENTER);

                        cto.Go();
                        //var ct = new ColumnText(pdfContent)
                        //{
                        //    Alignment = Element.ALIGN_CENTER,
                        //};
                        //ct.SetSimpleColumn(sigRectangle);
                        //sigRectangle.Bottom = sigRectangle.Bottom - 200;
                        //ct.AddText(new Phrase(signer, f));
                        //ct.AddText(new Chunk(signer, f));
                        //ct.SetText(new Phrase(signer, f));
                        //ct.Go();

                        //sigRectangle.BorderWidth = 1;
                        //sigRectangle.BorderColor = BaseColor.GREEN;
                        //sigRectangle.Border = iTextSharp.text.Rectangle.LEFT_BORDER |
                        //                      iTextSharp.text.Rectangle.RIGHT_BORDER |
                        //                      iTextSharp.text.Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;

                        //pdfContent.Rectangle(sigRectangle);
                        pdfContent.Stroke();

                        stamper.Dispose();
                        buffers = ms.ToArray();
                    }
                    using var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    fs.Write(buffers, 0, buffers.Length);

                    #endregion
                }
                reader.Close();

                rt.IsError = false;
                rt.SignaturePath = file;

                Guid guidRad = Guid.NewGuid();
                try
                {
                    string newFileLocate = file.Replace(".pdf", $"{guidRad}.pdf");
                    File.Move(file, newFileLocate);

                    File.Move(outputFile, file);
                    rt.OriginalPathOlder = newFileLocate;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error");
                    // Rollback when old file has move but contains exception
                    if (!File.Exists(file))
                    {
                        string newFileLocate = file.Replace(".pdf", $"{guidRad}.pdf");
                        File.Move(newFileLocate, file);
                    }
                }
                return rt;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", file);
                return new SignatureResponse { IsError = true, Message = "Lỗi ký điện tử, vui lòng liên hệ quản trị viên!" };
            }
        }

        public string CreatePdf(string excel, string pdfName, string physicalFolder, string signer)
        {
            try
            {
                if (string.IsNullOrEmpty(pdfName))
                    return string.Empty;

                string fullFile = excel;
                if (!excel.StartsWith(physicalFolder))
                    fullFile = $"{physicalFolder}/{excel}";

                var wb = new Workbook(fullFile);
                int sheetCounter = 0;
                foreach (Worksheet sheet in wb.Worksheets)
                {
                    if (sheetCounter > 0)
                        sheet.IsVisible = false;
                    sheetCounter++;
                }


                for (int i = 0; i < wb.Worksheets.Count; i++)
                {

                    foreach (Cell sheetCell in wb.Worksheets[i].Cells)
                    {
                        var cellStyle = sheetCell.GetStyle();
                        cellStyle.ForegroundColor = Color.White;
                        cellStyle.Pattern = BackgroundType.Solid;

                        sheetCell.SetStyle(cellStyle);
                    }
                    //Access the worksheet

                    Worksheet worksheet = wb.Worksheets[i];


                    //Access the maximum display range
                    Aspose.Cells.Range range = worksheet.Cells.MaxDisplayRange;
                    int expandMaxRow = range.RowCount + 10;
                    int expandMaxColumn = range.ColumnCount - 1;

                    worksheet.Cells.CreateRange("A1", CellsHelper.CellIndexToName(expandMaxRow, expandMaxColumn));

                    //Set the print area of worksheet
                    worksheet.PageSetup.PrintArea =
                        "A1:" + CellsHelper.CellIndexToName(expandMaxRow, expandMaxColumn);

                    //Set all margins 0

                    //worksheet.PageSetup.TopMargin = 0;
                    //worksheet.PageSetup.BottomMargin = 0;
                    //worksheet.PageSetup.LeftMargin = 0;
                    //worksheet.PageSetup.RightMargin = 0;


                }


                var ff = new FileInfo(fullFile);

                string rootFolder = ff.DirectoryName;
                var pathPdf = $"{rootFolder}\\{pdfName}";

                if (File.Exists(pathPdf))
                    File.Delete(pathPdf);

                PdfSaveOptions options = new PdfSaveOptions { OnePagePerSheet = true };

                wb.Save(pathPdf, options);
                if (!string.IsNullOrEmpty(signer))
                {
                    var sig = new SignatureConnect();
                    var sigRt = sig.SingNormal(pathPdf, null, signer, "Người lập", true);
                    Log.Information("CreatePdf sig.SingNormal {0}:{1}", sigRt.IsError, sigRt.Message);
                    if (!sigRt.IsError)
                    {
                        DeleteOlder(sigRt.OriginalPathOlder);
                        return sigRt.SignaturePath.Replace(physicalFolder, string.Empty).NormalizePath(); ;
                    }
                    return string.Empty;
                }

                return pathPdf.Replace(physicalFolder, string.Empty).NormalizePath();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return string.Empty;
            }
        }

        public void MoveTrash(string pathOlder, string physicalFolder)
        {
            try
            {
                string fullTempPath = $"{physicalFolder}\\trash";
                var fileInfo = new FileInfo(pathOlder);
                if (!Directory.Exists(fullTempPath))
                    Directory.CreateDirectory(fullTempPath);
                string fullFilePath = Path.Combine(fullTempPath, fileInfo.Name);
                File.Move(pathOlder, fullFilePath);
            }
            catch (Exception e)
            {
                Log.Error(e, "error");
            }
        }

        public void DeleteOlder(string pathOlder)
        {
            try
            {
                File.Delete(pathOlder);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
        }

    }

    public class StartEndMatcher
    {
        public int start { get; set; }
        public int end { get; set; }
        public string word { get; set; }
        public bool founded { get; set; }
    }

    public class SignatureResponse
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string SignaturePath { get; set; }
        public string OriginalPathOlder { get; set; }
    }
}
