#pragma checksum "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9fbed2730728f0313fcd695e430de01452dcf553"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CostStatuses_Create), @"mvc.1.0.view", @"/Views/CostStatuses/Create.cshtml")]
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
#nullable restore
#line 1 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
using GPLX.Web.Models.CostStatus;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9fbed2730728f0313fcd695e430de01452dcf553", @"/Views/CostStatuses/Create.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9dcf69e1b5c9f96f3b7ae5bf93153fc34949a3ec", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_CostStatuses_Create : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<GPLX.Database.Models.CostStatuses>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("___createForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery-validation/dist/jquery.validate.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/javascript"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/jquery.customValidate.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/status/create.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
  
    var dropdownData = ViewBag.DropdownData as CostStatusListModel ?? new CostStatusListModel();
    string viewMode = ViewBag.ViewMode ?? string.Empty;
    string display = Model == null ? "Thêm mới trạng thái" : ViewBag.ViewMode == "view" ? "Thông tin trạng thái" : "Chỉnh sửa thông tin trạng thái";
    string record = ViewBag.RawId ?? string.Empty;

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"row\">\r\n    <input type=\"hidden\" id=\"___record\"");
            BeginWriteAttribute("value", " value=\"", 499, "\"", 516, 1);
#nullable restore
#line 10 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 507, record, 507, 9, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n    <div class=\"col-md-12\">\r\n        <div class=\"card\">\r\n            <div class=\"card-header\">\r\n                <div class=\"card-title\">\r\n                    ");
#nullable restore
#line 15 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
               Write(display);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n            <div class=\"card-body\">\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf5536971", async() => {
                WriteLiteral(@"
                    <div class=""row"">
                        <div class=""col-md-6 col-sm-12"">
                            <div class=""form-group"">
                                <label for=""inputName"">Mã trạng thái</label>
                                <input id=""inputName"" name=""inputName""");
                BeginWriteAttribute("value", " value=\"", 1113, "\"", 1135, 1);
#nullable restore
#line 24 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 1121, Model?.Name, 1121, 14, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 1173, "\"", 1210, 1);
#nullable restore
#line 24 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 1184, viewMode.Equals("view"), 1184, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                            </div>
                        </div>
                        <div class=""col-md-3 col-sm-6"">
                            <div class=""form-group"">
                                <label>Giá trị</label>
                                <input");
                BeginWriteAttribute("value", " value=\"", 1489, "\"", 1512, 1);
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 1497, Model?.Value, 1497, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" id=\"inputValue\" name=\"inputValue\" type=\"number\" class=\"form-control form-control-sm\"");
                BeginWriteAttribute("disabled", " disabled=\"", 1598, "\"", 1635, 1);
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 1609, viewMode.Equals("view"), 1609, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                            </div>
                        </div>
                        <div class=""col-md-3 col-sm-6"">
                            <div class=""form-group"">
                                <label>Đối tượng</label>
                                <i class=""fad fa-question-square"" data-toggle=""tooltip"" data-placement=""top"" title=""Phân biệt luồng trạng thái giữa SUB và đơn vị thành viên""></i>
                                <select class=""form-control form-control-sm"" id=""selectSubject"" name=""selectSubject""");
                BeginWriteAttribute("disabled", " disabled=\"", 2174, "\"", 2211, 1);
#nullable restore
#line 37 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 2185, viewMode.Equals("view"), 2185, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n");
#nullable restore
#line 38 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                     foreach (var c in dropdownData.StatusForSubject)
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55310746", async() => {
#nullable restore
#line 40 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                                                                                                  Write(c.Text);

#line default
#line hidden
#nullable disable
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 40 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                           WriteLiteral(c.Value);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "selected", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 40 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
AddHtmlAttributeValue("", 2416, Model?.StatusForSubject.Equals(c.Value), 2416, 42, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 41 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                    }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                                </select>
                            </div>
                        </div>
                    </div>

                    <div class=""row"">
                        <div class=""col-md-4"">
                            <div class=""form-group"">
                                <label>Thời gian</label>
                                <i class=""fad fa-question-square"" data-toggle=""tooltip"" data-placement=""top"" title=""Phân biệt trạng thái áp dụng cho dự trù tuần hay dự trù năm""></i>
                                <select class=""form-control form-control-sm"" id=""selectStatusForCostEstimateType"" name=""selectStatusForCostEstimateType""");
                BeginWriteAttribute("disabled", " disabled=\"", 3192, "\"", 3229, 1);
#nullable restore
#line 52 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 3203, viewMode.Equals("view"), 3203, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n");
#nullable restore
#line 53 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                     foreach (var c in dropdownData.StatusForCostEstimateType)
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55314823", async() => {
#nullable restore
#line 55 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                                                                                                           Write(c.Text);

#line default
#line hidden
#nullable disable
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 55 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                           WriteLiteral(c.Value);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "selected", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 55 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
AddHtmlAttributeValue("", 3443, Model?.StatusForCostEstimateType.Equals(c.Value), 3443, 51, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 56 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                    }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                                </select>
                            </div>
                        </div>
                        <div class=""col-md-3"">
                            <div class=""form-group"">
                                <label>Nhóm dữ liệu</label>
                                <i class=""fad fa-question-square"" data-toggle=""tooltip"" data-placement=""top"" title=""Cùng một mã trạng thái nhưng sẽ áp dụng cho các nhóm dữ liệu khác nhau""></i>
                                <select class=""form-control form-control-sm"" id=""selectTypes"" name=""selectTypes""");
                BeginWriteAttribute("disabled", " disabled=\"", 4133, "\"", 4170, 1);
