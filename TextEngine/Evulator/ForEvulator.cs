using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ForEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var varname = tag.GetAttribute("var");
            var startAttr = tag.ElemAttr["start"];
            var start = startAttr?.Value;
            var stepAttr = tag.ElemAttr["step"];
            var step = stepAttr?.Value;
            if (string.IsNullOrEmpty(start))
            {
                start = "0";
            }
            if (step == null || step == "0")
            {
                step = "1";
            }
            var toAttr = tag.ElemAttr["to"];
            if (string.IsNullOrEmpty(varname) && string.IsNullOrEmpty(step) && (toAttr == null || string.IsNullOrEmpty(toAttr.Value)))
            {
                return null;
            }
            object startres = null;
            if(startAttr != null)
            {
                if (startAttr.ParData == null)
                {
                    startAttr.ParData = new ParDecoder.ParDecode(start);
                    startAttr.ParData.Decode();
                }
                startres = this.EvulatePar(startAttr.ParData);
            }
            else
            {
                startres = 0;
            }
            object stepres = null;
            if (stepAttr != null)
            {
                if (stepAttr.ParData == null)
                {
                    stepAttr.ParData = new ParDecoder.ParDecode(step);
                    stepAttr.ParData.Decode();
                }
                startres = this.EvulatePar(stepAttr.ParData);
            }
            else
            {
                stepres = 1;
            }
            int startnum = 0;
            int stepnum = 0;
            int tonum = 0;
            if (!TypeUtil.IsNumericType(startres))
            {
                stepnum = 1;
            }
            else
            {
                stepnum = (int) Convert.ChangeType(stepres, TypeCode.Int32);
            }
            if(TypeUtil.IsNumericType(startres))
            {
                startnum = (int)Convert.ChangeType(startres, TypeCode.Int32); ;
            }
            var tores = this.EvulateAttribute(toAttr);
            if (!TypeUtil.IsNumericType(tores))
		    {
                return null;
            }
            tonum = (int)Convert.ChangeType(tores, TypeCode.Int32);
		    var result = new TextEvulateResult();
            this.CreateLocals();
            for (int i = startnum; i < tonum; i += stepnum)
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
            }
            this.DestroyLocals();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}
