using System;
using IntexII.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IntexII.Controllers.Infrastructure
{


    [HtmlTargetElement("div", Attributes = "page-dynamic")]
    public class PaginationTagHelper : TagHelper
    {

        private IUrlHelperFactory uhf;

        public PaginationTagHelper(IUrlHelperFactory temp)
        {
            uhf = temp;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext vc { get; set; }

        // Different than ViewContext
        public PageInfo PageDynamic { get; set; }
        public string PageAction { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }


        public override void Process(TagHelperContext thc, TagHelperOutput tho)
        {
            IUrlHelper uh = uhf.GetUrlHelper(vc);

            TagBuilder final = new TagBuilder("div");

            for (int i = PageDynamic.CurrentPage - 3; i <= PageDynamic.CurrentPage + 3; i++)
            {
                if (i <= 0)
                {
                    continue;
                }
                else
                {
                    TagBuilder tb = new TagBuilder("a");

                    tb.Attributes["href"] = uh.Action(PageAction, new { pageNum = i });
                    tb.Attributes["style"] = "margin-right: 10px; margin-left: 10px;";


                    tb.AddCssClass(i == PageDynamic.CurrentPage ? PageClassSelected : PageClassNormal);


                    tb.InnerHtml.Append(i.ToString());

                    final.InnerHtml.AppendHtml(tb);
                }

            }

            tho.Content.AppendHtml(final.InnerHtml);
        }
    }


}

