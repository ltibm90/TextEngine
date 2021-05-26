using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class TextTagCommandEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var res = new TextEvulateResult();
            res.Result = TextEvulateResultEnum.EVULATE_NOACTION;
            var str = tag.Value;
            if (string.IsNullOrEmpty(str)) return res;
            var lines = StringUtils.SplitLineWithQuote(str, true);
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                line = line.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                this.EvulateText(line, vars);
            }
            return res;
        }
    }
}
