#pragma checksum "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "239f0860f1e2ccfb35b9f45f765ff080effca77e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_InvestmentPlan__ViewHistory), @"mvc.1.0.view", @"/Views/InvestmentPlan/_ViewHistory.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\_ViewImports.cshtml"
using GPLX.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\_ViewImports.cshtml"
using GPLX.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"239f0860f1e2ccfb35b9f45f765ff080effca77e", @"/Views/InvestmentPlan/_ViewHistory.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9dcf69e1b5c9f96f3b7ae5bf93153fc34949a3ec", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_InvestmentPlan__ViewHistory : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IList<GPLX.Core.DTO.Response.InvestmentPlan.InvestmentPlanViewHistoryResponse>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div class=\"row\" id=\"viewHistories\">\r\n    <div class=\"col-12 col-sm-12\">\r\n");
#nullable restore
#line 5 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
          
            if (Model.Count > 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <div class=""col-md-12 text-right mb-2"">
                    <a class=""badge badge-success p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('all');"">
                        Tất cả
                    </a>
                    <a class=""badge badge-success p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('approval');"">
                        <i class=""fad fa-check mr-1""></i> Đã duyệt
                    </a>
                    <a class=""badge badge-danger p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('decline');"">
                        <i class=""fad fa-times mr-1""></i> Từ chối
                    </a>
                </div>
");
#nullable restore
#line 19 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"

                foreach (var costEstimateItemLogResponse in Model)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"timeline\"");
            BeginWriteAttribute("prop-stats", " prop-stats=\"", 1039, "\"", 1135, 1);
#nullable restore
#line 22 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
WriteAttributeValue("", 1052, string.IsNullOrEmpty(costEstimateItemLogResponse.Reason) ? "approval": "decline", 1052, 83, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                        <div>\r\n                            <div class=\"timeline-item\">\r\n                                <span class=\"time\" style=\"font-size: 0.9rem\"><i class=\"fas fa-clock\"></i> ");
#nullable restore
#line 25 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                                                      Write(costEstimateItemLogResponse.TimeChange);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                                <h3 class=\"timeline-header\"><a class=\"blue-color\" onclick=\"javascript: void (0);\">");
#nullable restore
#line 26 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                                                              Write(costEstimateItemLogResponse.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a></h3>\r\n                                <div class=\"timeline-body\">\r\n                                    <b class=\"blue-color font-weight-bold\">Người duyệt:</b> ");
#nullable restore
#line 28 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                                        Write(costEstimateItemLogResponse.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                                    <b class=\"blue-color font-italic\">Chức vụ:</b> ");
#nullable restore
#line 29 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                               Write(costEstimateItemLogResponse.PositionName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                                    <b class=\"blue-color font-italic\">Trạng thái:</b> <span");
            BeginWriteAttribute("class", " class=\"", 1974, "\"", 2073, 1);
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
WriteAttributeValue("", 1982, string.IsNullOrEmpty(costEstimateItemLogResponse.Reason) ? "text-success": "text-danger", 1982, 91, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                                                                                                                                            Write(costEstimateItemLogResponse.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span> <br />\r\n\r\n");
#nullable restore
#line 32 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                     if (!string.IsNullOrEmpty(costEstimateItemLogResponse.Reason))
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <b>Lý do:</b><br />\r\n");
#nullable restore
#line 35 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                    Write(costEstimateItemLogResponse.Reason);

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                                                                             
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </div>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n");
#nullable restore
#line 41 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
                }
            }
        

#line default
#line hidden
#nullable disable
            WriteLiteral("        <p class=\"text-center\"");
            BeginWriteAttribute("style", "\r\n           style=\"", 2660, "\"", 2726, 2);
            WriteAttributeValue("", 2680, "display:", 2680, 8, true);
#nullable restore
#line 45 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\InvestmentPlan\_ViewHistory.cshtml"
WriteAttributeValue(" ", 2688, Model.Count > 0 ? "none" : "block", 2689, 37, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n            Kế hoạch chưa có dữ liệu lịch sử!\r\n        </p>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IList<GPLX.Core.DTO.Response.InvestmentPlan.InvestmentPlanViewHistoryResponse>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591