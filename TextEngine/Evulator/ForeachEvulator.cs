using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ForeachEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var varname = tag.GetAttribute("var");
            var inlist = this.EvulateAttribute(tag.ElemAttr["in"]);
            if (inlist == null || !(inlist is IEnumerable list)) return null;
            this.CreateLocals();
            var result = new TextEvulateResult();
            foreach (var item in list)
            {
                this.SetLocal(varname, item);
                var cresult = tag.EvulateValue(0, 0, vars);
                if (cresult == null) continue;
                result.TextContent += cresult.TextContent;
                if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
                {
                    result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                    this.DestroyLocals();
                    return result;
                }
                else if (cresult.Result == TextEvulateResultEnum.EVULATE_BREAK)
                {
                    break;
                }
            }
            this.DestroyLocals();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}
