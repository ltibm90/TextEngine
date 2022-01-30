using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Misc
{
    public class HTMLUTIL
    {
        public static string ToAttribute(TextElementAttributes attributes, List<string> exclude = null)
        {
            if (attributes == null || attributes.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var item in attributes)
            {
                if (exclude != null && exclude.Contains(item.Name)) continue;
                if(item.Value == null)
                {
                    sb.Append(" " + item.GetQuotedName());
                }
                else
                {
                    sb.Append(" " + item.GetQuotedName() + "=\"" + item.Value.Replace("\"", "\\\"") + "\"");

                }
            }
            return sb.ToString();
        }
    }
}
