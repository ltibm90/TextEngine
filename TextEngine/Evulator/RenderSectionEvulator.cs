using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class RenderSectionEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var cr = this.ConditionSuccess(tag, "if", vars);
            var result = new TextEvulateResult()
            {
                Result = TextEvulateResultEnum.EVULATE_NOACTION
            };
            if (!cr) return result;
            var attr = tag.ElemAttr.FirstAttribute;
            bool globalnoprint = tag.GetAttribute("noprint", "0") == "1";
            if (attr != null)
            {
                var attrResult = this.EvulateAttributeName(attr, vars);
                string name = attrResult != null ? attrResult.ToString() : "";
                if (!string.IsNullOrEmpty(name))
                {
                    TextElements sections = tag.TagInfo.SingleData as TextElements;
                    var list = sections.GetElements(m => m.ElemAttr.FirstAttribute != null && m.ElemAttr.FirstAttribute.Name == name);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var r = list[i].EvulateValue();
                        bool isnoprint = list[i].GetAttribute("noprint", "0") == "1";
                        if (isnoprint) r.Result = TextEvulateResultEnum.EVULATE_NOACTION;
                        if (list.Count == 1) return  (globalnoprint) ? null : r;
                        if (!globalnoprint && result.Result == TextEvulateResultEnum.EVULATE_TEXT)
                        {
                            result.TextContent += r.TextContent;
                        }
                    }
                    result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                }

            }
            return (globalnoprint) ? null : result;
        }
    }
}
