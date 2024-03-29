﻿using System;
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
            var inlist = this.EvulateAttribute(tag.ElemAttr["in"], vars);
            if (inlist == null || !(inlist is IEnumerable list)) return null;
            this.CreateLocals();
            var result = new TextEvulateResult();
            uint loop_count = 0;
            foreach (var item in list)
            {
                this.SetLocal(varname, item);
                this.SetLocal("loop_count", loop_count++);
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
                if (this.Options.Max_ForEach_Loop != 0 && loop_count - 1 > this.Options.Max_ForEach_Loop) break;
            }
            this.DestroyLocals();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}
