﻿using System;
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
            var svar = new KeyValues<object>();
            this.Evulator.LocalVariables.Add(svar);
            for (int i = 0; i < tonum; i++)
            {
                svar[varname] = i;
                var cresult = tag.EvulateValue(0, 0, vars);
                if (cresult == null) continue;
                result.TextContent += cresult.TextContent;
                if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
                {
                    result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                    this.Evulator.LocalVariables.Remove(svar);
                    return result;
                }
                else if (cresult.Result == TextEvulateResultEnum.EVULATE_BREAK)
                {
                    break;
                }
            }
            this.Evulator.LocalVariables.Remove(svar);
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}
