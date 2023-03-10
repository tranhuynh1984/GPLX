#pragma checksum "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "73ce160352f77c3606b909c1bfdcd6ae8d10dbb2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProfitPlan__ViewHistory), @"mvc.1.0.view", @"/Views/ProfitPlan/_ViewHistory.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"73ce160352f77c3606b909c1bfdcd6ae8d10dbb2", @"/Views/ProfitPlan/_ViewHistory.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9dcf69e1b5c9f96f3b7ae5bf93153fc34949a3ec", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_ProfitPlan__ViewHistory : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IList<GPLX.Core.DTO.Response.ProfitPlan.ProfitPlanViewHistoryResponse>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div class=\"row\" id=\"viewHistories\">\r\n    <div class=\"col-12 col-sm-12\">\r\n");
#nullable restore
#line 5 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
          
            if (Model.Count > 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <div class=""col-md-12 text-right mb-2"">
                    <a class=""badge badge-success p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('all');"">
                        T???t c???
                    </a>
                    <a class=""badge badge-success p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('approval');"">
                        <i class=""fad fa-check mr-1""></i> ???? duy???t
                    </a>
                    <a class=""badge badge-danger p-2 fs-90"" href=""#"" onclick=""costJsBase.HistoriesFilter('decline');"">
                        <i class=""fad fa-times mr-1""></i> T??? ch???i
                    </a>
                </div>
");
#nullable restore
#line 19 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"

                foreach (var costEstimateItemLogResponse in Model)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"timeline\"");
            BeginWriteAttribute("prop-stats", " prop-stats=\"", 1031, "\"", 1127, 1);
#nullable restore
#line 22 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
WriteAttributeValue("", 1044, string.IsNullOrEmpty(costEstimateItemLogResponse.Reason) ? "approval": "decline", 1044, 83, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                        <div>\r\n                            <div class=\"timeline-item\">\r\n                                <span class=\"time\" style=\"font-size: 0.9rem\"><i class=\"fas fa-clock\"></i> ");
#nullable restore
#line 25 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                                                      Write(costEstimateItemLogResponse.TimeChange);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                                <h3 class=\"timeline-header\"><a class=\"blue-color\" onclick=\"javascript: void (0);\">");
#nullable restore
#line 26 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                                                              Write(costEstimateItemLogResponse.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a></h3>\r\n                                <div class=\"timeline-body\">\r\n                                    <b class=\"blue-color font-weight-bold\">Ng?????i duy???t:</b> ");
#nullable restore
#line 28 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                                        Write(costEstimateItemLogResponse.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                                    <b class=\"blue-color font-italic\">Ch???c v???:</b> ");
#nullable restore
#line 29 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                               Write(costEstimateItemLogResponse.PositionName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                                    <b class=\"blue-color font-italic\">Tr???ng th??i:</b> <span");
            BeginWriteAttribute("class", " class=\"", 1966, "\"", 2065, 1);
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
WriteAttributeValue("", 1974, string.IsNullOrEmpty(costEstimateItemLogResponse.Reason) ? "text-success": "text-danger", 1974, 91, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                                                                                                                                            Write(costEstimateItemLogResponse.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span> <br />\r\n\r\n");
#nullable restore
#line 32 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                     if (!string.IsNullOrEmpty(costEstimateItemLogResponse.Reason))
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <b>L?? do:</b><br />\r\n");
#nullable restore
#line 35 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                    Write(costEstimateItemLogResponse.Reason);

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                                                                             
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </div>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n");
#nullable restore
#line 41 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
                }
            }
        

#line default
#line hidden
#nullable disable
            WriteLiteral("        <p class=\"text-center\"");
            BeginWriteAttribute("style", "\r\n           style=\"", 2652, "\"", 2718, 2);
            WriteAttributeValue("", 2672, "display:", 2672, 8, true);
#nullable restore
#line 45 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\ProfitPlan\_ViewHistory.cshtml"
WriteAttributeValue(" ", 2680, Model.Count > 0 ? "none" : "block", 2681, 37, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n            K??? ho???ch ch??a c?? d??? li???u l???ch s???!\r\n        </p>\r\n    </div>\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IList<GPLX.Core.DTO.Response.ProfitPlan.ProfitPlanViewHistoryResponse>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
