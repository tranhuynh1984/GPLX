#pragma checksum "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1f5f97603d0ca1774a7953d9ca079496538b5c1a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Components_SideBar__MenuPartial), @"mvc.1.0.view", @"/Views/Shared/Components/SideBar/_MenuPartial.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f5f97603d0ca1774a7953d9ca079496538b5c1a", @"/Views/Shared/Components/SideBar/_MenuPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9dcf69e1b5c9f96f3b7ae5bf93153fc34949a3ec", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Shared_Components_SideBar__MenuPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IList<MenuBuilder>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<ul class=\"nav nav-pills nav-sidebar flex-column\" data-widget=\"treeview\" role=\"menu\" data-accordion=\"false\">\r\n    <!-- Add icons to the links using the .nav-icon class\r\n    with font-awesome or any other icon font library -->\r\n");
#nullable restore
#line 14 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
     foreach (var m in Model)
    {
        if (!string.IsNullOrEmpty(m.Parent.Url))
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <li class=\"nav-item\" ");
#nullable restore
#line 18 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                             Write(_writeIdentify(m.Parent.Module));

#line default
#line hidden
#nullable disable
            WriteLiteral(" specifer=\"");
#nullable restore
#line 18 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                                                         Write(m.Parent.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" style=\"display:");
#nullable restore
#line 18 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                                                                                       Write(!string.IsNullOrEmpty(m.Parent.Module)? "none": "block");

#line default
#line hidden
#nullable disable
            WriteLiteral("\">\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 724, "\"", 744, 1);
#nullable restore
#line 19 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
WriteAttributeValue("", 731, m.Parent.Url, 731, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"nav-link\">\r\n                    <i");
            BeginWriteAttribute("class", " class=\"", 787, "\"", 825, 2);
            WriteAttributeValue("", 795, "nav-icon", 795, 8, true);
#nullable restore
#line 20 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
WriteAttributeValue(" ", 803, m.Parent.IconClass, 804, 21, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("></i>\r\n                    <p>");
#nullable restore
#line 21 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                  Write(m.Parent.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n                </a>\r\n            </li>\r\n");
#nullable restore
#line 24 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
        }
        else
        {
            if (m.ChildFunctions.Count == 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <li class=\"nav-header\">");
#nullable restore
#line 29 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                  Write(m.Parent.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n");
#nullable restore
#line 30 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <li class=\"nav-item\" ");
#nullable restore
#line 33 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                 Write(_writeIdentify(m.Parent.Module));

#line default
#line hidden
#nullable disable
            WriteLiteral(" specifer=\"");
#nullable restore
#line 33 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                                                             Write(m.Parent.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" style=\"display: ");
#nullable restore
#line 33 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                                                                                            Write(!string.IsNullOrEmpty(m.Parent.Module)? "none": "block");

#line default
#line hidden
#nullable disable
            WriteLiteral("\">\r\n                    <a href=\"javascript: void(0);\" class=\"nav-link without\">\r\n                        <i");
            BeginWriteAttribute("class", " class=\"", 1400, "\"", 1438, 2);
            WriteAttributeValue("", 1408, "nav-icon", 1408, 8, true);
#nullable restore
#line 35 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
WriteAttributeValue(" ", 1416, m.Parent.IconClass, 1417, 21, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("></i>\r\n                        <p>\r\n                            ");
#nullable restore
#line 37 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                       Write(m.Parent.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            <i class=\"right fas fa-angle-left\"></i>\r\n                        </p>\r\n                    </a>\r\n                    <ul class=\"nav nav-treeview\" id=\"parentId\" >\r\n");
#nullable restore
#line 42 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                         foreach (var mChildFunction in m.ChildFunctions)
                        {
                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 44 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                       Write(await Component.InvokeAsync("SubMenu", mChildFunction));

#line default
#line hidden
#nullable disable
#nullable restore
#line 44 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                                                                                   
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </ul>\r\n                </li>\r\n");
#nullable restore
#line 48 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
            }
        }
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("     \r\n    <li class=\"nav-item\" identifier=\"GENERAL\" specifer=\"44\"");
            BeginWriteAttribute("style", " style=\"", 2073, "\"", 2081, 0);
            EndWriteAttribute();
            WriteLiteral(">\r\n        <a href=\"/QLDM/Index\" class=\"nav-link\">\r\n            <i class=\"nav-icon fad fa-home\"></i>\r\n            <p>Quản lý danh mục</p>\r\n        </a>\r\n    </li>\r\n    <li class=\"nav-item\" identifier=\"GENERAL\" specifer=\"44\"");
            BeginWriteAttribute("style", " style=\"", 2305, "\"", 2313, 0);
            EndWriteAttribute();
            WriteLiteral(">\r\n        <a href=\"/DMCTV/List\" class=\"nav-link\">\r\n            <i class=\"nav-icon fad fa-home\"></i>\r\n            <p>Danh mục CTV</p>\r\n        </a>\r\n    </li>\r\n     <li class=\"nav-item\" identifier=\"GENERAL\" specifer=\"35\"");
            BeginWriteAttribute("style", " style=\"", 2534, "\"", 2542, 0);
            EndWriteAttribute();
            WriteLiteral(@">
        <a href=""/DeXuat/List"" class=""nav-link without"">
            <i class=""nav-icon fad fa-house-user""></i>
            <p>
                Quản lý đề xuất
                <i class=""right fas fa-angle-left""></i>
            </p>
        </a>
    </li>
    
    <li class=""nav-item"">
        <a href=""#"" class=""nav-link without"">
            <i class=""fad fa-sign-out""></i>
            <p>Version: ");
#nullable restore
#line 77 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
                   Write(System.DateTime.Now.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n        </a>\r\n    </li>\r\n\r\n");
#nullable restore
#line 81 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
     if ((bool)ViewBag.Loged)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <li class=\"nav-item\">\r\n            <a href=\"/Account/Logout\" class=\"nav-link without\" onclick=\"costJsBase.onSignout();\">\r\n                <i class=\"fad fa-sign-out\"></i>\r\n                <p>Đăng xuất</p>\r\n            </a>\r\n        </li>\r\n");
#nullable restore
#line 89 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</ul>\r\n");
        }
        #pragma warning restore 1998
#nullable restore
#line 3 "D:\Dev\GPLX\GPLX\GPLX.Web\GPLX.Web\Views\Shared\Components\SideBar\_MenuPartial.cshtml"
 
    string _writeIdentify(string module)
    {
        if (!string.IsNullOrEmpty(module))
            return $"identifier={module}";
        return null;
    }

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IList<MenuBuilder>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
