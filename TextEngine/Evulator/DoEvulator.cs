using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class DoEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if ((tag.NoAttrib && string.IsNullOrEmpty(tag.Value)) || (!tag.NoAttrib && string.IsNullOrEmpty(tag.GetAttribute("c")))) return null;
            this.CreateLocals();
            int loop_count = 0;
            var result = new TextEvulateResult();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            do
            {
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
            } while (this.ConditionSuccess(tag));
            this.DestroyLocals();
            return result;
        }

    }
}
