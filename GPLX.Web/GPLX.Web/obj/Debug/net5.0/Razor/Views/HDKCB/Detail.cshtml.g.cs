#pragma checksum "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a15a7303a1b268ed1a34f08e832be72649c96b80"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_HDKCB_Detail), @"mvc.1.0.view", @"/Views/HDKCB/Detail.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a15a7303a1b268ed1a34f08e832be72649c96b80", @"/Views/HDKCB/Detail.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9dcf69e1b5c9f96f3b7ae5bf93153fc34949a3ec", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_HDKCB_Detail : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<GPLX.Core.DTO.Response.HDKCB.HDKCBDetailResponse>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("___createForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
  
    string viewMode = ViewBag.ViewMode ?? string.Empty;

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"row\">\r\n    <input type=\"hidden\" id=\"___record\"");
            BeginWriteAttribute("value", " value=\"", 179, "\"", 201, 1);
#nullable restore
#line 6 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 187, Model?.IDHD, 187, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n    <div class=\"col-md-12 col-sm-12\">\r\n        <div class=\"card\">\r\n            <div class=\"card-header\">Th??ng tin h???p ?????ng</div>\r\n            <div class=\"card-body\">\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a15a7303a1b268ed1a34f08e832be72649c96b804408", async() => {
                WriteLiteral("\r\n                    <div class=\"row\">\r\n                        <div class=\"col-md-4\">\r\n                            <div class=\"form-group\">\r\n                                <label>M?? H??</label>\r\n                                <input");
                BeginWriteAttribute("value", " value=\"", 650, "\"", 672, 1);
#nullable restore
#line 16 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 658, Model?.MaHD, 658, 14, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 710, "\"", 747, 1);
#nullable restore
#line 16 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 721, viewMode.Equals("view"), 721, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"/>
                            </div>
                        </div>
                        <div class=""col-md-4"">
                            <div class=""form-group"">
                                <label>T??n H??</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 1015, "\"", 1038, 1);
#nullable restore
#line 22 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1023, Model?.TenHD, 1023, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 1076, "\"", 1113, 1);
#nullable restore
#line 22 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1087, viewMode.Equals("view"), 1087, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"/>
                            </div>
                        </div>
                        <div class=""col-md-4"">
                            <div class=""form-group"">
                                <label>Ng??y k??</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 1382, "\"", 1404, 1);
#nullable restore
#line 28 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1390, Model?.Ngay, 1390, 14, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 1442, "\"", 1479, 1);
#nullable restore
#line 28 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1453, viewMode.Equals("view"), 1453, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"/>
                            </div>
                        </div>
                    </div>
                    
                    <div class=""row"">
                        <div class=""col-md-3"">
                            <div class=""form-group"">
                                <label>IDHD</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 1834, "\"", 1856, 1);
#nullable restore
#line 37 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1842, Model?.IDHD, 1842, 14, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 1894, "\"", 1931, 1);
#nullable restore
#line 37 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 1905, viewMode.Equals("view"), 1905, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                            </div>
                        </div>
                        <div class=""col-md-3"">
                            <div class=""form-group"">
                                <label>TG b???t ?????u</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 2204, "\"", 2224, 1);
#nullable restore
#line 43 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 2212, Model?.ND, 2212, 12, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 2262, "\"", 2299, 1);
#nullable restore
#line 43 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 2273, viewMode.Equals("view"), 2273, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"/>
                            </div>
                        </div>
                        <div class=""col-md-3"">
                            <div class=""form-group"">
                                <label>TG k???t th??c</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 2572, "\"", 2592, 1);
#nullable restore
#line 49 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 2580, Model?.NS, 2580, 12, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 2630, "\"", 2667, 1);
#nullable restore
#line 49 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 2641, viewMode.Equals("view"), 2641, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"/>
                            </div>
                        </div>
                        <div class=""col-md-3"">
                            <div class=""form-group"">
                                <label>Tr???ng th??i</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 2939, "\"", 2969, 1);
#nullable restore
#line 55 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\HDKCB\Detail.cshtml"
WriteAttributeValue("", 2947, Model?.IsActiveName, 2947, 22, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\" disabled/>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
            </div>
        </div>
    </div>
    <div class=""col-md-12 col-sm-12"">
            <div class=""card"">
                <div class=""card-header"">
                    <span style=""font-size:1.5rem"">Chi ti???t d???ch v??? h???p ?????ng KCB</span>
                    <div style=""width:200px; display:inline-block; float:right"">
                        <input type=""text"" class=""form-control form-control-sm"" placeholder=""T??m nhanh"" id=""txtSearchServicePopupInput"" name=""Keywords"">
                    </div>
                </div>
                <div class=""card-body"">
                    <table id=""tblServiceDetailList"" class=""table table-sm table-hover stripe table-bordered nowrap"" style=""width: 100%"">
                        <thead>
                            <tr>
                                <th>STT</th>
                                <th>M?? DV</th>
                                <th>T??n DV</th>
                                <th>??VT</th>
                                <th>S??? d???ng</th");
            WriteLiteral(@">
                                <th>Gi?? BV</th>
                                <th>Gi?? ??u ????i</th>
                                <th>Th??nh ti???n</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                        <tfoot>
                            <tr>
                                <th colspan=""7"">T???ng ti???n</th>
                                <th></th>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
        </div>
</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<GPLX.Core.DTO.Response.HDKCB.HDKCBDetailResponse> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
