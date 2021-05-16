using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class StringUtils
    {
        public static List<string> SplitLineWithQuote(string text)
        {
            List<string> all = new List<string>();
            char quotechar = '0';
            int start = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var cur = text[i];

                if (quotechar == '0' && cur == '\'' || cur == '"') quotechar = cur;
                else if (quotechar != '0' && cur == quotechar) quotechar = '0';
                bool nextN = i + 1 < text.Length && text[i + 1] == '\n';
                if (quotechar == '0' && (cur == '\n' || (cur == '\r')))
                {
                    all.Add(text.Substring(start, i - start));
                    if (nextN) i++;
                    start = i + 1;
                }
            }
            if (start < text.Length) all.Add(text.Substring(start));
            return all;
        }
    }
}
