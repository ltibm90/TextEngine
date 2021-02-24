using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class TexttagEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            return new TextEvulateResult()
            {
                Result = TextEvulateResultEnum.EVULATE_TEXT,
                TextContent = tag.Value
            };
        }
    }
}
