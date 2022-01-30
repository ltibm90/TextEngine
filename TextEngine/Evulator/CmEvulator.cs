using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Macros;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public  class CallMacroEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if (string.IsNullOrEmpty(tag.ElemAttr.FirstAttribute?.Name)) return null;
            if(tag.ElemAttr.FirstAttribute ==null || !this.ConditionSuccess(tag, "if", vars)) return null;
            bool globalnoprint = tag.GetAttribute("noprint", "0") == "1";
            var attrResult = this.EvulateAttributeName(tag.ElemAttr.FirstAttribute, vars);
            string name = attrResult != null ? attrResult.ToString() : "";
            var teRes = new TextEvulateResult()
            {
                Result = TextEvulateResultEnum.EVULATE_NOACTION
            };
            if (string.IsNullOrEmpty(name)) return null;
            var elements = this.GetMacroElements(name);
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element != null)
                {
                    var newelement = new KeyValues<object>();
                    for (int j = 0; j < element.ElemAttr.Count; j++)
                    {
                        if (element.ElemAttr[j].Name == "name" || element.ElemAttr[j].Name == "noprint") continue;
                        newelement[element.ElemAttr[j].Name] = this.EvulateAttribute(element.ElemAttr[i], vars);
                    }
                    bool isnoprint = element.GetAttribute("noprint", "0") == "1";
                    for (int j = 1; j < tag.ElemAttr.Count; j++)
                    {
                        var key = tag.ElemAttr[j];
                        if (key.Name == "noprint") continue;
                        newelement[key.Name] = this.EvulateAttribute(key, vars);
                    }
                    var result = element.EvulateValue(0, 0, newelement);
                    if (isnoprint) result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
                    if (elements.Count == 1) return (globalnoprint) ? null : result;
                    if(!globalnoprint && result.Result == TextEvulateResultEnum.EVULATE_TEXT)
                    {
                        teRes.TextContent += result.TextContent;
                    }
 
                }
            }



            return (globalnoprint) ? null : teRes;
        }
        protected TextElement GetMacroElement(string name)
        {

            return this.Evulator.SavedMacrosList.GetMacro(name);
        }
        protected List<TextElement> GetMacroElements(string name)
        {
            return this.Evulator.SavedMacrosList.GetMacros(name);
        }

    }
}
