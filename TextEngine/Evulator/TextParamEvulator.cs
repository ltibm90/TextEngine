using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class TextParamEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            for (int i = 0; i < tag.SubElementsCount; i++)
            {
                var elem = tag.SubElements[i];
                if(elem.ElementType == TextElementType.TextNode)
                {
                    result.TextContent += elem.Value;
                }
                else if(elem.ElementType == TextElementType.Parameter)
                {
                    result.TextContent += elem.EvulateValue().TextContent;
                }
            }
            return result;
        }
    }
}
