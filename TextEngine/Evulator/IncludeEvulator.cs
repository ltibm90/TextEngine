using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class IncludeEvulator : BaseEvulator
    {
        private string GetLastDir()
        {
            var value = this.Evulator.LocalVariables.GetValue("_DIR_");
            if (value == null || string.IsNullOrEmpty(value.ToString())) return string.Empty;
            return value.ToString() + "\\";
        }
        private TextEvulateResult Render_Parse(TextElement tag, object vars)
        {
            var loc = this.GetLastDir() + this.EvulateAttribute(tag.ElemAttr["name"], vars)?.ToString();
            if (!this.ConditionSuccess(tag, "if", vars) || !File.Exists(loc)) return null;
            this.SetLocal("_DIR_", Path.GetDirectoryName(loc));
            string xpath = tag.GetAttribute("xpath");
            bool xpathold = false;
            if (string.IsNullOrEmpty(xpath))
            {
                xpath = tag.GetAttribute("xpath_old");
                xpathold = true;
            }

            var content = File.ReadAllText(loc);
            var result = new TextEvulateResult();

            result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
            tag.Parent.SubElements.Remove(tag);        
            if(string.IsNullOrEmpty(xpath))
            {
                this.Evulator.Parse(tag.Parent, content);
            }
            else
            {
                var tempitem = new TextElement();
                tempitem.ElemName = "#document";
                this.Evulator.Parse(tempitem, content);
                TextElements elems = null;
                if (!xpathold)
                {
                    elems = tempitem.FindByXPath(xpath);
                }
                else
                {
                    elems = tempitem.FindByXPathOld(xpath);
                }
                for (int i = 0; i < elems.Count; i++)
                {
                    elems[i].Parent = tag.Parent;
                    tag.Parent.SubElements.Add(@elems[i]);
                }
            }
            return result;
        }
        private TextEvulateResult Render_Default(TextElement tag, object vars)
        {
            var loc = this.GetLastDir() + this.EvulateAttribute(tag.ElemAttr["name"], vars)?.ToString();
            var parse = tag.GetAttribute("parse", "true");
            if (!File.Exists(loc) || !this.ConditionSuccess(tag, "if", vars)) return null;
            this.SetLocal("_DIR_", Path.GetDirectoryName(loc));
            var content = File.ReadAllText(loc);
            var result = new TextEvulateResult();

            if (parse == "false")
            {
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                result.TextContent = content;
            }
            else
            {
                var tempelem = new TextElement
                {
                    ElemName = "#document",
                    BaseEvulator = this.Evulator
                };
                var tempelem2 = new TextElement
                {
                    ElemName = "#document",
                    BaseEvulator = this.Evulator
                };

                string xpath = tag.GetAttribute("xpath");
                bool xpathold = false;
                if (string.IsNullOrEmpty(xpath))
                {
                    xpath = tag.GetAttribute("xpath_old");
                    xpathold = true;
                }
      
                this.Evulator.Parse(tempelem2, content);
                if (string.IsNullOrEmpty(xpath))
                {
                    tempelem = tempelem2;
                }
                else
                {
                    TextElements elems = null;
                    if (!xpathold)
                    {
                        elems = tempelem2.FindByXPath(xpath);
                    }
                    else
                    {
                        elems = tempelem2.FindByXPathOld(xpath);
                    }
                    for (int i = 0; i < elems.Count; i++)
                    {
                        elems[i].Parent = tempelem;
                        tempelem.SubElements.Add(elems[i]);
                    }
                }
                var cresult = tempelem.EvulateValue(0, 0, vars);
                result.TextContent += cresult.TextContent;
                if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
                {
                    result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                    return result;
                }
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            }
            return result;
        }
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            this.CreateLocals();
            if (Evulator.IsParseMode)
            {
                return this.Render_Parse(tag, vars);
            }
            return this.Render_Default(tag, vars);

        }
        public override void RenderFinish(TextElement tag, object vars, TextEvulateResult latestResult)
        {
            base.RenderFinish(tag, vars, latestResult);
            this.DestroyLocals();
        }


    }
}
