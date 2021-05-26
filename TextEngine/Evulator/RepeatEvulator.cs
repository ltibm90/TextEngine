using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class RepeatEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var toResult = this.EvulateAttribute(tag.ElemAttr["count"], vars);
            if(toResult == null|| !TypeUtil.IsNumericType(toResult))
            {
                return null;
            }
            int tonum = (int)Convert.ChangeType(toResult, TypeCode.Int32);
            if (tonum < 1) return null;
            var varname = tag.GetAttribute("current_repeat");
            var result = new TextEvulateResult();
            this.CreateLocals();
            for (int i = 0; i < tonum; i++)
            {
                this.SetLocal(varname, i);
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
                if (this.Options.Max_Repeat_Loop != 0 && i > this.Options.Max_Repeat_Loop) break;
            }
            this.DestroyLocals();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}