#nullable restore
#line 64 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 4144, viewMode.Equals("view"), 4144, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n");
#nullable restore
#line 65 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                     foreach (var c in dropdownData.Types)
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55318801", async() => {
#nullable restore
#line 67 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                                                                                      Write(c.Text);

#line default
#line hidden
#nullable disable
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 67 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                           WriteLiteral(c.Value);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "selected", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 67 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
AddHtmlAttributeValue("", 4364, Model?.Type.Equals(c.Value), 4364, 30, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 68 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
                                    }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                                </select>
                            </div>
                        </div>
                        <div class=""col-md-2"">
                            <div class=""form-group"">
                                <label>Thứ tự</label>
                                <i class=""fad fa-question-square"" data-toggle=""tooltip"" data-placement=""top"" title=""Quy định thứ tự của trạng thái trong luồng phê duyệt hoặc từ chối""></i>
                                <input type=""number"" class=""form-control form-control-sm"" id=""inputOrder"" name=""inputOrder""");
                BeginWriteAttribute("value", " value=\"", 5033, "\"", 5056, 1);
#nullable restore
#line 76 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 5041, Model?.Order, 5041, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                            </div>
                        </div>
                    </div>

                    <div class=""row"">
                        <div class=""col-md-2"">
                            <div class=""form-group"">
                                <label>Trạng thái phê duyệt</label> <i class=""fad fa-question-square"" data-toggle=""tooltip"" data-placement=""top"" title=""Dùng để phân biệt giữa trạng thái phê duyệt và các trạng thái khác""></i>
                                <div class=""custom-control custom-checkbox"">
                                    <input class=""custom-control-input"" type=""checkbox"" id=""checkApprove""");
                BeginWriteAttribute("checked", " checked=\"", 5710, "\"", 5744, 1);
#nullable restore
#line 86 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 5720, Model?.IsApprove == 1, 5720, 24, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@">
                                    <label for=""checkApprove"" class=""custom-control-label"">Phê duyệt</label>
                                </div>
                            </div>
                        </div> 

                        <div class=""col-md-2"">
                            <div class=""form-group"">
                                <label>Bước ký số</label>
                                <div class=""custom-control custom-checkbox"">
                                    <input class=""custom-control-input"" type=""checkbox"" id=""checkSigned""");
                BeginWriteAttribute("checked", " checked=\"", 6312, "\"", 6338, 1);
#nullable restore
#line 96 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
WriteAttributeValue("", 6322, Model?.Singed, 6322, 16, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@">
                                    <label for=""checkSigned"" class=""custom-control-label"">Ký số</label>
                                </div>
                            </div>
                        </div>
                    </div>
                ");
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
            <div class=""card-footer"">
                <div class=""card-footer text-right"">
                    <button type=""button"" class=""btn btn-sm med-btn-outline-primary"" id=""btnSave"">
                        <i class=""fad fa-save""></i> Lưu lại
                    </button>
                    <button type=""button"" class=""btn btn-sm med-btn-outline-primary"" data-back=""true"">
                        Quay lại
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>


");
            DefineSection("scripts", async() => {
                WriteLiteral(" \r\n");
#nullable restore
#line 120 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
     if (!string.IsNullOrEmpty(record) && Model == null)
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <script type=\"text/javascript\">\r\n            costJsBase.EventNotify(\'error\', \'Không tìm thấy dữ liệu yêu cầu!\')\r\n        </script>\r\n");
#nullable restore
#line 125 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\CostStatuses\Create.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55326977", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55328164", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9fbed2730728f0313fcd695e430de01452dcf55329351", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<GPLX.Database.Models.CostStatuses> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
