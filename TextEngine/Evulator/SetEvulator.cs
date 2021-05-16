﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class SetEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            bool conditionok = this.ConditionSuccess(tag, "if", vars);
            var result = new TextEvulateResult
            {
                Result = TextEvulateResultEnum.EVULATE_NOACTION
            };

            if (conditionok)
            {
                string defname = tag.GetAttribute("name");
                if (string.IsNullOrEmpty(defname) || !defname.All((c) => char.IsLetterOrDigit(c))) return result;
                this.Evulator.DefineParameters.Set(defname, this.EvulateAttribute(tag.ElemAttr["value"], vars));
            }
            return result;
        }
    }
}
